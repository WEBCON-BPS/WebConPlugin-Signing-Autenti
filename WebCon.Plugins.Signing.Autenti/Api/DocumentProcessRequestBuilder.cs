using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.Plugins.Signing.Autenti.SendForSignature;
using WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.Exceptions;

namespace WebCon.Plugins.Signing.Autenti.Api;

public class DocumentProcessRequestBuilder : IDocumentProcessRequestBuilder
{
    private readonly IPartiesProvider _partiesProvider;
    private readonly IAttachmentsProvider _attachmentsProvider;
    private readonly ITagsProvider _tagsProvider;
    private readonly ISignatureVisualisationProvider _signatureVisualisationProvider;
    private readonly PluginLogger _logger;

    private string _title;
    private string _description;
    private string _processLanguage;

    private readonly List<Func<Task>> _asyncSteps = [];

    private List<Party> _parties = [];
    private List<FileData> _files = [];
    private List<Constraint> _constraints = [];
    private List<Tag> _tags = [];

    public DocumentProcessRequestBuilder(
        IPartiesProvider participantsProvider,
        IAttachmentsProvider attachmentsProvider,
        ITagsProvider tagsProvider,
        ISignatureVisualisationProvider signatureVisualisationProvider,
        PluginLogger logger)
    {
        _partiesProvider = participantsProvider;
        _attachmentsProvider = attachmentsProvider;
        _tagsProvider = tagsProvider;
        _signatureVisualisationProvider = signatureVisualisationProvider;
        _logger = logger;
    }

    public IDocumentProcessRequestBuilder WithDocumentDetails(DocumentDetailsConfig config)
    {
        _logger?.AppendDebug("Builder with document details");

        if (string.IsNullOrEmpty(config.DocumentName))
            throw new SDKArgumentException("Document name must be provided.", nameof(config.DocumentName));

        _title = config.DocumentName;
        _description = config.MessageToReceipients;
        _processLanguage = config.ProcessLanguage;
        return this;
    }

    public IDocumentProcessRequestBuilder WithOrganizationSender(DocumentDetailsConfig config)
    {
        _logger?.AppendDebug($"Builder with organization sender: {config.SendAsOrganization}");

        if (!config.SendAsOrganization)
            return this;

        _asyncSteps.Add(async () =>
        {
            _parties.Insert(0, OrganizationSenderProvider.Create());
        });
        return this;
    }

    public IDocumentProcessRequestBuilder WithTags()
    {
        _logger?.AppendDebug($"Builder with tags");
        _tags = _tagsProvider.GetTags();
        return this;
    }

    public IDocumentProcessRequestBuilder WithSignatureVisualisation()
    {
        _logger?.AppendDebug($"Builder with signature visualisation");
        _constraints.Add(_signatureVisualisationProvider.GetSignatureVisualisationConstraint());
        return this;
    }

    public IDocumentProcessRequestBuilder WithParticipantsAsync()
    {
        _logger?.AppendDebug("Builder with participants");

        _asyncSteps.Add(async () =>
        {
            _parties.AddRange(await _partiesProvider.GetPartiesAsync());
        });
        return this;
    }

    public IDocumentProcessRequestBuilder WithAttachmentsAsync()
    {
        _logger?.AppendDebug("Builder with attachmments");

        _asyncSteps.Add(async () =>
        {
            _files = await _attachmentsProvider.GetAttachmentsAsync();
        });
        return this;
    }

    public async Task<RequestDto> BuildAsync()
    {
        foreach (var step in _asyncSteps)
            await step();

        return new RequestDto()
        {
            DocumentRequest = new DocumentProcessRequest
            {
                Title = _title,
                Description = _description,
                ProcessLanguage = _processLanguage,
                Parties = _parties,
                Constraints = _constraints,
                Tags = _tags
            },
            Files = _files
        };            
    }
}