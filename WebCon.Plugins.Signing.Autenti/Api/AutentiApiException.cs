using System;
using System.Net;

namespace WebCon.Plugins.Signing.Autenti.Api;

public class AutentiApiException(HttpStatusCode statusCode, string responseBody)
    : Exception($"Autenti API returned {(int)statusCode} ({statusCode}). Response: {responseBody}")
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string ResponseBody { get; } = responseBody;
}