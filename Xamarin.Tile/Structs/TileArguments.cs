﻿using Platform.Core.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Tile.Structs
{
    /// <summary>
    /// Arguments are parameters tile was launched with was launched with
    /// </summary>
    public class TileArguments
    {
        /// <summary>
        /// UUID of target module (clicked in dashboard)
        /// </summary>
        public Uuid TargetModuleUuid { get; set; } = Uuid.Empty;

        /// <summary>
        /// Slot of target module (clicked in dashboard)
        /// </summary>
        public int TargetModuleSlot { get; set; } = -1;

        /// <summary>
        /// Type of target module (clicked in dashboard)
        /// </summary>
        public string TargetModuleType { get; set; } = String.Empty;
    }
}