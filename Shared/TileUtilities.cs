using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTemplate.Shared
{
    /// <summary>
    /// Portable tile utilities. Available everywhere in tile.
    /// </summary>
    public static class TileUtilities
    {
        #region Public Properties
        /// <summary>
        /// Platform this tile runs on
        /// </summary>
        public static TilePlatform Platform
        {
            get
            {
                CheckImplementation();
                return _tileUtilitiesImplementation.Platform;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets platform specific implementation for Tile Utilities.
        /// Must be called before use of tile utilities.
        /// </summary>
        /// <param name="implementation">Platform specific implementation</param>
        public static void SetImplemenation(ITileUtilities implementation)
        {
            _tileUtilitiesImplementation = implementation;
        }

        /// <summary>
        /// Simple dialog builder
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="buttonText">Dialog button text</param>
        /// <returns></returns>
        public static Task ShowAlertAsync(string title, string message, string buttonText)
        {
            CheckImplementation();
            return _tileUtilitiesImplementation.ShowAlertAsync(title, message, buttonText);
        }

        /// <summary>
        /// Opens Moduware dashboard app
        /// </summary>
        public static void OpenDashboard()
        {
            CheckImplementation();
            _tileUtilitiesImplementation.OpenDashboard();
        }
        #endregion

        #region Private Fields
        private static ITileUtilities _tileUtilitiesImplementation;
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks if tile utilities implementation was assigned
        /// </summary>
        private static void CheckImplementation()
        {
            if (_tileUtilitiesImplementation == null) throw new MissingImplementationException("You need set implementation before using tile utilities");
        }
        #endregion
        
    }

    /// <summary>
    /// Exception that will be thrown for missing implementation
    /// </summary>
    internal class MissingImplementationException : Exception
    {
        public MissingImplementationException()
        {
        }

        public MissingImplementationException(string message) : base(message)
        {
        }

        public MissingImplementationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
