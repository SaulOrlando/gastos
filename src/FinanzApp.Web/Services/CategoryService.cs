using FinanzApp.Web.Models;
using FinanzApp.Web.Repositories;
using FinanzApp.Web.ViewModels;

namespace FinanzApp.Web.Services;

public class CategoryService : ICategoryService
{
    private const int MaxNameLength = 50;

    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryViewModel>> GetForUserAsync(string userId)
    {
        var categories = await _categoryRepository.GetAllForUserAsync(userId);

        return categories.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            IsSystem = c.UserId == null
        }).ToList();
    }

    public async Task<CreateCategoryResult> CreateAsync(string name, string userId)
    {
        var trimmedName = name?.Trim() ?? string.Empty;

        var validation = ValidateName(trimmedName);
        if (validation is not null)
        {
            return validation;
        }

        if (await _categoryRepository.NameExistsAsync(trimmedName, userId))
        {
            return Error("Ya tienes una categoría con ese nombre.");
        }

        var category = new ExpenseCategory
        {
            UserId = userId,
            Name = trimmedName,
            IsSystem = false,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);

        return new CreateCategoryResult
        {
            Success = true,
            Category = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                IsSystem = false
            }
        };
    }

    public async Task<CreateCategoryResult> UpdateAsync(int id, string name, string userId)
    {
        var trimmedName = name?.Trim() ?? string.Empty;

        var validation = ValidateName(trimmedName);
        if (validation is not null)
        {
            return validation;
        }

        var category = await _categoryRepository.GetOwnedByIdAsync(id, userId);

        if (category is null)
        {
            return Error("No encontramos esa categoría. Quizá ya la eliminaste.");
        }

        if (await _categoryRepository.NameExistsAsync(trimmedName, userId, id))
        {
            return Error("Ya tienes una categoría con ese nombre.");
        }

        category.Name = trimmedName;
        await _categoryRepository.UpdateAsync(category);

        return new CreateCategoryResult
        {
            Success = true,
            Category = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                IsSystem = false
            }
        };
    }

    public async Task<CreateCategoryResult> DeleteAsync(int id, string userId)
    {
        var category = await _categoryRepository.GetOwnedByIdAsync(id, userId);

        if (category is null)
        {
            return Error("No encontramos esa categoría. Quizá ya la eliminaste.");
        }

        await _categoryRepository.DeleteAsync(id, userId);

        return new CreateCategoryResult { Success = true };
    }

    private static CreateCategoryResult? ValidateName(string name)
    {
        if (name.Length == 0)
        {
            return Error("Escribe un nombre para la categoría.");
        }

        if (name.Length > MaxNameLength)
        {
            return Error("El nombre no puede pasar de 50 caracteres.");
        }

        return null;
    }

    private static CreateCategoryResult Error(string message) =>
        new() { Success = false, Error = message };
}