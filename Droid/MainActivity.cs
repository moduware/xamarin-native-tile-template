using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.BLE;
using Serilog;
using System.Threading.Tasks;
using Android.Content;
using System;
using System.Collections.Generic;
using TileTemplate.Shared;
using System.Drawing;
using TileTemplate.Shared.Events;
using AssassinEventSystem;

namespace TileTemplate.Droid
{
    [Activity(Label = "Tile template", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    [IntentFilter(new [] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.template", Categories = new [] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : Activity
    {
        private SharedLogic _tile;

        public event EventHandler<ColorConfigButtonClickEventArgs> ColorConfigButtonClicked = delegate { };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // start logging
            var Prefix = "[TileTemplate]";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .CreateLogger();

            Assassin.Error += (s, e) => Log.Error($"{Prefix} Error: {e.Message}");
            Assassin.Warning += (s, e) => Log.Warning($"{Prefix} Warning: {e.Message}");
            Assassin.Information += (s, e) => Log.Information($"{Prefix} Information: {e.Message}");
            Assassin.Notification += (s, e) => Log.Information($"{Prefix} Notification: {e.Message}");

            Assassin.RaiseNotification("Logging started");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            SetupUiListeners();

            TileUtilities.SetImplemenation(new AndroidTileUtilities(this));
            _tile = new SharedLogic();

            // on launch
            //Task.Run(() => Initialize(Intent));
            Task.Run(async () =>
            {
                if (Intent.Data == null)
                {
                    await TileUtilities.ShowAlertAsync("Warning", "Please launch the tile from Moduware app", "Ok");
                    TileUtilities.OpenDashboard(); // open Moduware app
                }
                else
                {
                    await TileUtilities.ShowAlertAsync("Yay", "I love your start intentions!", "Ok");
                    _tile.SetArguments(Intent.Data.ToString());
                    if(!_tile.IsConnected)
                    {
                        await _tile.FindConnectedGateway();
                    }
                }
            });
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            Task.Run(async () =>
            {
                if (intent.Data != null)
                {
                    await TileUtilities.ShowAlertAsync("Ohh", "Why are you changing your intentions?", "Ok");
                    _tile.SetArguments(intent.Data.ToString());
                    if (!_tile.IsConnected)
                    {
                        await _tile.FindConnectedGateway();
                    }
                } // if tile re-openned but no arguments provided checking if we already have them
                else if(!_tile.HasArguments)
                {
                    // if we don't have arguments, alerting user again and rerouting to dashboard
                    await TileUtilities.ShowAlertAsync("Warning", "Please launch the tile from Moduware app", "Ok");
                    TileUtilities.OpenDashboard(); // open Moduware app
                }
            });
        }

        private void SetupUiListeners()
        {
            // Binding handlers to UI elements
            var ConfigButton = FindViewById<Button>(Resource.Id.button1);
            ConfigButton.Click += ConfigButtonClickHandler;

            var DashboardButton = FindViewById<Button>(Resource.Id.button2);
            //DashboardButton.Click += (s, e) => Utilities.OpenDashboard();
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

