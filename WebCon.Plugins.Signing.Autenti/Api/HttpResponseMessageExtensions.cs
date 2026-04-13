using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebCon.WorkFlow.SDK.Common;

namespace WebCon.Plugins.Signing.Autenti.Api;

internal static class HttpResponseMessageExtensions
{
    private const string ChallengeHeaderName = "X-Challenge";
    private const string LockClassifier = "CHALLENGE_CLASSIFIER-UNIQUE_TYPE:DOCUMENT_ACCESS_UNLOCK";

    public static async Task EnsureSuccessOrThrowWithBodyAsync(this HttpResponseMessage response, PluginLogger logger)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync();

        var challengeInfo = TryDecodeChallengeHeader(response);
        if (challengeInfo is not null)
        {
            if(challengeInfo.Contains(LockClassifier))
            {
                logger.AppendInfo($"Document locked: {challengeInfo}");
                throw new DocumentLockException();
            }

            throw new AutentiApiException(response.StatusCode,
                $"{body}{Environment.NewLine}X-Challenge (decoded): {challengeInfo}");
        }

        throw new AutentiApiException(response.StatusCode, body);
    }

    internal static string TryDecodeChallengeHeader(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.Forbidden)
            return null;

        if (!response.Headers.TryGetValues(ChallengeHeaderName, out var values))
            return null;

        var headerValue = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(headerValue))
            return null;

        try
        {
            var bytes = DecodeBase64(headerValue);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException)
        {
            return $"[unable to decode Base64] {headerValue}";
        }
    }

    private static byte[] DecodeBase64(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }
}