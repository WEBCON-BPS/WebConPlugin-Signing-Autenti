using System.Collections.Generic;
using System.Linq;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Tools.Other;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;

public class TagsProvider( DocumentDetailsConfig config, ActionContextInfo context) : ITagsProvider
{
    public List<Tag> GetTags()
    {
        var tags = new List<Tag>();

        foreach (var tagId in config.TagsId?.Split(';'))
            if (!string.IsNullOrEmpty(tagId))
                AddTagIfNotExists(tags, tagId.Trim());

        if (!config.TagsIdFieldId.HasValue)
            return tags;

        var tagsPicker = context.CurrentDocument.Fields.GetByID(config.TagsIdFieldId.Value).GetValue()?.ToString();

        if(string.IsNullOrEmpty(tagsPicker))
            return tags;


        foreach (var tag in tagsPicker?.Split(';').Where(x => !string.IsNullOrEmpty(x)))
        {
            var tagId = TextHelper.GetPairId(tag.Trim()).Trim();

            if (string.IsNullOrEmpty(tagId))
                continue;

            AddTagIfNotExists(tags, tagId);
        }

        context.PluginLogger.AppendDebug($"Built {tags.Count} tag/s: {string.Join(", ", tags.Select(t => t.Id))}");
        return tags;
    }

    private void AddTagIfNotExists(List<Tag> tags, string tagId)
    {
        if (!tags.Any(t => t.Id == tagId))
            tags.Add(new Tag { Id = tagId });
    }
}