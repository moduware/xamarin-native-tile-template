﻿using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.BLE;
using Serilog;
using System.Threading.Tasks;
using Android.Content;
using System;
using Moduware.Platform.Tile.Droid;
using Moduware.Platform.Core.CommonTypes;

namespace XamarinAndroidTileTemplate
{
    [Activity(Label = "Tile template", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    [IntentFilter(new [] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.template", Categories = new [] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : TileActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TileId = "moduware.tile.template";

            // Logger to output messages from PlatformCore to console
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .CreateLogger();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var ConfigButton = FindViewById<Button>(Resource.Id.button1);
            ConfigButton.Click += ConfigButtonClickHandler;

            var DashboardButton = FindViewById<Button>(Resource.Id.button2);
            DashboardButton.Click += (s, e) => Utilities.OpenDashboard();
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

            // We are working with target module or first of type, what is fine for single module use
            Uuid targetUuid = GetUuidOfTargetModuleOrFirstOfType("nexpaq.module.led");

            // Running command on found module
            if (targetUuid != Uuid.Empty)
            {
                Core.API.Module.SendCommand(targetUuid, "SetRGB", new[] { RedNumber, GreenNumber, BlueNumber });
            }
        }
    }
}

