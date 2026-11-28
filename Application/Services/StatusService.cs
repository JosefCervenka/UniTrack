using Application.Dtos;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class StatusService
{
    private readonly AppDbContext _context;
    
    public StatusService(AppDbContext  context)
    {
        _context = context;
    }

    public async Task<ReadStatus[]> GetAllAsync()
    {
        return await _context.Status.Select(x => new ReadStatus()
        {
            DisplayName = x.DisplayName,
            Id = x.Id
        }).ToArrayAsync();
    }
}