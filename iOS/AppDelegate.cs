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
            // if tile was open with arguments saving them for processing in OnActivated
            _query = url.AbsoluteString;

            return true;
        }

        public override void OnActivated(UIApplication application)
        {
            Task.Run(async () =>
            {
                // if tile has no arguments and none were provided
                // rerouting user to Moduware app
                if (!_tile.HasArguments && _query == null)
                {
                    await TileUtilities.ShowAlertAsync("Warning", "Please launch the tile from Moduware app", "Ok");
                    TileUtilities.OpenDashboard(); // open Moduware app
                } // If some arguments were provided assigning them to tile
                else if(_query != null)
                {
                    //await TileUtilities.ShowAlertAsync("Yay", "I love your intentions!", "Ok");
                    _tile.SetArguments(_query);
                    _query = null;
                    // If arguments we provided but we are not connected looking for a connected gateway
                    if(!_tile.IsConnected)
                    {
                        await _tile.FindConnectedGateway();
                    }
                }
            });
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


