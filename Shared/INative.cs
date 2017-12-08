using Moduware.Platform.Core.CommonTypes;
using Moduware.Platform.Tile.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileTemplate.Shared.Events;

namespace TileTemplate.Shared
{
    public interface INative
    {
        /// <summary>
        /// General native tile utilities
        /// </summary>
        IUtilities Utilities { get; }

        /// <summary>
        /// General function to search for connected module
        /// </summary>
        /// <param name="types"></param>
        /// <returns>Target module or first of type</returns>
        Uuid GetUuidOfTargetModuleOrFirstOfType(List<string> types);

        /// <summary>
        /// Happens when gateway-module config applied and we can start sending commands,
        /// useful when module need some command to start working
        /// </summary>
        event EventHandler ConfigurationApplied;

        /// <summary>
        /// DEMO: happens when color configuration button was clicked in native UI
        /// </summary>
        event EventHandler<ColorConfigButtonClickEventArgs> ColorConfigButtonClicked;
    }
}
