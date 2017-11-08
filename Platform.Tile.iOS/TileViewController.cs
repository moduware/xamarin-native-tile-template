using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Moduware.Platform.Tile.iOS
{
    public partial class TileViewController : UIViewController
    {
        private string v;
        private object p;

        public TileViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Utilities = new Utilities(RunOnUiThread);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            OnCreateActions();
        }

        private void RunOnUiThread(Action action)
        {
            InvokeOnMainThread(action);
        }
    }
}
