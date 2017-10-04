using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.BLE;
using Platform.Core;
using Serilog;
using System.Threading.Tasks;
using Platform.Core.CommonTypes;
using Xamarin.Tile.Structs;
using System;

namespace Xamarin.Android.Tile
{
    [Activity(Label = "Tile template", MainLauncher = true)]
    [IntentFilter(new [] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.led", Categories = new [] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : Activity
    {
        public TileArguments Arguments = new TileArguments();
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

            // Receiving arguments for tile
            if (Intent.Data != null)
            {
                if (Intent.Data.Host == "index")
                {
                    Arguments = new TileArguments
                    {
                        TargetModuleUuid = new Uuid(Intent.Data.GetQueryParameter("target-module-uuid")),
                        TargetModuleSlot = int.Parse(Intent.Data.GetQueryParameter("target-module-slot")),
                        TargetModuleType = Intent.Data.GetQueryParameter("target-module-type")
                    };
                }
            }

            // Launching core in separate thread from UI
            Task.Run(async () =>
            {
                // Loading Platform Core
                Core = new Core(code => RunOnUiThread(code), PassiveMode: true);
                // Handling errors happening in core of native tile
                // TODO: Switch to new events manager instead
                Core.Error += (sender, e) => Log.Information("[PlatformCore] Error: " + e.Message);

                // Searching for connected gateways
                var connected = await Core.Gateways.CheckConnected();

                if(!connected)
                {
                    // let user know that there are no connected gateways and it is required to open Moduware app for connection
                    ShowNotConnectedAlert(() =>
                    {
                        // TODO: open moduware application
                    });
                }
            });


            var ConfigButton = FindViewById<Button>(Resource.Id.button1);
            var RedEditbox = FindViewById<EditText>(Resource.Id.editText1);
            var GreenEditbox = FindViewById<EditText>(Resource.Id.editText2);
            var BlueEditbox = FindViewById<EditText>(Resource.Id.editText3);

            ConfigButton.Click += (source, e) =>
            {
                // getting color
                var RedNumber = int.Parse(RedEditbox.Text);
                var GreenNumber = int.Parse(GreenEditbox.Text);
                var BlueNumber = int.Parse(BlueEditbox.Text);

                Uuid targetUuid = GetTargetModuleOrFirstOfType("nexpaq.module.led");

                // Running command on found module
                if (targetUuid != Uuid.Empty)
                {
                    Core.API.Module.SendCommand(targetUuid, "SetRGB", new[] { RedNumber, GreenNumber, BlueNumber });
                }
            };
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
    }
}

