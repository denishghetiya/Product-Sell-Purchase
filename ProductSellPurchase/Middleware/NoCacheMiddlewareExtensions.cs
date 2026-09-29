using Microsoft.AspNetCore.Builder;

namespace ProductSellPurchase.Middleware
{
    public static class NoCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseNoCache(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NoCacheMiddleware>();
        }
    }
}
