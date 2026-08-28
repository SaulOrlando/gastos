using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public interface ICategoryService
{
    Task<List<CategoryViewModel>> GetForUserAsync(string userId);

    Task<CreateCategoryResult> CreateAsync(string name, string userId);

    Task<CreateCategoryResult> UpdateAsync(int id, string name, string userId);

    Task<CreateCategoryResult> DeleteAsync(int id, string userId);
}