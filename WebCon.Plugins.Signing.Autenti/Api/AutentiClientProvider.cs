using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.WorkFlow.SDK.Tools.Data;
using WebCon.WorkFlow.SDK.Tools.Data.Model;

namespace WebCon.Plugins.Signing.Autenti.Api;

public class AutentiClientProvider(ConnectionsHelper connectionsHelper, AuthorizationConfig config)
{
    public async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var autentiService = connectionsHelper.GetConnectionToWebService(new GetByConnectionParams(config.ConnectionId));
        var pem = connectionsHelper.GetConnectionToWebService(new GetByConnectionParams(config.PemConnectionId)).ClientSecret;

        var assertion = CreateJwtAssertion(pem);
        var accessToken = await ExchangeAssertionForTokenAsync(autentiService, assertion);

        var httpClient = new HttpClient(GetProxyHandler(autentiService.Url));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpClient.BaseAddress = new Uri(autentiService.Url);

        return httpClient;
    }

    private string CreateJwtAssertion(string pem)
    {
        using var rsa = ImportPrivateKeyFromPem(pem);
        var securityKey = new RsaSecurityKey(rsa.ExportParameters(true))
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var now = DateTimeOffset.UtcNow;

        var payload = new JwtPayload
        {
            { "aud", "autenti.com" },
            { "exp", now.AddSeconds(300).ToUnixTimeSeconds() },
            { "iat", now.ToUnixTimeSeconds() },
            { "iss", config.Issuer },
            { "sub", config.Subject },
            { "jti", Guid.NewGuid().ToString() },
            { "email", config.UserEmail },
            { "email_verified", true },
        };

        var header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256))
        {
            ["kid"] = "autenti-key"
        };

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> ExchangeAssertionForTokenAsync(WebServiceConnection autentiService, string assertion)
    {
        var requestBody = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = autentiService.ClientID,
            ["client_secret"] = autentiService.ClientSecret,
            ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
            ["assertion"] = assertion
        });

        using var tokenHttpClient = new HttpClient(GetProxyHandler(autentiService.AuthorizationServiceUrl));
        var response = await tokenHttpClient.PostAsync(autentiService.AuthorizationServiceUrl, requestBody);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);

        return tokenResponse?.AccessToken
            ?? throw new InvalidOperationException("Token response did not contain an access_token.");
    }

    private HttpClientHandler GetProxyHandler(string url)
        => !config.UseProxy ? new HttpClientHandler() : new HttpClientHandler() { Proxy = connectionsHelper.GetProxy(url) };

    private RSA ImportPrivateKeyFromPem(string pem)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(pem.ToCharArray());
        return rsa;
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }
    }
}