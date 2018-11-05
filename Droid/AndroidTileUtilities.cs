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
    /// <summary>
    /// Android implementation of Tile Utilities
    /// </summary>
    public class AndroidTileUtilities : ITileUtilities
    {
        #region Public Constructors
        /// <summary>
        /// Ctor for android tile utilities
        /// </summary>
        /// <param name="context">Android activity context</param>
        public AndroidTileUtilities(Context context)
        {
            _context = context;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Platform this tile runs on
        /// </summary>
        public TilePlatform Platform => TilePlatform.Android;
        #endregion

        #region Public Methods
        /// <summary>
        /// Simple dialog builder
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="buttonText">Dialog button text</param>
        /// <returns></returns>
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

        /// <summary>
        /// Opens Moduware dashboard app
        /// </summary>
        public void OpenDashboard()
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("moduware.application.dashboard://"));
            intent.AddFlags(ActivityFlags.NewTask);
            _context.StartActivity(intent);
        }
        #endregion

        #region Private Fields
        private Context _context;
        #endregion
    }
}