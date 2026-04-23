using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.DTOs.CategoriaDtos;
using OpenBooksBackMobile.Entities;
using OpenBooksBackMobile.Shared;

public class CategoriaService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoriaService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<CategoriaResponseDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = _context.Categorias.AsNoTracking();

        int totalRecords = await query.CountAsync();

        var categorias = await query
            .OrderBy(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dto = _mapper.Map<List<CategoriaResponseDto>>(categorias);

        return new PagedResult<CategoriaResponseDto>
        {
            Results = dto,
            TotalRecords = totalRecords,
            PageSize = pageSize,
            CurrentPage = pageNumber
        };
    }

    public async Task<CategoriaResponseDto?> GetByIdAsync(int id)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null) return null;

        return _mapper.Map<CategoriaResponseDto>(categoria);
    }

    public async Task<CategoriaResponseDto> CreateAsync(CategoriaCreateDto dto)
    {
        var categoria = _mapper.Map<Categoria>(dto);

        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoriaResponseDto>(categoria);
    }

    public async Task<bool> UpdateAsync(int id, CategoriaUpdateDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null) return false;

        _mapper.Map(dto, categoria);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null) return false;

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<CategoriaResponseDto?> GetByIdWithBooksAsync(int id)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .Include(c => c.LibroCategorias)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
            return null;

        return new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            TotalLibros = categoria.LibroCategorias.Count
        };
    }
}
