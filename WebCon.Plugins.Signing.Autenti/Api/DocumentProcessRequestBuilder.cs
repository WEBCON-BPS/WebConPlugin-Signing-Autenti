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
    private readonly PluginLogger _logger;

    private string _title;
    private string _description;
    private string _processLanguage;

    private readonly List<Func<Task>> _asyncSteps = [];

    private List<Party> _parties = [];
    private List<FileData> _files = [];
    private List<Constraint> _constraints = [];

    public DocumentProcessRequestBuilder(
        IPartiesProvider participantsProvider,
        IAttachmentsProvider attachmentsProvider,
        PluginLogger logger)
    {
        _partiesProvider = participantsProvider;
        _attachmentsProvider = attachmentsProvider;
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

    public IDocumentProcessRequestBuilder WithSignatureVisualisation(DocumentDetailsConfig config)
    {
        _logger?.AppendDebug($"Builder with signature visualisation: {config.SignatureVisualRepresentation}");

        var visualisationId = config.SignatureVisualRepresentation switch
        {
            SignatureRepresentation.Signature_in_any_place_chosen_by_the_recipient => AutentiVisualisations.Manual,
            _ => AutentiVisualisations.AutentiSignatureCard
        };

        _constraints.Add(new Constraint
        {
            ConstrainedActions = [AutentiActions.SignatureApplication],
            Classifiers = [AutentiConstraints.Visualisation],
            Attributes = new ConstraintAttributes
            {
                VisualisationId = visualisationId
            }
        });

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
                Tags = [] ,
                Constraints = _constraints
            },
            Files = _files
        };            
    }
}