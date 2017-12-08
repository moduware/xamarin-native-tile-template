using Moduware.Platform.Core;
using Moduware.Platform.Core.CommonTypes;
using Moduware.Platform.Core.EventArguments;
using Serilog;
using System;
using System.Collections.Generic;

namespace TileTemplate.Shared
{
    public class SharedLogic
    {
        public static List<string> _targetModuleTypes = new List<string>
        {
            "nexpaq.module.led",
            "moduware.module.led" // new module type using other kind of LEDs
        };
        public static string TileId = "moduware.tile.template";

        private Core _core;
        private Uuid _targetModuleUuid;
        private string _targetModuleType;

        private Func<List<string>, Uuid> _moduleSearchFunc;
        private INative _native;

        public SharedLogic(Core core, INative nativeMethods)
        {
            _core = core;
            _native = nativeMethods;

            // After configuration recieved we need find module we want work with
            _native.ConfigurationApplied += (o, e) => SetupTargetModule();
            // If module was pulled, we need to check if there are still supported module 
            _core.API.Module.Pulled += ModuleDisconnectedHandler;
            _core.API.Gateway.Disconnected += ModuleDisconnectedHandler;

            /**
            * We can setup lister for received data here
            * you can remove it if your tile not receiving any data from module
            */
            _core.API.Module.DataReceived += ModuleDataReceivedHandler;

            /**
             * You can use raw data event to process raw data from module in byte format without 
             * processing it through module driver
             */
            // Core.API.Module.RawDataReceived += ...;

            _native.ColorConfigButtonClicked += ColorConfigButtonClicked;
        }

        /// <summary>
        /// After module disconnected we need make sure we can continue working
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ModuleDisconnectedHandler(Object o, EventArgs e)
        {
            SetupTargetModule();
        }


        /// <summary>
        /// If there are any supported module plugged in with preference to target module using it,
        /// if there are no supported modules, showing alert and openning dashboard
        /// </summary>
        public void SetupTargetModule()
        {
            bool noModule = false;
            _targetModuleUuid = _moduleSearchFunc(_targetModuleTypes);
            if (_targetModuleUuid != Uuid.Empty)
            {
                var module = _core.API.Module.GetByUUID(_targetModuleUuid);
                if (module == null)
                {
                    noModule = true;
                }
                else
                {
                    _targetModuleType = module.TypeID;
                }
            }
            else
            {
                noModule = true;
            }

            if (noModule)
            {
                _native.Utilities.ShowNoSupportedModuleAlert(() =>
                {
                    _native.Utilities.OpenDashboard();
                });
            }
        }

        public void SetColorInRgb(int r, int g, int b)
        {
            _core.API.Module.SendCommand(_targetModuleUuid, "SetRGB", new[] { r, g, b });
        }

        private void ColorConfigButtonClicked(object sender, Events.ColorConfigButtonClickEventArgs e)
        {
            SetColorInRgb(e.Red, e.Green, e.Blue);
        }

        /// <summary>
        /// Handle data coming from module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Module message parsed using driver</param>
        private void ModuleDataReceivedHandler(object sender, DriverParseResultEventArgs e)
        {
            var targetModuleUuid = _native.GetUuidOfTargetModuleOrFirstOfType(_targetModuleTypes);
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

    }
}
