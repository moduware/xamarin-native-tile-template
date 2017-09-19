using Android.App;
using Android.Widget;
using Android.OS;
using Platform.Core;
using Plugin.BLE;

namespace Xamarin.Android.Tile
{
    [Activity(Label = "Xamarin.Android.Tile", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Loading Platform Core
            var Core = new Core(code => RunOnUiThread(code), PassiveMode: true, settings: new CoreSettings
            {
                RequestManifests = true,
                // TODO: make this setting true by default, as it is used like this in both Tile and App
            });

            // Searching for connected gateways
            Core.Gateways.CheckConnected();

        }
    }
}

