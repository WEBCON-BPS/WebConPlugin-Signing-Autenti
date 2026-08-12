using System;
using WebCon.Plugins.Signing.Autenti.Api.Models;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;

public class SignatureVisualisationProvider(DocumentDetailsConfig config, ActionContextInfo context) : ISignatureVisualisationProvider
{
    public Constraint GetSignatureVisualisationConstraint()
    {
        return new Constraint()
        {
            ConstrainedActions = [AutentiActions.SignatureApplication],
            Classifiers = [AutentiConstraints.Visualisation],
            Attributes = new ConstraintAttributes
            {
                VisualisationId = GetVisualisationId(config, context)
            }
        };
    }

    private string GetVisualisationId(DocumentDetailsConfig config, ActionContextInfo context)
    {
        var signatureVisualRepresentation = config.SignatureVisualRepresentation != SignatureRepresentation.Configuration_from_field
            ? config.SignatureVisualRepresentation
            : GetSignatureRepresentationFromField(config, context);

        return signatureVisualRepresentation switch
        {
            SignatureRepresentation.Signature_in_any_place_chosen_by_the_recipient => AutentiVisualisations.Manual,
            _ => AutentiVisualisations.AutentiSignatureCard
        };
    }

    private SignatureRepresentation GetSignatureRepresentationFromField(DocumentDetailsConfig config, ActionContextInfo context)
    {
        if(!config.SignatureVisualizationFieldId.HasValue)
            throw new InvalidOperationException("Signature visualization field ID is not set.");

        var id = context.CurrentDocument.ChooseFields.GetByID(config.SignatureVisualizationFieldId.Value).Value.ID;

        if (!int.TryParse(id, out var intValue) || !Enum.IsDefined(typeof(SignatureRepresentation), intValue))
            throw new InvalidOperationException($"'{id}' is not a valid Signature Visualization value. Use custom data source with Signature Visualisations provided in package.");

        return (SignatureRepresentation)intValue;   
    }
}