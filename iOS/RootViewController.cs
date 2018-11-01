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
        private SharedLogic _tile;

        public SharedLogic Tile => _tile;

        public RootViewController() {
            TileUtilities.SetImplemenation(new IosTileUtilities(InvokeOnMainThread));
            _tile = new SharedLogic();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        partial void SetColorButton_TouchUpInside(UIButton sender)
        {
            var r = int.Parse(RedColor.Text);
            var g = int.Parse(GreenColor.Text);
            var b = int.Parse(BlueColor.Text);

            _tile.SetColorInRgb(r, g, b).Wait();
        }
    }
}