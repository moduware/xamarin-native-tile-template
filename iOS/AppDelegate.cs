using AssassinEventSystem;
using Foundation;
using Serilog;
using System.Threading.Tasks;
using TileTemplate.Shared;
using UIKit;

namespace TileTemplate.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        private string _query = null;
        private SharedLogic _tile;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // start logging
            StartLogging();

            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // If you have defined a root view controller, set it here:
            Window.RootViewController = new RootViewController();
            _tile = (Window.RootViewController as RootViewController).Tile;

            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // custom stuff here using different properties of the url passed in
            //var viewController = (RootViewController)Window.RootViewController;
            //viewController.OnQueryRecieved(url.AbsoluteString);
            _query = url.AbsoluteString;

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            Task.Run(async () =>
            {
                if(!_tile.HasArguments && _query == null)
                {
                    await TileUtilities.ShowAlertAsync("Warning", "Please launch the tile from Moduware app", "Ok");
                    TileUtilities.OpenDashboard(); // open Moduware app
                }
                else if(_query != null)
                {
                    await TileUtilities.ShowAlertAsync("Yay", "I love your intentions!", "Ok");
                    _tile.SetArguments(_query);
                    _query = null;
                    // looking for connected gateway
                    if(!_tile.IsConnected)
                    {
                        await _tile.FindConnectedGateway();
                    }
                }
            });
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
            //var viewController = (RootViewController)Window.RootViewController;
            //viewController.OnResumeActions();
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        private void StartLogging()
        {
            var Prefix = "[TileTemplate]";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.NSLog()
                .CreateLogger();

            Assassin.Error += (s, e) => Log.Error($"{Prefix} Error: {e.Message}");
            Assassin.Warning += (s, e) => Log.Warning($"{Prefix} Warning: {e.Message}");
            Assassin.Information += (s, e) => Log.Information($"{Prefix} Information: {e.Message}");
            Assassin.Notification += (s, e) => Log.Information($"{Prefix} Notification: {e.Message}");

            Assassin.RaiseNotification("Logging started");
        }
    }
}


