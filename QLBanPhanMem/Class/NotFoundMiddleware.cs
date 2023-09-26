using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

    public class NotFoundMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<NotFoundMiddleware> _logger;
        private readonly LinkGenerator _linkGenerator;
        public NotFoundMiddleware(RequestDelegate next, ILogger<NotFoundMiddleware> logger, LinkGenerator linkGenerator)
            {
                _next = next;
                _logger = logger;
                _linkGenerator = linkGenerator;
            }

        public async Task InvokeAsync(HttpContext context)
        {
        await _next(context);

        if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
        {
            await _next(context);

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                _logger.LogWarning($"Resource not found: {context.Request.Path}");

                // Sử dụng View "NotFound" từ thư mục "Shared"
                var viewResult = new ViewResult
                {
                    ViewName = "/Views/Shared/Error.cshtml" // Điều này sẽ sử dụng view "NotFound.cshtml" trong thư mục "Shared"
                };

                viewResult.ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = "Resource not found"
                };
                viewResult.ViewData.Model = null; // Bạn có thể chuyển dữ liệu mô hình tùy theo nhu cầu

                await viewResult.ExecuteResultAsync(new ActionContext
                {
                    HttpContext = context,
                    RouteData = context.GetRouteData(),
                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                });
                var url = _linkGenerator.GetPathByAction(context, "Index", "Home"); // Thay thế "Index" và "Home" bằng action và controller mặc định của bạn

                context.Response.Redirect(url);
            }

        }
    }
    }

