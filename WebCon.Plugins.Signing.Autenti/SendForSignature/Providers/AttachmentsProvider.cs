using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents;
using WebCon.WorkFlow.SDK.Documents.Model.Attachments;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;

public class AttachmentsProvider(AttachmentsConfig config, ActionContextInfo context) : IAttachmentsProvider
{
    private readonly Regex _fileNameRegex = string.IsNullOrEmpty(config.AttRegularExpression)
        ? null
        : new Regex(config.AttRegularExpression, RegexOptions.Compiled);

    public async Task<List<FileData>> GetAttachmentsAsync()
    {
        context.PluginLogger.AppendDebug($"Selecting attachments using mode: {config.AttachmentsChoosingOption}");

        var result = config.AttachmentsChoosingOption switch
        {
            AttachmentsChoosingOptions.Category => await GetByCategoryAsync(),
            AttachmentsChoosingOptions.SQL => await GetBySqlAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(config.AttachmentsChoosingOption), 
                    config.AttachmentsChoosingOption, "Unsupported attachment selection mode.")
        };

        ValidateFileExtensions(result);
        ValidateTotalFileSize(result);

        context.PluginLogger.AppendDebug($"Found {result.Count} attachment(s): {string.Join(", ", result.Select(f => f.Name))}");
        return result;
    }

    private async Task<List<FileData>> GetByCategoryAsync()
    {
        context.PluginLogger.AppendDebug($"Category filter: {config.CategorySelectionOptions}, GroupID: {config.GroupID}, Regex: {config.AttRegularExpression ?? "(none)"}");

        var matchingAttachments = context.CurrentDocument.Attachments
            .Where(att => MatchFileGroup(att.FileGroup))
            .Where(att => MatchRegex(att.FileName))
            .ToList();

        if (matchingAttachments.Count == 0)
            throw new InvalidOperationException("No attachments found matching the configured category and file name filter.");

        var results = new List<FileData>(matchingAttachments.Count);
        foreach (var attachment in matchingAttachments)
            results.Add(new FileData
            {
                Name = attachment.FileName,
                Content = await attachment.GetContentAsync()
            });

        return results;
    }

    private bool MatchFileGroup(AttachmentsGroup fileGroup) => config.CategorySelectionOptions switch
    {
        CategorySelectionOptions.ID => fileGroup?.ID.ToString() == config.GroupID,
        CategorySelectionOptions.All => true,
        CategorySelectionOptions.None => fileGroup == null,
        _ => false,
    };

    private bool MatchRegex(string fileName)
        => _fileNameRegex is null || _fileNameRegex.IsMatch(fileName);

    private async Task<List<FileData>> GetBySqlAsync()
    {
        context.PluginLogger.AppendDebug("Executing SQL query for attachment selection");

        var dt = await new SqlExecutionHelper(context).GetDataTableForSqlCommandAsync(config.AttQuery);
        if (dt.Rows.Count == 0)
            throw new InvalidOperationException("The SQL query for attachment selection returned empty attachments list. Please attach at least one file.");

        var manager = new DocumentAttachmentsManager(context);
        var results = new List<FileData>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
        {
            var attachment = await manager.GetAttachmentAsync((int)row[0]);
            results.Add(new FileData
            {
                Name = attachment.FileName,
                Content = await attachment.GetContentAsync()
            });
        }

        return results;
    }

    private static void ValidateFileExtensions(List<FileData> files)
    {
        var unsupportedFiles = files
            .Where(f => !f.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Name)
            .ToList();

        if (unsupportedFiles.Count > 0)
            throw new InvalidOperationException(
                $"Only PDF files are supported for signing. The following attachments have unsupported extensions: {string.Join(", ", unsupportedFiles)}.");
    }

    private static void ValidateTotalFileSize(List<FileData> files)
    {
        const long maxTotalSizeBytes = 20 * 1024 * 1024; // 20 MB

        var totalSize = files.Sum(f => (long)f.Content.Length);

        if (totalSize > maxTotalSizeBytes)
            throw new InvalidOperationException(
                $"Total attachment size ({totalSize / (1024.0 * 1024.0):F2} MB) exceeds the Autenti API limit of 20 MB. " +
                $"Files: {string.Join(", ", files.Select(f => $"{f.Name} ({f.Content.Length / (1024.0 * 1024.0):F2} MB)"))}.");
    }
}