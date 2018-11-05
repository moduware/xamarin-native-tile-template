using System;
using UIKit;
using Plugin.BLE;
using System.Threading.Tasks;
using Serilog;
using System.Collections.Generic;
using TileTemplate.Shared;

namespace TileTemplate.iOS
{
    public partial class RootViewController : UIViewController
    {
        public SharedLogic Tile { get; }

        public RootViewController() {
            TileUtilities.SetImplemenation(new IosTileUtilities(InvokeOnMainThread));
            Tile = new SharedLogic();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        partial void SetColorButton_TouchUpInside(UIButton sender)
        {
            // Parsing values from UI
            // FIXME: no empty line check, what can cause exception or crash
            var r = int.Parse(RedColor.Text);
            var g = int.Parse(GreenColor.Text);
            var b = int.Parse(BlueColor.Text);

            // Sending command to module
            Tile.SetColorInRgb(r, g, b);
        }
    }
}