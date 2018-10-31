using System.Threading.Tasks;

namespace TileTemplate.Shared
{
    public interface ITileUtilities
    {
        Task ShowAlertAsync(string title, string message, string buttonText);
        void OpenDashboard();
    }
}