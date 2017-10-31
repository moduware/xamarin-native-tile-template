using Android.App;
using Android.Content;
using Android.OS;
using Platform.Core.CommonTypes;
using Platfrom.Tile.Shared.Structs;
using Platfrom.Tile.Shared;
using System;
using System.Threading.Tasks;

namespace Platform.Tile.Droid
{
    public partial class TileActivity : Android.App.Activity
    {
        /// <summary>
        /// Initializing core on tile activity creation and checking for connected gateways
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Utilities = new Utilities(this);

            OnCreateActions();
        }

        /// <summary>
        /// If tile was in background and has no connected devices when brought back to front, 
        /// checking if there are any connected
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            OnResumeActions();
        }

        /// <summary>
        /// Every time when new intent started we are waiting for arguments and\or configuration
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent.Data != null)
            {
                OnQueryRecieved(intent.Data.ToString());
            }
        }
    }
}
