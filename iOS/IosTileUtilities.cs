using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using TileTemplate.Shared;
using UIKit;

namespace TileTemplate.iOS
{
    /// <summary>
    /// iOS implementation of Tile Utilities
    /// </summary>
    public class IosTileUtilities : ITileUtilities
    {
        #region Public Constructors
        /// <summary>
        /// Ctor for ios tile utilities
        /// </summary>
        /// <param name="runOnUiThread">Activity to run code on a UI thread</param>
        public IosTileUtilities(Action<Action> runOnUiThread)
        {
            _runOnUIThread = runOnUiThread;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Platform this tile runs on
        /// </summary>
        public TilePlatform Platform => TilePlatform.iOS;
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
            _runOnUIThread(async () =>
            {
                var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(buttonText, UIAlertActionStyle.Default, (action) => {
                    t.TrySetResult(true);
                }));

                //Let javascript handle the OK click by passing the completionHandler to the controller
                //await UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewControllerAsync(false);
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
            });
            return t.Task;
        }

        /// <summary>
        /// Opens Moduware dashboard app
        /// </summary>
        public void OpenDashboard()
        {
            var url = "moduware.application.dashboard://";
            _runOnUIThread(() =>
            {
                if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl(url)))
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
                }
            });
        }
        #endregion

        #region Private Fields
        private Action<Action> _runOnUIThread;
        #endregion
        
    }
}