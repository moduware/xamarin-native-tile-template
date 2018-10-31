using Flurl;
using Moduware.Platform.Core.CommunicationProtocol;
using Moduware.Platform.Core.Connection;
using Moduware.Platform.Core.Connection.EventArguments;
using Newtonsoft.Json;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Serilog;
using System;
using System.Collections;
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
        public bool HasArguments => _arguments != null;

        public SharedLogic()
        {
            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            // tracking disconnect event
            _bluetoothAdapter.DeviceDisconnected += async (o, e) =>
            {
                if(e.Device == _gatewayDevice)
                {
                    ResetTile();
                    // show alert and switch to dashboard
                    await TileUtilities.ShowAlertAsync("Gateway disconnected", "Gateway was disconnected, please use Moduware app to reconnect", "Ok");
                    TileUtilities.OpenDashboard();
                }
            };
            // Tracking connection lost event
            _bluetoothAdapter.DeviceConnectionLost += async (o, e) =>
            {
                if (e.Device == _gatewayDevice)
                {
                    ResetTile();
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
            // Finding gateway among connected devices
            var connectedDevices = _bluetoothAdapter.GetSystemConnectedOrPairedDevices(new Guid[]
            {
                BleConnection.BleServiceId
            });
            
            foreach (var device in connectedDevices)
            {
                if(device.State == DeviceState.Connected || device.State == DeviceState.Limited)
                {
                    // converting listed device to connected with services
                    _gatewayDevice = await _bluetoothAdapter.ConnectToKnownDeviceAsync(device.Id);
                    break;
                }
            }
            if(_gatewayDevice == null)
            {
                throw new NullReferenceException("Cannot find connected gateway");
            }

            // creating BLE connection out of gateway device
            _connection = new BleConnection();
            await _connection.Initialize(_gatewayDevice, true);

            // Listning for messages from connection
            _connection.Received += _connectionMessageReceived;
        }

        private async void _connectionMessageReceived(object sender, ProtocolMessageReceivedEventArgs e)
        {
            var message = e.Message;
            // Waiting for slots states changed message
            if(message.Source == ProtocolMessageAddress.Gateway && message.Type.Equals(ProtocolMessageType.Gateway.SlotStatesInfo))
            {
                var states = new BitArray(message.Data);
                var slot = int.Parse(_arguments.Slot);
                if (states.Get(slot) == false)
                {
                    // target module was plugged out
                    // alert user and switch to dashboard
                    ResetTile();
                    await TileUtilities.ShowAlertAsync("Module plugged out", "Target module was plugged out, please reopen the tile from Moduware app", "Ok");
                    TileUtilities.OpenDashboard();
                }
            }
        }

        private void ResetTile()
        {
            _arguments = null;
            _gatewayDevice = null;
            _connection.Received -= _connectionMessageReceived;
            _connection = null;
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
