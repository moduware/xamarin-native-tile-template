using Flurl;
using Moduware.Platform.Core.CommunicationProtocol;
using Moduware.Platform.Core.CommunicationProtocol.SourceDestinationStrategies;
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

        public async Task SetColorInRgb(int r, int g, int b)
        {
            var slot = int.Parse(_arguments.Slot);
            var message = ProtocolMessage.Create(new PhoneToModuleStrategy(slot), new ProtocolMessageType("2702"), new[] { (byte)r, (byte)g, (byte)b });
            await _connection.Send(message);
            //_core.API.Module.SendCommand(_targetModuleUuid, "SetRGB", new[] { r, g, b });
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


        
    }
}
