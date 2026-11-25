using Application.Dtos;
using Domain;
using Domain.Enums;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class LostItemService
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateLostItem> _createLostItemValidator;
    private readonly IValidator<UpdateLostItem> _updateLostItemValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LostItemService(AppDbContext context, IValidator<CreateLostItem> createLostItemValidator,
        IHttpContextAccessor httpContextAccessor, IValidator<UpdateLostItem> updateLostItemValidator)
    {
        _context = context;
        _createLostItemValidator = createLostItemValidator;
        _httpContextAccessor = httpContextAccessor;
        _updateLostItemValidator = updateLostItemValidator;
    }
    
    public async Task UpdateAsync(UpdateLostItem updateLostItem)
    {
        _updateLostItemValidator.ValidateAndThrow(updateLostItem);

        var email = _httpContextAccessor!.HttpContext!.User!.Identity!.Name;

        if (email is null)
            throw new Exception("User not found");

        var ownerId = _context.User.FirstOrDefault(X => X.Email == email)?.Id;

        if (ownerId is null)
            throw new Exception("User not found");
        
        var item = _context.LostItem.FirstOrDefault(X => X.Id == updateLostItem.Id);
        
        if(ownerId != item!.OwnerId)
            throw new Exception("User not found");
        
        item.Name = updateLostItem.Name;
        item.LostLocation = updateLostItem.LostLocation;
        item.Desctiption = updateLostItem.Description;
        item.StatusId = (int)StatusEnum.Lost;
        
        await _context.SaveChangesAsync();
    }

    public async Task CreateAsync(CreateLostItem createLostItem)
    {
        _createLostItemValidator.ValidateAndThrow(createLostItem);

        var email = _httpContextAccessor!.HttpContext!.User!.Identity!.Name;

        if (email is null)
            throw new Exception("User not found");

        var ownerId = _context.User.FirstOrDefault(X => X.Email == email)?.Id;

        if (ownerId is null)
            throw new Exception("User not found");

        var lostItem = new LostItem()
        {
            Name = createLostItem.Name,
            LostLocation = createLostItem.LostLocation,
            Desctiption = createLostItem.Description,
            StatusId = (int)StatusEnum.Lost,
            OwnerId = (Guid)ownerId
        };

        _context.LostItem.Add(new LostItem());

        await _context.SaveChangesAsync();
    }
}