using System.Security.Claims;
using FinanzApp.Web.Models;
using FinanzApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanzApp.Web.Components
{
    public class TopNavViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TopNavViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var principal = User as ClaimsPrincipal;
            var isAuthenticated = principal?.Identity?.IsAuthenticated == true;

            string fullName = string.Empty;
            string profilePicture = string.Empty;

            if (isAuthenticated && principal != null)
            {
                var user = await _userManager.GetUserAsync(principal);
                if (user != null)
                {
                    fullName = string.IsNullOrWhiteSpace(user.FullName)
                        ? user.Email ?? string.Empty
                        : user.FullName;
                    profilePicture = user.ProfilePicture ?? string.Empty;
                }
            }

            return View(new UserNavViewModel
            {
                IsAuthenticated = isAuthenticated,
                FullName = fullName,
                ProfilePicture = profilePicture
            });
        }
    }
}
