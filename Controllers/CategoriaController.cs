using Microsoft.AspNetCore.Mvc;
using OpenBooksBackMobile.DTOs.CategoriaDtos;
using OpenBooksBackMobile.Shared;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly CategoriaService _categoriaService;

    public CategoriasController(CategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategorias(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _categoriaService.GetAllPagedAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaResponseDto>> GetById(int id)
    {
        var categoria = await _categoriaService.GetByIdAsync(id);

        if (categoria == null)
            return NotFound();

        return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaResponseDto>> Create(
        [FromBody] CategoriaCreateDto dto)
    {
        var categoria = await _categoriaService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = categoria.Id },
            categoria
        );
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoriaUpdateDto dto)
    {
        var updated = await _categoriaService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoriaService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

}
