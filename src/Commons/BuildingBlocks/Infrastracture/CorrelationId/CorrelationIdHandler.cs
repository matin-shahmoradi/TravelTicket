using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Infrastracture.CorrelationId
{
    public sealed class CorrelationIdHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = httpContextAccessor.HttpContext;

            if (context != null &&
                context.Items.TryGetValue(CorrelationConstants.HeaderName, out var correlationId))
            {
                request.Headers.TryAddWithoutValidation(
                    CorrelationConstants.HeaderName,
                    correlationId!.ToString());
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
