using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Platform.Tile.Shared;

namespace Platform.Tile.iOS
{
    class Utilities : IUtilities
    {
        public void OpenDashboard(string request = "")
        {
            var url = "moduware.application.dashboard://" + request;
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl(url)))
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
            }
        }

        public void ShowNotConnectedAlert(Action callback)
        {
            var alert = UIAlertController.Create("Not connected", "You are not connected to any moduware device, please search and connect to one.", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            //Let javascript handle the OK click by passing the completionHandler to the controller
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, callback);
        }
    }
}