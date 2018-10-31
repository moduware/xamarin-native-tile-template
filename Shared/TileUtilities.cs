using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTemplate.Shared
{
    public static class TileUtilities
    {
        private static ITileUtilities _tileUtilitiesImplementation;

        public static void SetImplemenation(ITileUtilities implementation)
        {
            _tileUtilitiesImplementation = implementation;
        }

        public static Task ShowAlertAsync(string title, string message, string buttonText)
        {
            CheckImplementation();
            return _tileUtilitiesImplementation.ShowAlertAsync(title, message, buttonText);
        }

        public static void OpenDashboard()
        {
            CheckImplementation();
            _tileUtilitiesImplementation.OpenDashboard();
        }

        private static void CheckImplementation()
        {
            if (_tileUtilitiesImplementation == null) throw new MissingImplementationException("You need set implementation before using tile utilities");
        }
    }

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
