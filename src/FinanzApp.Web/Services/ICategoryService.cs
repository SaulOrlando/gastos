using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface ICategoryService
{
    Task<List<CategoryViewModel>> GetForUserAsync(string userId);

    Task<CreateCategoryResult> CreateAsync(string name, string userId, string color = "#21C3D6");

    Task<CreateCategoryResult> UpdateAsync(int id, string name, string userId, string color = "#21C3D6");

    Task<CreateCategoryResult> DeleteAsync(int id, string userId);

    Task<Dictionary<string, string>> GetColorsByNamesAsync(string userId);
}