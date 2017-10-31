using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Platform.Tile.iOS
{
    public partial class TileViewController : UIViewController
    {
        private string v;
        private object p;

        public TileViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // TODO: Utilities = new Utilities(this);

            OnCreateActions();
        }

        private void RunOnUiThread(Action action)
        {
            InvokeOnMainThread(action);
        }
    }
}
