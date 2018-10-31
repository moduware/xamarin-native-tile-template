using Flurl;
using Moduware.Platform.Core.Connection;
using Newtonsoft.Json;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TileTemplate.Shared
{
    public class SharedLogic
    {
        private TileArguments _arguments = null;
        private IAdapter _bluetoothAdapter;
        private IDevice _gatewayDevice = null;
        private BleConnection _connection = null;

        public bool IsConnected => _connection != null;

        public SharedLogic()
        {
            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            _bluetoothAdapter.DeviceDisconnected += async (o, e) =>
            {
                if(e.Device == _gatewayDevice)
                {
                    _connection = null;
                    // show alert and switch to dashboard
                    await TileUtilities.ShowAlertAsync("Gateway disconnected", "Gateway was disconnected, please use Moduware app to reconnect", "Ok");
                    TileUtilities.OpenDashboard();
                }
            };
            _bluetoothAdapter.DeviceConnectionLost += async (o, e) =>
            {
                if (e.Device == _gatewayDevice)
                {
                    _connection = null;
                    // show alert and switch to dashboard
                    await TileUtilities.ShowAlertAsync("Gateway lost", "Gateway connection lost, please use Moduware app to reconnect", "Ok");
                    TileUtilities.OpenDashboard();
                }
            };
        }

        public void SetArguments(string queryUrl)
        {
            var url = new Url(queryUrl);
            var argumentsJson = url.QueryParams["args"].ToString();
            var arguments = JsonConvert.DeserializeObject<TileArguments>(argumentsJson);
            _arguments = arguments;
        }

        public async Task FindConnectedGateway()
        {
            var connectedDevices = _bluetoothAdapter.GetSystemConnectedOrPairedDevices(new Guid[]
            {
                BleConnection.BleServiceId
            });
            
            foreach (var device in connectedDevices)
            {
                if(device.State == DeviceState.Connected || device.State == DeviceState.Limited)
                {
                    // we need convert device from list to proper device with services
                    // FIXME: must be called from a main thread
                    //try
                    //{
                        _gatewayDevice = await _bluetoothAdapter.ConnectToKnownDeviceAsync(device.Id);
                    //} catch(Exception e)
                    //{

                    //}
                    
                    //gatewayDevice = device;
                    break;
                }
            }
            if(_gatewayDevice == null)
            {
                throw new NullReferenceException("Cannot find connected gateway");
            }
            _connection = new BleConnection();
            await _connection.Initialize(_gatewayDevice, true);
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
