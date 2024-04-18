using CRUDDemo.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDDemo.Filter.ActionFilter
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly IPersonService _personService; 
        private readonly ICountryService _countryService; 

        PersonCreateAndEditPostActionFilter(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //TO DO: Before logic
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countryService.GetAllCountries();

                    personsController.ViewBag.Countries = countries;
                    personsController.ViewBag.Error = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personRequest"];

                    context.Result = personsController.View(personRequest); //short-ircuit or skips the subseuent acction filters and action methods
                }
                else
                {
                    await next(); //involes the subsequent filter and action method
                }
            }
            else
            {
                await next();
            }
            
            
           // await next();
            //TO DO; After logic

        }
    }
}
