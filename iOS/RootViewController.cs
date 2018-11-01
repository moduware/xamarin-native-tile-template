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

        private Tuple<int, int, int> GetColorFromUi()
        {
            var RedNumber = int.Parse(RedColor.Text);
            var GreenNumber = int.Parse(GreenColor.Text);
            var BlueNumber = int.Parse(BlueColor.Text);

            return new Tuple<int, int, int>(RedNumber, GreenNumber, BlueNumber);
        }

        partial void SetColorButton_TouchUpInside(UIButton sender)
        {
            var color = GetColorFromUi();
            //ColorConfigButtonClicked(this, new ColorConfigButtonClickEventArgs
            //{
            //    Red = color.Item1,
            //    Green = color.Item2,
            //    Blue = color.Item3
            //});
        }
    }
}