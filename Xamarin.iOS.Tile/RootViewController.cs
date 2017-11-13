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

namespace XamariniOSTileTemplate
{
    public partial class RootViewController : TileViewController
    {
        private List<string> targetModuleTypes = new List<string>
        {
            "nexpaq.module.led",
            "moduware.module.led" // new module type using other kind of LEDs
        };

        public RootViewController() : base("RootViewController", null) { }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            // We need assign Id of our tile here, it is required for proper Dashboard - Tile communication
            TileId = "moduware.tile.template";

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
            /**
            * We can setup lister for received data here
            * you can remove it if your tile not receiving any data from module
            */
            Core.API.Module.DataReceived += ModuleDataReceivedHandler;

            /**
             * You can use raw data event to process raw data from module in byte format without 
             * processing it through module driver
             */
            // Core.API.Module.RawDataReceived += ...;
        }

        private void ModuleDataReceivedHandler(object sender, DriverParseResultEventArgs e)
        {
            var targetModuleUuid = GetUuidOfTargetModuleOrFirstOfType(targetModuleTypes);
            // If there are no supported modules plugged in
            if (targetModuleUuid == Uuid.Empty) return;
            // Ignoring data coming from non-target modules
            if (e.ModuleUUID != targetModuleUuid) return;

            // TODO: here we need to work with parsed data from module somehow 

            /**
             * It is a good practice to scope your data to some contexts
             * and first check context before processing data from module
             */
            // if(e.DataSource == "SensorValue") { ... }

            // outputing data variables to log
            foreach (var variable in e.Variables)
            {
                Log.Information(variable.Key + "= " + variable.Value);
            }
        }

        partial void SetColorButton_TouchUpInside(UIButton sender)
        {
            var RedNumber = int.Parse(RedColor.Text);
            var GreenNumber = int.Parse(GreenColor.Text);
            var BlueNumber = int.Parse(BlueColor.Text);

            // We are working with target module or first of type, what is fine for single module use
            var targetModuleUuid = GetUuidOfTargetModuleOrFirstOfType(targetModuleTypes);

            // Running command on found module
            if (targetModuleUuid != Uuid.Empty)
            {
                Core.API.Module.SendCommand(targetModuleUuid, "SetRGB", new[] { RedNumber, GreenNumber, BlueNumber });
            }
        }
    }
}