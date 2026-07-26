using BuildingBlocks.Extensions;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.api.Features.GetPaymentById;
using Payment.api.Features.VerifyPayment;

namespace Payment.api.Features.CallBack
{
    public sealed class PaymentEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/payments/orders/{orderId}/pay-url", async (
               HttpContext context,
               ISender sender,
               [FromRoute] Guid orderId) =>
            {
                var query = new GetPaymentUrlQuery(orderId);
                var result = await sender.Send(query);

                if (result.Value!.Status == 100 || result.Value.PaymentUrl is not null)
                    return Results.Redirect(result.Value.PaymentUrl!);

                return result.ToHttpResult(context);
            });
            app.MapGet("api/payments/callback", async (
                HttpContext context,
                ISender sender,
                [FromQuery] Guid paymentId,
                [FromQuery] string authority,
                [FromQuery] string status) =>
            {
                var result = await sender.Send(
                    new VerifyPaymentCommand(paymentId, authority, status));

                if (!result.IsSuccess)
                    return result.ToHttpResult(context);

                return Results.Ok(result);
            });
        }
    }
}
