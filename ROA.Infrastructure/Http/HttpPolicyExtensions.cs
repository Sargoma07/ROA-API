using System.Net;
using Polly;

namespace ROA.Infrastructure.Http;

public static class HttpPolicyExtensions
{
    public static PolicyBuilder<HttpResponseMessage> CreateHandleHttpErrorBuilder()
    {
        return Policy<HttpResponseMessage>.Handle<HttpRequestException>().OrResult(HttpStatusCodePredicate());
    }

    public static Func<int, TimeSpan> CreteExponentialPolicy()
    {
        var rand = new Random();

        return retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(rand.Next(0, 100));
    }

    private static Func<HttpResponseMessage, bool> HttpStatusCodePredicate()
    {
        var httpStatusCodesWorthRetrying = new[]
        {
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.TooManyRequests,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        return x => httpStatusCodesWorthRetrying.Contains(x.StatusCode);
    }
}