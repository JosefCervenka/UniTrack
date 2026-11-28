using System.Security.Claims;
using System.Text;
using Application.Dtos;
using Application.Validations;
using Domain;
using Domain.Enums;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Application.Services;

public class LostItemService
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateLostItem> _createLostItemValidator;
    private readonly IValidator<UpdateLostItem> _updateLostItemValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmbeddingService _embeddingService;
    private readonly SearchLostItemValidator _searchLostItemValidator;
    private readonly SearchLostItemPictureValidator _searchLostItemPictureValidator;
    private readonly DeleteLostItemValidator _deleteLostItemValidator;

    public LostItemService(AppDbContext context, IValidator<CreateLostItem> createLostItemValidator,
        IHttpContextAccessor httpContextAccessor, IValidator<UpdateLostItem> updateLostItemValidator,
        EmbeddingService embeddingService, SearchLostItemValidator searchLostItemValidator,
        SearchLostItemPictureValidator searchLostItemPictureValidator,
        DeleteLostItemValidator deleteLostItemValidator)
    {
        _context = context;
        _createLostItemValidator = createLostItemValidator;
        _httpContextAccessor = httpContextAccessor;
        _updateLostItemValidator = updateLostItemValidator;
        _embeddingService = embeddingService;
        _searchLostItemValidator = searchLostItemValidator;
        _searchLostItemPictureValidator = searchLostItemPictureValidator;
        _deleteLostItemValidator = deleteLostItemValidator;
    }

    private async Task<Vector> Embedding(LostItem lostItem)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(lostItem.Name);
        stringBuilder.AppendLine(lostItem.Desctiption);
        stringBuilder.AppendLine(lostItem.LostLocation);
        var stringForEmbedding = stringBuilder.ToString();
        return await _embeddingService.EmbedToPgVectorAsync(stringForEmbedding);
    }

    private static ReadLostItem MapToReadLostItem(LostItem lostItem)
    {
        return new ReadLostItem
        {
            Id = lostItem.Id,
            Name = lostItem.Name,
            Description = lostItem.Desctiption,
            LostLocation = lostItem.LostLocation,
            GivenName = lostItem.Owner.GivenName,
            Surname = lostItem.Owner.Surname,
            OwnerId = lostItem.OwnerId
        };
    }

    public async Task<byte[]?> GetPictureAsync(Guid id)
    {
        await _searchLostItemPictureValidator.ValidateAndThrowAsync(new SearchLostItem()
        {
            Id = id
        });

        var image = await  _context.LostItem
            .Select(x => new { x.Id, x.Image })
            .FirstOrDefaultAsync(x => x.Id == id);

        return image?.Image;
    }

    public async Task<ReadLostItem> GetAsync(Guid id)
    {
        await _searchLostItemValidator.ValidateAndThrowAsync(new SearchLostItem()
        {
            Id = id
        });

        var lostItem = await _context.LostItem
            .Include(x => x.Owner)
            .FirstAsync(x => x.Id == id);

        return MapToReadLostItem(lostItem);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _deleteLostItemValidator.ValidateAndThrowAsync(new SearchLostItem()
        {
            Id = id
        });

        var lostItem = await _context.LostItem
            .FirstAsync(x => x.Id == id);

        _context.LostItem.Remove(lostItem);

        await _context.SaveChangesAsync();
    }


    public async Task<ReadLostItem[]> GetAllAsync(string search, int take, int skip)
    {
        var searchVector = await _embeddingService.EmbedToPgVectorAsync(search);

        var lostItems = await _context.LostItem
            .Include(x => x.Owner)
            .OrderBy(x => x.Embedding!.CosineDistance(searchVector))
            .Skip(skip).Take(take)
            .ToArrayAsync();

        return lostItems.Select(MapToReadLostItem).ToArray();
    }

    public async Task UpdateAsync(UpdateLostItem updateLostItem)
    {
        await _updateLostItemValidator.ValidateAndThrowAsync(updateLostItem);

        var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

        if (email is null)
            throw new Exception("User not found");

        var ownerId = _context.User.FirstOrDefault(X => X.Email == email)?.Id;

        if (ownerId is null)
            throw new Exception("User not found");

        var item = _context.LostItem.FirstOrDefault(X => X.Id == updateLostItem.Id);

        if (ownerId != item!.OwnerId)
            throw new Exception("User not found");

        item.Name = updateLostItem.Name;
        item.LostLocation = updateLostItem.LostLocation;
        item.Desctiption = updateLostItem.Description;
        item.StatusId = (int)StatusEnum.Lost;
        item.Embedding = await Embedding(item);

        await _context.SaveChangesAsync();
    }

    public async Task CreateAsync(CreateLostItem createLostItem)
    {
        await _createLostItemValidator.ValidateAndThrowAsync(createLostItem);

        var principal = _httpContextAccessor?.HttpContext?.User;

        var email = principal?.FindFirst(ClaimTypes.Email)?.Value;

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
            Image = createLostItem.Image,
            StatusId = (int)StatusEnum.Lost,
            OwnerId = (Guid)ownerId
        };

        lostItem.Embedding = await Embedding(lostItem);

        _context.LostItem.Add(lostItem);

        await _context.SaveChangesAsync();
    }
}