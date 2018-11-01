using System.Threading.Tasks;

namespace TileTemplate.Shared
{
    public interface ITileUtilities
    {
        TilePlatform Platform { get; }

        Task ShowAlertAsync(string title, string message, string buttonText);
        void OpenDashboard();
    }

    public enum TilePlatform
    {
        Android,
        iOS
    }
}