using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Moduware.Platform.Tile.Shared;
using System.Threading.Tasks;

namespace Moduware.Platform.Tile.iOS
{
    class Utilities : IUtilities
    {
        Action<Action> _runOnUIThread;

        public Utilities(Action<Action> runOnUiThread)
        {
            _runOnUIThread = runOnUiThread;
        }

        public void OpenDashboard(string request = "")
        {
            var url = "moduware.application.dashboard://" + request;
            _runOnUIThread(() =>
            {
                if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl(url)))
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
                }
            });
        }

        public async void ShowNotConnectedAlert(Action callback)
        {
            await ShowAlertAsync("Not connected", "You are not connected to any moduware device, please search and connect to one.", "OK");
            callback(); 
        }

        public Task ShowAlertAsync(string title, string message, string buttonText)
        {
            var t = new TaskCompletionSource<bool>();
            _runOnUIThread(() =>
            {
                var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(buttonText, UIAlertActionStyle.Default, (action) => {
                    t.TrySetResult(true);
                }));

                //Let javascript handle the OK click by passing the completionHandler to the controller
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
            });
            return t.Task;
        }
    }
}