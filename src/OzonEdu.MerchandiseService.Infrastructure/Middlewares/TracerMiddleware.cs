using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTracing;

namespace OzonEdu.MerchandiseService.Infrastructure.Middlewares
{
    public class TracerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITracer _tracer;

        public TracerMiddleware(RequestDelegate next, ITracer tracer)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var span = _tracer.BuildSpan(nameof(TracerMiddleware)).StartActive();

            await _next(context);
        }
    }
}