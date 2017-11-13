using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Moduware.Platform.Tile.Shared;

namespace Moduware.Platform.Tile.Droid
{
    public class Utilities : IUtilities
    {
        private Context context;

        public Utilities(Context context)
        {
            this.context = context;
        }

        public void ShowNotConnectedAlert(Action callback)
        {
            int DialogsTheme = 5;
            var DialogBuilder = new AlertDialog.Builder(context, DialogsTheme);
            DialogBuilder.SetTitle("Not connected");
            DialogBuilder.SetMessage("You are not connected to any moduware device, please search and connect to one.");
            DialogBuilder.SetCancelable(false);
            DialogBuilder.SetPositiveButton("Ok", (sender, e) => {
                callback();
            });

            (context as Activity).RunOnUiThread(action: () => {
                var AlertDialog = DialogBuilder.Create();
                AlertDialog.Show();
            });
        }

        public void OpenDashboard(string request = "")
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("moduware.application.dashboard://" + request));
            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }
    }
}