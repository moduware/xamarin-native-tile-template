using Android.App;
using Android.Content;
using Android.OS;
using Platform.Core.CommonTypes;
using Platfrom.Tile.Shared.Structs;
using System;
using System.Threading.Tasks;

namespace Platform.Tile.Droid
{
    public class TileActivity : Activity
    {
        protected string TileId;
        protected TileArguments Arguments = new TileArguments();
        protected Core.Core Core;
        protected Utilities Utilities;

        private string CurrentConfiguration = String.Empty;

        /// <summary>
        /// Initializing core on tile activity creation and checking for connected gateways
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Utilities = new Utilities(this);

            // Running core in separate thread
            Task.Run(() =>
            {
                // Loading Platform Core
                Core = new Core.Core(code => RunOnUiThread(code), PassiveMode: true);

                Core.Gateways.GatewayConnected += Gateways_GatewayConnected;
                Core.Gateways.GatewayDisconnected += Gateways_GatewayDisconnected;

                CheckConnectedGateways();
            });
        }

        /// <summary>
        /// If tile was in background and has no connected devices when brought back to front, 
        /// checking if there are any connected
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            if (Core != null && Core.Gateways.List.Count == 0)
            {
                CheckConnectedGateways();
            }
        }

        /// <summary>
        /// Every time when new intent started we are waiting for arguments and\or configuration
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent.Data != null)
            {
                // For index we are getting full set of arguments and configuration
                if (intent.Data.Host == "index")
                {
                    Arguments = new TileArguments
                    {
                        TargetModuleUuid = new Uuid(intent.Data.GetQueryParameter("target-module-uuid")),
                        TargetModuleSlot = int.Parse(intent.Data.GetQueryParameter("target-module-slot")),
                        TargetModuleType = intent.Data.GetQueryParameter("target-module-type")
                    };

                    // At this step Core 99% not yet loaded, so we are saving config and core will use it when ready
                    CurrentConfiguration = intent.Data.GetQueryParameter("current-configuration");
                } // For configure we are receiving only configuration from main app
                else if (intent.Data.Host == "configure")
                {
                    // Loading configuration in core, merging known devices with information about them
                    var configuration = intent.Data.GetQueryParameter("current-configuration");
                    Core.API.MergeConfig(configuration);
                    RestoreDrivers();
                }
            }
        }

        // TODO: no need in this function after core is fixed
        /// <summary>
        /// Function restores drivers for all modules, use after merging config
        /// </summary>
        private void RestoreDrivers()
        {
            foreach(var gateway in Core.Gateways.List)
            {
                foreach(var module in gateway.Modules)
                {
                    if(module != null && module.Driver == null && module.UUID != null)
                    {
                        Core.API.Driver.RestoreDefault(module.UUID);
                    }
                }
            }
        }

        /// <summary>
        /// Actions to perfom when we are not connected to any gateway.
        /// Notify user and open moduware app.
        /// </summary>
        protected void NotConnectedActions()
        {
            // let user know that there are no connected gateways and it is required to open Moduware app for connection
            Utilities.ShowNotConnectedAlert(() =>
            {
                // open moduware application
                Utilities.OpenDashboard();
            });
        }

        /// <summary>
        /// Check if we have any connected gateways
        /// </summary>
        private void CheckConnectedGateways()
        {
            Task.Run(async () =>
            {
                var connected = await Core.Gateways.CheckConnected();

                if (!connected)
                {
                    NotConnectedActions();
                }
            });
        }

        /// <summary>
        /// When gateway connected start waiting for it's initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gateways_GatewayConnected(object sender, Core.EventArguments.GatewayEventArgs e)
        {
            e.Gateway.Initialized += Gateway_Initialized;
        }

        /// <summary>
        /// If gateway disconnected, check if it was last one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gateways_GatewayDisconnected(object sender, Core.EventArguments.ProductIdEventArgs e)
        {
            if(Core.Gateways.List.Count == 0)
            {
                NotConnectedActions();
            }
            
        }

        /// <summary>
        /// After gateway initialized we need request configuration or apply one received with arguments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gateway_Initialized(object sender, EventArgs e)
        {
            // TODO: Checking if all gateways initialized
            // For now we expect only one gateway
            if (CurrentConfiguration == String.Empty)
            {
                RequestCurrentConfiguration();
            }
            else
            {
                // Using configuration from arguments
                Core.API.MergeConfig(CurrentConfiguration);
                CurrentConfiguration = String.Empty;
            }
        }

        /// <summary>
        /// Request current configuration of gateway and modules
        /// </summary>
        public void RequestCurrentConfiguration()
        {
            Utilities.OpenDashboard($"index?tile-id={TileId}&action=getConfiguration");
        }

        /// <summary>
        /// Returns UUID of target module, if not specified will try to search for supported connected module.
        /// Will return empty UUID if there are no suitable module found.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Uuid GetUuidOfTargetModuleOrFirstOfType(string type)
        {
            if (Arguments.TargetModuleUuid != Uuid.Empty)
            {
                return Arguments.TargetModuleUuid;
            }
            else
            {
                var module = Core.API.Module.GetFirstByType(type);
                if (module != null)
                {
                    return module.UUID;
                }
            }

            return Uuid.Empty;
        }
    }
}
