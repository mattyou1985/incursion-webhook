using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace IncursionWebhook.Services.Discord.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class DevelopmentOnlyAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            var env = context.HttpContext.RequestServices.GetService<IHostingEnvironment>();
            if (!env.IsDevelopment())
            {
                context.Result = new NotFoundResult();
            }
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
        }
    }
}
