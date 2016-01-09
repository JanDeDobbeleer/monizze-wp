using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.ServiceLocation;
using Monizze.Common.Interfaces;
using Monizze.LiveTile;
using Monizze.View;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Monizze
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection _transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += App_UnhandledException;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            await RegisterBackgroundTasks();

            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame {CacheSize = 1};

                // TODO: change this value to a cache size that is appropriate for your application

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    _transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        _transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                var initial = (await ServiceLocator.Current.GetInstance<ICredentialManager>().IsLoggedIn())
                    ? typeof (MainView)
                    : typeof (LoginView);
                if (!rootFrame.Navigate(initial, e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Restores the content _transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame == null)
                return;
            rootFrame.ContentTransitions = _transitions ?? new TransitionCollection { new NavigationThemeTransition() };
            rootFrame.Navigated -= RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = ServiceLocator.Current.GetInstance<ILogger>();
            logger.Error("Exception occured on root level, app crashes", e.Exception);
            new ManualResetEvent(false).WaitOne(new TimeSpan(0, 0, 0, 2));
        }

        private async Task RegisterBackgroundTasks()
        {
            // Get rid of existing registrations.  
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                try
                {
                    task.Value.Unregister(false);
                }
                catch // hack  
                {
                    ServiceLocator.Current.GetInstance<ILogger>().Info("Could not register background task");
                }
            }
            // Call RemoveAccess
            try
            {
                BackgroundExecutionManager.RemoveAccess();
            }
            catch //happens when a new one is initialized
            {
                ServiceLocator.Current.GetInstance<ILogger>().Info("Could not remove access");
            }
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status.Equals(BackgroundAccessStatus.Denied))
                return;
            RegisterTask("Tile Task", typeof(BackgroundTask), new TimeTrigger(15, false));
        }

        private void RegisterTask(string name, Type bgType, IBackgroundTrigger trigger)
        {
            var builder = new BackgroundTaskBuilder
            {
                Name = name,
                TaskEntryPoint = bgType.FullName
            };
            builder.SetTrigger(trigger);
            builder.Register();
        }
    }
}