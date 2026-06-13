using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Contracts.Categories;
using ProductCatalog.Api.Domain;
using ProductCatalog.Api.Services;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Category>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<Category>>> GetCategories(
        CancellationToken cancellationToken)
    {
        return Ok(await categoryService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Category>> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateAsync(request, cancellationToken);
        return Created($"/api/categories/{category.Id}", category);
    }
}
