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

            Task.Run(() =>
            {
                // Loading Platform Core
                var Core = new Core(code => RunOnUiThread(code), PassiveMode: true, settings: new CoreSettings
                {
                    RequestManifests = true,
                    // TODO: make this setting true by default, as it is used like this in both Tile and App
                });

                Core.Error += (sender, e) => Log.Information("[PlatformCore] Error: " + e.Message);

                // Searching for connected gateways
                Core.Gateways.CheckConnected();
            });
            

        }
    }
}

