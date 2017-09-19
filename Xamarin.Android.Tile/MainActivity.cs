using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.BLE;
using Platform.Core;
using Serilog;
using System.Threading.Tasks;

namespace Xamarin.Android.Tile
{
    [Activity(Label = "Tile template", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Logger to output messages from PlatformCore to console
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .CreateLogger();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Core Core = null;

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

                // finding all LED modules
                foreach(var module in Core.Gateways.List[0].Modules)
                {
                    if (module == null) continue;
                    if (module.Manifest.TypeID == "nexpaq.module.led")
                    {
                        Core.API.Module.SendCommand(module.UUID, "SetRGB", new[] { RedNumber, GreenNumber, BlueNumber });
                    }
                }
            };

            

            Task.Run(() =>
            {
                // Loading Platform Core
                Core = new Core(code => RunOnUiThread(code), PassiveMode: true, settings: new CoreSettings
                {
                    RequestManifests = true,
                    RequestModuleDrivers = true
                    // TODO: make this setting true by default, as it is used like this in both Tile and App
                });

                Core.Error += (sender, e) => Log.Information("[PlatformCore] Error: " + e.Message);

                // Searching for connected gateways
                Core.Gateways.CheckConnected();
            });
            

        }
    }
}

