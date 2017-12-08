using System;

using UIKit;
using Moduware.Platform.Core;
using Plugin.BLE;
using System.Threading.Tasks;
using Serilog;
using Moduware.Platform.Tile.iOS;
using Moduware.Platform.Core.CommonTypes;
using System.Collections.Generic;
using Moduware.Platform.Core.EventArguments;
using TileTemplate.Shared;
using TileTemplate.Shared.Events;

namespace TileTemplate.iOS
{
    public partial class RootViewController : TileViewController, INative
    {
        private SharedLogic _tile;

        public event EventHandler<ColorConfigButtonClickEventArgs> ColorConfigButtonClicked = delegate { };
        public RootViewController() : base("RootViewController", null) { }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            // We need assign Id of our tile here, it is required for proper Dashboard - Tile communication
            TileId = SharedLogic.TileId;

            // Logger to output messages from PlatformCore to console
            Log.Logger = new LoggerConfiguration()
                .WriteTo.NSLog()
                .CreateLogger();

            // We need to know when core is ready so we can start listening for data from gateways
            CoreReady += CoreReadyHandler;

            base.ViewDidLoad();
        }

        private void CoreReadyHandler(Object source, EventArgs e)
        {
            // When core is ready initialising our logic
            _tile = new SharedLogic(Core, this);
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
            ColorConfigButtonClicked(this, new ColorConfigButtonClickEventArgs
            {
                Red = color.Item1,
                Green = color.Item2,
                Blue = color.Item3
            });
        }
    }
}