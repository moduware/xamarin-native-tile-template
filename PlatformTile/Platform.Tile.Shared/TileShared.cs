using Flurl;
using Moduware.Platform.Core;
using Moduware.Platform.Core.CommonTypes;
using Moduware.Platform.Tile.Shared;
using Moduware.Platfrom.Tile.Shared.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if __IOS__
namespace Moduware.Platform.Tile.iOS
{ 
    public partial class TileViewController : UIKit.UIViewController
#endif
#if __ANDROID__
namespace Moduware.Platform.Tile.Droid
{ 
    public partial class TileActivity : Android.App.Activity
#endif
    {
        public Core.Core Core { get { return _core; } }

        protected string TileId;
        protected TileArguments Arguments = new TileArguments();
        protected Core.Core _core;
        protected IUtilities Utilities;

        private string CurrentConfiguration = String.Empty;

        public void OnCreateActions()
        {
            RunCore();
        }

        public void OnResumeActions()
        {
            if (Core != null && Core.Gateways.List.Count == 0)
            {
                CheckConnectedGateways();
            }
        }

        public void OnQueryRecieved(string queryUrl)
        {
            var url = new Url(queryUrl);
            var domain = url.Path.Replace(TileId + "://", String.Empty);
            // For index we are getting full set of arguments and configuration
            if (domain == "index")
            {
                Arguments = new TileArguments
                {
                    // UUID of target module from Dashboard
                    TargetModuleUuid = new Uuid(url.QueryParams["target-module-uuid"].ToString()),
                    // Slot that module plugged in, for 2 size modules check "orientation" in manifest 
                    // to calculate second taken slot
                    TargetModuleSlot = int.Parse(url.QueryParams["target-module-slot"].ToString()),
                    // Type of the target module, use for progressive enchancement, when some module types
                    // are more powerfull then others
                    TargetModuleType = url.QueryParams["target-module-type"].ToString()
                };

                // At this step Core 99% not yet loaded, so we are saving config and core will use it when ready
                CurrentConfiguration = url.QueryParams["current-configuration"].ToString();
            } // For configure we are receiving only configuration from main app
            else if (domain == "configure")
            {
                // Loading configuration in core, merging known devices with information about them
                var configuration = url.QueryParams["current-configuration"].ToString();
                Core.API.MergeConfig(configuration);
            }
        }

        private void RunCore()
        {
            // Running core in separate thread
            Task.Run(() =>
            {
                // Loading Platform Core
                _core = new Core.Core(code => RunOnUiThread(code), PassiveMode: true);
                
                Core.Gateways.GatewayConnected += Gateways_GatewayConnected;
                Core.Gateways.GatewayDisconnected += Gateways_GatewayDisconnected;

                CoreReady(this, EventArgs.Empty);

                CheckConnectedGateways();
            });
        }

        /// <summary>
        /// Actions to perfom when we are not connected to any gateway.
        /// Notify user and open moduware app.
        /// </summary>
        public void NotConnectedActions()
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
        public void CheckConnectedGateways()
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
            if (Core.Gateways.List.Count == 0)
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
        public Uuid GetUuidOfTargetModuleOrFirstOfType(List<string> types)
        {
            if (Arguments.TargetModuleUuid != Uuid.Empty)
            {
                return Arguments.TargetModuleUuid;
            }
            else
            {
                foreach(var moduleType in types)
                {
                    var module = Core.API.Module.GetFirstByType(moduleType);
                    if (module != null && module.UUID != null)
                    {
                        return module.UUID;
                    }
                }
            }

            return Uuid.Empty;
        }

        public Uuid GetUuidOfTargetModuleOrFirstOfType(string type)
        {
            return GetUuidOfTargetModuleOrFirstOfType(new List<string> { type });
        }

        public event EventHandler CoreReady = delegate { };
    }
}
