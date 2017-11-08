using System;

using UIKit;
using Moduware.Platform.Core;
using Plugin.BLE;
using System.Threading.Tasks;
using Serilog;
using Moduware.Platform.Tile.iOS;
using Moduware.Platform.Core.CommonTypes;

namespace XamariniOSTileTemplate
{
    public partial class RootViewController : TileViewController
    {
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
            TileId = "moduware.tile.template";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.NSLog()
                .CreateLogger();

            base.ViewDidLoad();
        }

        partial void SetColorButton_TouchUpInside(UIButton sender)
        {
            var RedNumber = int.Parse(RedColor.Text);
            var GreenNumber = int.Parse(GreenColor.Text);
            var BlueNumber = int.Parse(BlueColor.Text);

            // We are working with target module or first of type, what is fine for single module use
            Uuid targetUuid = GetUuidOfTargetModuleOrFirstOfType("nexpaq.module.led");

            // Running command on found module
            if (targetUuid != Uuid.Empty)
            {
                Core.API.Module.SendCommand(targetUuid, "SetRGB", new[] { RedNumber, GreenNumber, BlueNumber });
            }
        }
    }
}