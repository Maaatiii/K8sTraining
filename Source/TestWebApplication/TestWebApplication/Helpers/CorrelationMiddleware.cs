using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace TestWebApplication.Helpers
{
	public class CorrelationMiddleware
	{
		private readonly RequestDelegate _next;

		public CorrelationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			context.Request.Headers.TryGetValue("Correlation-Id-Header", out var correlationIds);

			var correlationId = correlationIds.FirstOrDefault() ?? Guid.NewGuid().ToString();

			CorrelationContext.SetCorrelationId(correlationId);

			using (LogContext.PushProperty("CorrelationId", correlationId))
			{
				await _next.Invoke(context);
			}
		}
	}
}
