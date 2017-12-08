using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.BLE;
using Serilog;
using System.Threading.Tasks;
using Android.Content;
using System;
using Moduware.Platform.Tile.Droid;
using Moduware.Platform.Core.CommonTypes;
using Moduware.Platform.Core.EventArguments;
using System.Collections.Generic;
using TileTemplate.Shared;
using System.Drawing;
using TileTemplate.Shared.Events;

namespace TileTemplate.Droid
{
    [Activity(Label = "Tile template", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    [IntentFilter(new [] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.template", Categories = new [] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : TileActivity, INative
    {
        private SharedLogic _tile;

        public event EventHandler<ColorConfigButtonClickEventArgs> ColorConfigButtonClicked = delegate { };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // We need assign Id of our tile here, it is required for proper Dashboard - Tile communication
            TileId = SharedLogic.TileId;

            // Logger to output messages from PlatformCore to console
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .CreateLogger();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            CoreReady += CoreReadyHandler;

            SetupUiListeners();
        }

        private void SetupUiListeners()
        {
            // Binding handlers to UI elements
            var ConfigButton = FindViewById<Button>(Resource.Id.button1);
            ConfigButton.Click += ConfigButtonClickHandler;

            var DashboardButton = FindViewById<Button>(Resource.Id.button2);
            DashboardButton.Click += (s, e) => Utilities.OpenDashboard();
        }

        private void CoreReadyHandler(Object source, EventArgs e)
        {
            // When core is ready initialising our logic
            _tile = new SharedLogic(Core, this); 
        }

        private Color GetColorFromUi()
        {
            var RedEditbox = FindViewById<EditText>(Resource.Id.editText1);
            var GreenEditbox = FindViewById<EditText>(Resource.Id.editText2);
            var BlueEditbox = FindViewById<EditText>(Resource.Id.editText3);

            // getting color
            var RedNumber = int.Parse(RedEditbox.Text);
            var GreenNumber = int.Parse(GreenEditbox.Text);
            var BlueNumber = int.Parse(BlueEditbox.Text);

            var color = Color.FromArgb(RedNumber, GreenNumber, BlueNumber);
            return color;
        }

        private void ConfigButtonClickHandler(Object source, EventArgs e)
        {
            var color = GetColorFromUi();
            ColorConfigButtonClicked(this, new ColorConfigButtonClickEventArgs
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            });
        }
    }
}

