using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.Tile.Shared
{
    public interface IUtilities
    {
        void ShowNotConnectedAlert(Action callback);
        void OpenDashboard(string request = "");
    }
}
