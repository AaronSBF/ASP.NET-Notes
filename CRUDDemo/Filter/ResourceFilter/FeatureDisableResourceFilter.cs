using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDDemo.Filter.ResourceFilter
{
    public class FeatureDisableResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisableResourceFilter> _logger;
        private readonly bool _isDisabled;

        public FeatureDisableResourceFilter(ILogger<FeatureDisableResourceFilter> logger, bool isDisabled = true)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //TO DO: before logic
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(FeatureDisableResourceFilter), nameof(OnResourceExecutionAsync));

            if (_isDisabled)
            {
                context.Result = new NotFoundResult(); //404 - Not FOund

                context.Result = new StatusCodeResult(501); //501 - Not Implemented
            }
            else
            {
                await next();
            }
            

            //TO DO: after logic
            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(FeatureDisableResourceFilter), nameof(OnResourceExecutionAsync));
        }
    }
}
