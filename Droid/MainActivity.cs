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

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            SetupUiListeners();

            _tile = new SharedLogic();

            // on launch
            //Task.Run(() => Initialize(Intent));
            Task.Run(async () =>
            {
                if (Intent.Data == null)
                {
                    await ShowAlertAsync("Warning", "Please launch the tile from Moduware app", "Ok");
                    OpenDashboard(); // open Moduware app
                }
                else
                {
                    await ShowAlertAsync("Yay", "I love your start intentions!", "Ok");
                    _tile.SetArguments(Intent.Data.ToString());
                }
            });
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            // when tile already launched
            //Task.Run(() => Initialize(intent));
            Task.Run(async () =>
            {
                if (intent.Data != null)
                {
                    await ShowAlertAsync("Ohh", "Why are you changing your intentions?", "Ok");
                    _tile.SetArguments(intent.Data.ToString());
                }
            });
        }

        //private async Task Initialize(Intent intent)
        //{
        //    if (intent.Data == null)
        //    {
        //        await ShowAlertAsync("Warning", "Please launch the tile from Moduware app", "Ok");
        //        OpenDashboard(); // open Moduware app
        //    } else
        //    {
        //        await ShowAlertAsync("Yay", "I love your intentions!", "Ok");
        //    }
        //}

        public Task ShowAlertAsync(string title, string message, string buttonText)
        {
            var t = new TaskCompletionSource<bool>();

            int DialogsTheme = 5;
            var DialogBuilder = new AlertDialog.Builder(this, DialogsTheme);
            DialogBuilder.SetTitle(title);
            DialogBuilder.SetMessage(message);
            DialogBuilder.SetCancelable(false);
            DialogBuilder.SetPositiveButton(buttonText, (sender, e) => {
                t.TrySetResult(true);
            });

            (this as Activity).RunOnUiThread(action: () => {
                var AlertDialog = DialogBuilder.Create();
                AlertDialog.Show();
            });

            return t.Task;
        }

        public void OpenDashboard()
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("moduware.application.dashboard://"));
            intent.AddFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        }

        private void SetupUiListeners()
        {
            // Binding handlers to UI elements
            var ConfigButton = FindViewById<Button>(Resource.Id.button1);
            ConfigButton.Click += ConfigButtonClickHandler;

            var DashboardButton = FindViewById<Button>(Resource.Id.button2);
            //DashboardButton.Click += (s, e) => Utilities.OpenDashboard();
        }

        private void CoreReadyHandler(Object source, EventArgs e)
        {
            // When core is ready initialising our logic
            //_tile = new SharedLogic(Core, this); 
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

