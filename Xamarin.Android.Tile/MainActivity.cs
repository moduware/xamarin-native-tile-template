using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.BLE;
using Platform.Core;
using Serilog;
using System.Threading.Tasks;
using Platform.Core.CommonTypes;
using Xamarin.Tile.Structs;
using Android.Content;
using System;

namespace XamarinAndroidTileTemplate
{
    [Activity(Label = "Tile template", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    [IntentFilter(new [] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.led", Categories = new [] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : Activity
    {
        public string tileId = "moduware.tile.led";

        private TileArguments Arguments = new TileArguments();
        private string CurrentConfiguration = String.Empty;
        private Core Core;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Logger to output messages from PlatformCore to console
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .CreateLogger();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var ConfigButton = FindViewById<Button>(Resource.Id.button1);
            ConfigButton.Click += ConfigButtonClickHandler;

            FoundConnectedDevices += MainActivity_FoundConnectedDevices;

            Task.Run(() =>
            {
                // Loading Platform Core
                Core = new Core(code => RunOnUiThread(code), PassiveMode: true);
                // Handling errors happening in core of native tile
                // TODO: Switch to new events manager instead
                Core.Error += (sender, e) => Log.Information("[PlatformCore] Error: " + e.Message);
                Core.Gateways.GatewayConnected += (o, e) =>
                {
                    e.Gateway.Initialized += Gateway_Initialized;
                };

                CheckConnectedGateways();
            });
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (Core != null && Core.Gateways.List.Count == 0)
            {
                CheckConnectedGateways();
            }

            /*if (Core.Gateways.List.Count == 0)
            {
                Task.Run(async () =>
                {
                    // Searching for connected gateways
                    var connected = await Core.Gateways.CheckConnected();

                    if (!connected)
                    {
                        // let user know that there are no connected gateways and it is required to open Moduware app for connection
                        ShowNotConnectedAlert(() =>
                        {
                            // open moduware application
                            OpenDashboard();
                        });
                    }
                    else if (CurrentConfiguration == String.Empty)
                    {
                        //RequestCurrentConfiguration();
                    } else
                    {
                        Core.API.MergeConfig(CurrentConfiguration);   
                        // TODO: load current configuration using function created by Moemen
                    }
                });
            } else if (CurrentConfiguration == String.Empty)
            {
                //RequestCurrentConfiguration();
            } else
            {
                Core.API.MergeConfig(CurrentConfiguration);
                // TODO: load current configuration using function created by Moemen
            }*/
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent.Data != null)
            {
                if (intent.Data.Host == "index")
                {
                    Arguments = new TileArguments
                    {
                        TargetModuleUuid = new Uuid(intent.Data.GetQueryParameter("target-module-uuid")),
                        TargetModuleSlot = int.Parse(intent.Data.GetQueryParameter("target-module-slot")),
                        TargetModuleType = intent.Data.GetQueryParameter("target-module-type")
                    };

                    CurrentConfiguration = intent.Data.GetQueryParameter("current-configuration");
                }
                else if (intent.Data.Host == "configure")
                {
                    var configuration = intent.Data.GetQueryParameter("current-configuration");
                    Core.API.MergeConfig(configuration);
                }
            }
        }

        private void CheckConnectedGateways()
        {
            Task.Run(async () =>
            {
                var connected = await Core.Gateways.CheckConnected();

                if (connected)
                {
                    FoundConnectedDevices(this, EventArgs.Empty);
                }

                if (!connected)
                {
                    // let user know that there are no connected gateways and it is required to open Moduware app for connection
                    ShowNotConnectedAlert(() =>
                    {
                        // open moduware application
                        OpenDashboard();
                    });
                }
            });
        }

        private void MainActivity_FoundConnectedDevices(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Gateway_Initialized(object sender, EventArgs e)
        {
            // TODO: Checking if all gateways initialized
            // For now we expect only one gateway
            if(CurrentConfiguration == String.Empty)
            {
                RequestCurrentConfiguration();
            } else
            {
                Core.API.MergeConfig(CurrentConfiguration);
                CurrentConfiguration = String.Empty;
            }
        }

        private void ConfigButtonClickHandler(Object source, EventArgs e)
        {
            
            var RedEditbox = FindViewById<EditText>(Resource.Id.editText1);
            var GreenEditbox = FindViewById<EditText>(Resource.Id.editText2);
            var BlueEditbox = FindViewById<EditText>(Resource.Id.editText3);

            // getting color
            var RedNumber = int.Parse(RedEditbox.Text);
            var GreenNumber = int.Parse(GreenEditbox.Text);
            var BlueNumber = int.Parse(BlueEditbox.Text);

            Uuid targetUuid = GetTargetModuleOrFirstOfType("nexpaq.module.led");

            // Running command on found module
            if (targetUuid != Uuid.Empty)
            {
                var module = Core.API.Module.GetByUUID(targetUuid);
                if (module.Driver == null)
                {
                    Core.API.Driver.RestoreDefault(targetUuid);
                }

                Core.API.Module.SendCommand(targetUuid, "SetRGB", new[] { RedNumber, GreenNumber, BlueNumber });
            }
        }

        private void ShowNotConnectedAlert(Action callback)
        {
            int DialogsTheme = 5;
            var DialogBuilder = new AlertDialog.Builder(this, DialogsTheme);
            DialogBuilder.SetTitle("Not connected");
            DialogBuilder.SetMessage("You are not connected to any moduware device, please search and connect to one.");
            DialogBuilder.SetCancelable(false);
            DialogBuilder.SetPositiveButton("Ok", (sender, e) => {
                callback();
            });
           
            RunOnUiThread(action: () => {
                var AlertDialog = DialogBuilder.Create();
                AlertDialog.Show();
            });
        }

        /// <summary>
        /// Help function to search for first module of specific type
        /// </summary>
        /// <param name="type">Module type</param>
        /// <returns>module or null if not found</returns>
        private Platform.Core.Product.Module GetFirstModuleByType(string type)
        {
            foreach(var gateway in Core.Gateways.List)
            {
                foreach(var module in gateway.Modules)
                {
                    if (module == null) continue;
                    if (module.TypeID == type) return module;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns UUID of target module, if not specified will try to search for supported connected module.
        /// Will return empty UUID if there are no suitable module found.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Uuid GetTargetModuleOrFirstOfType(string type)
        {
            if (Arguments.TargetModuleUuid != Uuid.Empty)
            {
                return Arguments.TargetModuleUuid;
            }
            else
            {
                var module = GetFirstModuleByType(type);
                if (module != null)
                {
                    return module.UUID;
                }
            }

            return Uuid.Empty;
        }

        private void OpenDashboard(string request = "")
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("moduware.application.dashboard://" + request));
            intent.AddFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        }

        private void RequestCurrentConfiguration()
        {
            // TODO: request moduware application for current configuration
            OpenDashboard($"index?tile-id={tileId}&action=getConfiguration");
            // ? should we notify user about this or just do it silently ?
        }

        private event EventHandler FoundConnectedDevices = delegate { };
    }
}

