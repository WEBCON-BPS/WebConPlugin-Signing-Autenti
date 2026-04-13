using WebCon.Plugins.Signing.Autenti.Api.Models;

namespace WebCon.Plugins.Signing.Autenti.SendForSignature.Providers;

public static class OrganizationSenderProvider
{
    private const string SelfTenantId = "PARTY-TENANT:SELF";

    public static Party Create() => new()
    {
        Participant = new Participant { Id = SelfTenantId },
        Role = AutentiRoles.Sender
    };
}