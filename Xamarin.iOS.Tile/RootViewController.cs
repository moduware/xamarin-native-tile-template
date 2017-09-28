using System;

using UIKit;
using Platform.Core;
using Plugin.BLE;
using System.Threading.Tasks;
using Serilog;

namespace Xamarin.iOS.Tile
{
    public partial class RootViewController : UIViewController
    {
        private Core Core;

        public RootViewController() : base("RootViewController", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Task.Run(() =>
            {
                // Perform any additional setup after loading the view, typically from a nib.
                Core = new Core((code) => InvokeOnMainThread(code), PassiveMode: true, settings: new CoreSettings
                {
                    RequestManifests = true,
                    RequestModuleDrivers = true
                });
                
                Core.Error += (sender, e) => Log.Information("[PlatformCore] Error: " + e.Message);

                // Searching for connected gateways
                Core.Gateways.CheckConnected();
            });
        }

        partial void SetColorButton_TouchUpInside(UIButton sender)
        {
            var RedNumber = int.Parse(RedColor.Text);
            var GreenNumber = int.Parse(GreenColor.Text);
            var BlueNumber = int.Parse(BlueColor.Text);

            // finding all LED modules
            foreach (var module in Core.Gateways.List[0].Modules)
            {
                if (module == null) continue;
                if (module.Manifest.TypeID == "nexpaq.module.led")
                {
                    Core.API.Module.SendCommand(module.UUID, "SetRGB", new[] { RedNumber, GreenNumber, BlueNumber });
                }
            }
        }
    }
}