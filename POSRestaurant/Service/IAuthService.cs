using POSRestaurant.Models;

namespace POSRestaurant.Service
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
        Task LogoutAsync();
        bool IsAuthenticated();
        CurrentUser GetCurrentUser();
        bool HasPermission(string permissionName);
    }
}
