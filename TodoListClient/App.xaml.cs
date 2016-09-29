﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TodoListClient
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        // The following Azure AD properties are used by multiple pages in the app

        // Properties of the web API invoked
        public const string ResourceID = "https://damocotest1.onmicrosoft.com/TodoListServiceMT";        
        public const string APIHostname = "https://localhost:44321";
        public const string APIOnboardPath = "/API/SignUp/Onboard";
        public const string APITodoListPath = "/API/todolist";

        // Properties of the native client app
        public const string ClientID = "e2e3fe03-cbfd-42e5-9196-669af4209a3d";
        public static Uri ReturnUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();

        // Properties used for communicating with the Windows Azure AD tenant of choice
        public const string CommonAuthority = "https://login.microsoftonline.com/common";
        public static AuthenticationContext AuthenticationContext { get; set; }

          
        // At start time we verify whether we already have a cached token (hence we can already use the app)
        // or if we need to drive the user through the sign up/sign in experience
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Initialize the AuthenticationContext with the common (tenantless) endpoint
                App.AuthenticationContext = new AuthenticationContext(App.CommonAuthority);
                // if we aready have tokens in the cache
                if (App.AuthenticationContext.TokenCache.ReadItems().Count() > 0)
                {
                    // re-bind the AuthenticationContext to the authority that sourced the token in the cache
                    // this is needed for the cache to work when asking a token from that authority
                    // (the common endpoint never triggers cache hits)
                    string cachedAuthority = App.AuthenticationContext.TokenCache.ReadItems().First().Authority;
                    App.AuthenticationContext = new AuthenticationContext(cachedAuthority);
                    // navigate directly to the main app page
                    rootFrame.Navigate(typeof(TodoListPage), e.Arguments);
                }
                else
                {
                    // no previous tokens. Navigate to the welcome page
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
