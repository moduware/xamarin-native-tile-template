﻿using Android.App;
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
using AssassinEventSystem;

namespace TileTemplate.Droid
{
    [Activity(Label = "Tile template", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    [IntentFilter(new [] { "android.intent.action.VIEW" }, DataScheme = "moduware.tile.template", Categories = new [] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" })]
    public class MainActivity : Activity
    {
        private SharedLogic _tile;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // start logging
            StartLogging();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Binding handlers to UI elements
            SetupUiListeners();

            // Initializing tile logic and utils
            TileUtilities.SetImplemenation(new AndroidTileUtilities(this));
            _tile = new SharedLogic();

            // on launch
            Task.Run(async () =>
            {
                // If no arguments provided on startup rerouting to Moduware app
                if (Intent.Data == null)
                {
                    await TileUtilities.ShowAlertAsync("Warning", "Please launch the tile from Moduware app", "Ok");
                    TileUtilities.OpenDashboard(); // open Moduware app
                } // otherwise assigning these arguments and looking for connected gateway
                else
                {
                    //await TileUtilities.ShowAlertAsync("Yay", "I love your start intentions!", "Ok");
                    _tile.SetArguments(Intent.Data.ToString());
                    // looking for connected gateway
                    await _tile.FindConnectedGateway();
                    
                }
            });
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            Task.Run(async () =>
            {
                // Assigning tile arguments, they can replace previously assigned arguments
                // so that tile can be retargeted
                if (intent.Data != null)
                {
                    //await TileUtilities.ShowAlertAsync("Ohh", "Why are you changing your intentions?", "Ok");
                    _tile.SetArguments(intent.Data.ToString());

                    // if arguments were changed and we are not connected lets look for connected gateway
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

        protected override void OnDestroy()
        {
            FinishAffinity();
            base.OnDestroy();
            Process.KillProcess(Process.MyPid());
        }

        private void StartLogging()
        {
            var Prefix = "[TileTemplate]";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AndroidLog()
                .CreateLogger();

            Assassin.Error += (s, e) => Log.Error($"{Prefix} Error: {e.Message}");
            Assassin.Warning += (s, e) => Log.Warning($"{Prefix} Warning: {e.Message}");
            Assassin.Information += (s, e) => Log.Information($"{Prefix} Information: {e.Message}");
            Assassin.Notification += (s, e) => Log.Information($"{Prefix} Notification: {e.Message}");

            Assassin.RaiseNotification("Logging started");
        }

        private void SetupUiListeners()
        {
            // Binding handlers to UI elements
            var ConfigButton = FindViewById<Button>(Resource.Id.button1);
            ConfigButton.Click += ConfigButtonClickHandler;

            var DashboardButton = FindViewById<Button>(Resource.Id.button2);
            DashboardButton.Click += (s, e) => TileUtilities.OpenDashboard();
        }

        private async void ConfigButtonClickHandler(object source, EventArgs e)
        {
            // Parsing values from UI
            // FIXME: no empty line check, what can cause exception or crash
            var r = int.Parse(FindViewById<EditText>(Resource.Id.editText1).Text);
            var g = int.Parse(FindViewById<EditText>(Resource.Id.editText2).Text);
            var b = int.Parse(FindViewById<EditText>(Resource.Id.editText3).Text);

            // last check, if user made it to this place and we have no arguments or connection
            // something is wrong with our workflow
            if (!_tile.HasArguments || !_tile.IsConnected)
            {
                throw new Exception("Cannot send command to module as there are no connection or tile arguments");
            }

            // Sending command to module
            await _tile.SetColorInRgb(r, g, b);
        }
    }
}

