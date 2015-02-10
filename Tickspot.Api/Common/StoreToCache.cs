using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using Tickspot.Common.Interfaces;

namespace Tickspot.Api.Common
{
    public class StoreToCache
    {
        private const string FolderName = "Api";
        private const string FileNameFormat = "{0}.dat";

        protected readonly ILogger Logger;

        public StoreToCache(ILogger logger)
        {
            Logger = logger;
        }

        #region cache implementation
        private async Task SaveToCache<T>(string value) where T : class, new()
        {
            try
            {
                var folder = await GetCacheFolder();
                var f = await folder.CreateFileAsync(string.Format(FileNameFormat, typeof(T)), CreationCollisionOption.ReplaceExisting);
                using (var st = await f.OpenStreamForWriteAsync())
                {
                    var s = new DataContractSerializer(typeof(string));
                    s.WriteObject(st, value);
                }
            }
            catch (Exception e)
            {
                Logger.Error(GetType() + " Could not store cache for " + typeof(T), e);
            }
        }

        public async Task SaveToCache<T>(T value) where T : class, new()
        {
            var val = JsonConvert.SerializeObject(value);
            await SaveToCache<T>(val);
        }

        public async Task<T> GetFromCache<T>() where T : class, new()
        {
            try
            {
                var folder = await GetCacheFolder();
                using (var st = await folder.OpenStreamForReadAsync(string.Format(FileNameFormat, typeof(T))))
                {
                    var t = new DataContractSerializer(typeof(string));
                    var root = t.ReadObject(st) as string;
                    return JsonConvert.DeserializeObject<T>(root);
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Warn(GetType() + " No cache found for " + typeof(T));
            }
            catch (Exception e)
            {
                Logger.Error(GetType() + " Error handling cache for " + typeof(T), e);
            }
            return default(T);
        }

        protected async Task ClearCache()
        {
            var localFolder = await GetCacheFolder();
            var files = await localFolder.GetFilesAsync();
            foreach (var t in files)
            {
                await t.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        private async Task<StorageFolder> GetCacheFolder()
        {
            try
            {
                return await ApplicationData.Current.LocalFolder.GetFolderAsync(FolderName);
            }
            catch (Exception e)
            {
                Logger.Error(GetType() + " unable to retrive cache folder", e);
            }
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync(FolderName);
        }
        #endregion
    }
}
