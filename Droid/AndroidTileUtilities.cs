using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TileTemplate.Shared;

namespace TileTemplate.Droid
{
    public class AndroidTileUtilities : ITileUtilities
    {
        private Context _context;

        public AndroidTileUtilities(Context context)
        {
            _context = context;
        }

        public Task ShowAlertAsync(string title, string message, string buttonText)
        {
            var t = new TaskCompletionSource<bool>();

            int DialogsTheme = 5;
            var DialogBuilder = new AlertDialog.Builder(_context, DialogsTheme);
            DialogBuilder.SetTitle(title);
            DialogBuilder.SetMessage(message);
            DialogBuilder.SetCancelable(false);
            DialogBuilder.SetPositiveButton(buttonText, (sender, e) => {
                t.TrySetResult(true);
            });

            (_context as Activity).RunOnUiThread(action: () => {
                var AlertDialog = DialogBuilder.Create();
                AlertDialog.Show();
            });

            return t.Task;
        }

        public void OpenDashboard()
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("moduware.application.dashboard://"));
            intent.AddFlags(ActivityFlags.NewTask);
            _context.StartActivity(intent);
        }
    }
}