using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.System.Threading;

namespace Monizze.Common.Model
{
    /// <summary>
    /// This is an advanced useage, where you want to intercept the logging messages and devert them somewhere
    /// besides ETW.
    /// </summary>
    public sealed class StorageFileEventListener : EventListener
    {
        /// <summary>
        /// Storage file to be used to write logs
        /// </summary>
        StorageFile _storageFile;
        /// <summary>
        /// Name of the current event listener
        /// </summary>
        readonly string _name;

        /// <summary>
        /// Format for the filenames containing the date
        /// </summary>
        private const string NameFormat = "{0:dd-MM-yyyy}_{1}";
        /// <summary>
        /// The format to be used by logging.
        /// </summary>
        private const string Format = "{0:dd-MM-yyyy HH\\:mm\\:ss\\:ffff}\tType: {1}\tId: {2}\tMessage: '{3}'";

        /// <summary>
        /// Contains the local cache of the lines
        /// </summary>
        volatile List<string> _linesCache = new List<string>();
        /// <summary>
        /// Contains the number of lines that must be written to the log file
        /// </summary>
        volatile int _linesToProcess;
        /// <summary>
        /// Contains the sync root for the lines cache
        /// </summary>
        readonly object _syncRoot = new object();
        /// <summary>
        /// Contains a delay timer
        /// </summary>
        ThreadPoolTimer _timer;

        /// <summary>
        /// Gets the log file name (without path)
        /// </summary>
        public string LogFileName
        {
            get
            {
                return string.Format(NameFormat, DateTime.Now, _name.Replace(" ", "_")) + ".log";
            }
        }

        /// <summary>
        /// Gets the backup log file name (without path)
        /// </summary>
        public string BackupLogFilename
        {
            get
            {
                return string.Format(NameFormat, DateTime.Now, _name.Replace(" ", "_")) + ".log";
            }
        }

        /// <summary>
        /// Gets the full log file name (with path)
        /// </summary>
        public string FullLogFilename
        {
            get
            {
                return System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, LogFileName);
            }
        }

        /// <summary>
        /// Gets the full backup log file name (with path)
        /// </summary>
        public string FullBackupLogFilename
        {
            get
            {
                return System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, BackupLogFilename);
            }
        }

        /// <summary>
        /// Initializes a new instance of the listener
        /// </summary>
        /// <param name="name">the name of the listener. The file name of the log will be "(name).log"</param>
        public StorageFileEventListener(string name)
        {
            _name = name;
            Debug.WriteLine("StorageFileEventListener for {0} has name {1}", GetHashCode(), _name);
        }

        /// <summary>
        /// Writes the event to the log file stack
        /// </summary>
        /// <param name="eventData">the data to be written</param>
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            var newFormatedLine = string.Format(Format, DateTime.Now, eventData.Level, eventData.EventId, eventData.Payload[0]);
            Debug.WriteLine(newFormatedLine);
            AddLine(newFormatedLine);
        }

        /// <summary>
        /// True, if the log file is not written completly
        /// </summary>
        public bool InProgress
        {
            get { return _linesToProcess != 0; }
        }
        /// <summary>
        /// Waits for the worker to complete
        /// </summary>
        public void Flush()
        {
            while (_linesToProcess != 0)
                Task.Delay(10).Wait();
        }

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public override void Dispose()
        {
            if (_storageFile != null)
            {
                Flush();
                _storageFile = null;
            }
            base.Dispose();
        }

        /// <summary>
        /// Add a line to the cache and start the worker
        /// </summary>
        /// <param name="line">the line to add</param>
        public void AddLine(string line)
        {
            lock (_syncRoot)
            {
                _linesCache.Add(line);
                _linesToProcess++;
                // only start a new delay timer, if there already is no one
                if (_timer == null)
                    _timer = ThreadPoolTimer.CreateTimer(source => DelayedWorker(), new TimeSpan(0, 0, 0, 0, 500));
            }

        }

        /// <summary>
        /// The worker is writing the lines from the lines cache to the file
        /// </summary>
        void DelayedWorker()
        {
            // synchronize access to log lines in cache
            lock (_syncRoot)
            {
                // remove the timer to make place for a new one
                // ReSharper disable once RedundantCheckBeforeAssignment
                if (_timer != null)
                    _timer = null;

                // synchronized check, if we have lines to log
                if (_linesCache.Count == 0)
                    return;

                // get lines and provide a new lines list for following log entries in the mean time
                var lines = _linesCache;
                _linesCache = new List<string>();

                try
                {
                    // make five tries, if background workers are accessing the file also - so access could denied sometimes
                    for (var i = 0; i < 3; i++)
                    {
                        try
                        {
                            // for the first call - create the storage file
                            if (_storageFile == null)
                            {
                                var storageFileTask = ApplicationData.Current.LocalFolder.CreateFileAsync(LogFileName, CreationCollisionOption.OpenIfExists);
                                Wait(storageFileTask);
                                _storageFile = storageFileTask.GetResults();
                            }

                            // check the size of the log file
                            CheckLogFileSize();

                            // write the buffered log entries to the file
                            var appendLinesAction = FileIO.AppendLinesAsync(_storageFile, lines);
                            Wait(appendLinesAction);

                            // retry on errors
                            if (appendLinesAction.Status == AsyncStatus.Error)
                                continue;

                            return;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("DelayedWorker exception: " + ex.Message + DateTime.Now);
                        }

                        // Writing to the log file was not successful. Wait for a short period and do a retry
                        Task.Delay(300).Wait();
                    }
                }
                finally
                {
                    // so we have processed some log entries - notifiy that
                    _linesToProcess -= lines.Count;
                }
            }
        }

        /// <summary>
        /// Check, that the log file size will not explode
        /// </summary>
        /// <remarks>
        /// When the log file size is more than 1 MB, the current log file is copied to the BackupLogFileName.
        /// </remarks>
        private void CheckLogFileSize()
        {
            try
            {
                var bufferAction = FileIO.ReadBufferAsync(_storageFile);
                Wait(bufferAction);
                // when the log file reaches 1 mb file size, copy the log to bak file and empty the log file.
                const int mb = 1024 * 1000;
                if (bufferAction.GetResults().Length <= mb)
                    return;
                var bakFileAction = ApplicationData.Current.LocalFolder.CreateFileAsync(BackupLogFilename, CreationCollisionOption.GenerateUniqueName);
                Wait(bakFileAction);

                //copy and replace the old backup log
                var copyAction = _storageFile.CopyAndReplaceAsync(bakFileAction.GetResults());
                Wait(copyAction);

                Debug.WriteLine("clear file");

                // empty the existing log file
                var emptyFileAction = FileIO.WriteTextAsync(_storageFile, string.Empty);
                Wait(emptyFileAction);
            }
            catch (Exception ex)
            {
                // Do nothing here. It will be handled the next time
                Debug.WriteLine("CheckLogFileSize failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Waits until the status is not "Started"
        /// </summary>
        /// <param name="action">the action</param>
        static void Wait(IAsyncAction action)
        {
            while (action.Status == AsyncStatus.Started)
                Task.Delay(10).Wait();
        }

        /// <summary>
        /// Waits until the status is not "Started"
        /// </summary>
        /// <param name="bufferAction">the action</param>
        static void Wait(IAsyncInfo bufferAction)
        {
            while (bufferAction.Status == AsyncStatus.Started)
                Task.Delay(10).Wait();
        }

        /// <summary>
        /// Waits until the status is not "Started"
        /// </summary>
        /// <param name="bakFileAction">the action</param>
        static void Wait(IAsyncOperation<StorageFile> bakFileAction)
        {
            while (bakFileAction.Status == AsyncStatus.Started)
                Task.Delay(10).Wait();
        }
    }
}
