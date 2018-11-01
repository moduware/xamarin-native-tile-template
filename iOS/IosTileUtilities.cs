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
    public class IosTileUtilities : ITileUtilities
    {
        Action<Action> _runOnUIThread;

        public TilePlatform Platform => TilePlatform.iOS;

        public IosTileUtilities(Action<Action> runOnUiThread)
        {
            _runOnUIThread = runOnUiThread;
        }

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
    }
}