using Flurl;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;

namespace TileTemplate.Shared
{
    public class SharedLogic
    {
        private TileArguments _arguments = null;

        public bool HasArguments => _arguments != null;

        public SharedLogic()
        {
            
        }

        public void SetArguments(string queryUrl)
        {
            var url = new Url(queryUrl);
            var argumentsJson = url.QueryParams["args"].ToString();
            var arguments = JsonConvert.DeserializeObject<TileArguments>(argumentsJson);
            _arguments = arguments;
        }



        /// <summary>
        /// After module disconnected we need make sure we can continue working
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ModuleDisconnectedHandler(Object o, EventArgs e)
        {
            
        }


        //public void SetColorInRgb(int r, int g, int b)
        //{
        //    _core.API.Module.SendCommand(_targetModuleUuid, "SetRGB", new[] { r, g, b });
        //}

        //private void ColorConfigButtonClicked(object sender, Events.ColorConfigButtonClickEventArgs e)
        //{
        //    //SetColorInRgb(e.Red, e.Green, e.Blue);
        //}

        ///// <summary>
        ///// Handle data coming from module
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e">Module message parsed using driver</param>
        //private void ModuleDataReceivedHandler(object sender, DriverParseResultEventArgs e)
        //{
        //    var targetModuleUuid = _native.GetUuidOfTargetModuleOrFirstOfType(_targetModuleTypes);
        //    // If there are no supported modules plugged in
        //    if (targetModuleUuid == Uuid.Empty) return;
        //    // Ignoring data coming from non-target modules
        //    if (e.ModuleUUID != targetModuleUuid) return;

        //    // TODO: here we need to work with parsed data from module somehow 

        //    /**
        //     * It is a good practice to scope your data to some contexts
        //     * and first check context before processing data from module
        //     */
        //    // if(e.DataSource == "SensorValue") { ... }

        //    // outputing data variables to log
        //    foreach (var variable in e.Variables)
        //    {
        //        Log.Information(variable.Key + "= " + variable.Value);
        //    }
        //}

    }
}
