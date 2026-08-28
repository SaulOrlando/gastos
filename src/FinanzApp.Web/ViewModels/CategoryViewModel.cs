namespace FinanzApp.Web.ViewModels;

public class CategoryViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsSystem { get; set; }
}

public class CreateCategoryResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public CategoryViewModel? Category { get; set; }
}