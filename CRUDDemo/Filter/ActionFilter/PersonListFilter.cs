using CRUDDemo.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUDDemo.Filter.ActionFilter
{
    public class PersonListFilter : IActionFilter
    {
        private readonly ILogger<PersonListFilter> _logger;

        public PersonListFilter(ILogger<PersonListFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //To do: add after logic here
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonListFilter), nameof (OnActionExecuted));

            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>) context.HttpContext.Items["arguments"];

           // personsController.ViewData["searchBy"] = parameters["searchBy"];

            if(parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = parameters["searchBy"];
                }

                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = parameters["searchString"];
                }

                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = parameters["sortBy"];
                }
                if (parameters.ContainsKey("CurrentSortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = parameters["sortOrder"];
                }
            }

            personsController.ViewBag.SearchFields = new Dictionary<string, string>() {
             { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryID), "Country" },
            { nameof(PersonResponse.Address), "Address" }
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;
            //To do: add before logic here
            //_logger.LogInformation("PersonsListActionFilter.OnActionExecuting method");

            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonListFilter), nameof(OnActionExecuting));


            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                //validate the search by parameter value
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address),
                    };
                    //reset the searchBy parameter value
                    if (searchByOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy actual value {searchBy}", context.ActionArguments["searchBy"]);
                    };
                }

                
            }
        }
    }
}
