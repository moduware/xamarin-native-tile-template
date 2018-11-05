using System.Threading.Tasks;

namespace TileTemplate.Shared
{
    /// <summary>
    /// Interface that describes methods required for native tile utilities implementation
    /// </summary>
    public interface ITileUtilities
    {
        /// <summary>
        /// Platform this tile runs on
        /// </summary>
        TilePlatform Platform { get; }

        /// <summary>
        /// Simple dialog builder
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="buttonText">Dialog button text</param>
        /// <returns></returns>
        Task ShowAlertAsync(string title, string message, string buttonText);

        /// <summary>
        /// Opens Moduware dashboard app
        /// </summary>
        void OpenDashboard();
    }

    /// <summary>
    /// Platforms the tile can run on. For now Android and iOS.
    /// </summary>
    public enum TilePlatform
    {
        Android,
        iOS
    }
}