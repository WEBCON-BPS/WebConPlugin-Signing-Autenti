using System.Collections.Generic;
using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;

public interface ISignatureVisualisationProvider
{
    Constraint GetSignatureVisualisationConstraint();
}