using System.Text.Json;
using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.Enums;
using CollectionGallery.InfraStructure.Data.Repository;
using CollectionGallery.InfraStructure.Data.Tag.Models;
using CollectionGallery.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CollectionGallery.InfraStructure.Data.Services;

public class TagService
{
    private readonly ILogger<TagService> _logger;
    private readonly TagRepository _tagRepository;

    public TagService(ILogger<TagService> logger, TagRepository tagRepository)
    {
        _logger = logger;
        _tagRepository = tagRepository;
    }

    /// <summary>
    /// Inserts single tag or multiple tags in DB at once by splitting the tag string with <c>,</c>
    /// </summary>
    /// <param name="tagName">Single tag or comma based multiple tags as single string.</param>
    /// <returns><c>true</c> if tag(s) successfully inserted. <c>false</c> if no tag(s) are inserted due to already exists.</returns>
    /// <exception cref="InvalidPayloadException">If <paramref name="tagName"/> is null or just whitespace</exception>
    public async Task<bool> InsertAsync(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            throw new InvalidPayloadException($"Tag name ({tagName}) cannot be null or whitespace.");
        }

        List<string> cleanedTagNames = tagName
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(t => t.ToLower())
            .ToList();

        if (cleanedTagNames.Count == 0)
        {
            return false;
        }

        await _tagRepository.InsertTagsAsync(cleanedTagNames);

        return true;
    }
}