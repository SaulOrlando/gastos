namespace FinanzApp.Web.ViewModels
{
    public class UserNavViewModel
    {
        public bool IsAuthenticated { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
    }
}
