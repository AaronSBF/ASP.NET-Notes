using CRUDDemo.Filter.ActionFilter;
using CRUDDemo.Filter.AuthroriztionFilter;
using CRUDDemo.Filter.ResourceFilter;
using CRUDDemo.Filter.ResultsFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDDemo.Controllers
{
    [Route("[controller]")]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "Custom-KeyFromController", "Custom-ValueFromController" , 3}, Order = 3)]
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICountryService _countryService;
        private readonly ILogger<PersonsController> _logger;

        //contstructor
        public PersonsController(IPersonService personService, ICountryService countryService, ILogger<PersonsController> logger)
        {
            _personService = personService;
            _countryService = countryService;
            _logger = logger;
        }

        [Route("[action]")] 
        [Route("/")]
        [TypeFilter(typeof(PersonListFilter), Order = 4)]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] {"Custom-Key", "Custom-Value", 1}, Order = 1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");

            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder:{sortOrder}");
            //Search
           /* ViewBag.SearchFields = new Dictionary<string, string>() {
             { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryID), "Country" },
            { nameof(PersonResponse.Address), "Address" }
            };
           */
         // List<PersonResponse> persons = _personService.GetAllPersons();

            List<PersonResponse> persons = await _personService.GetFilteredPersons(searchBy, searchString);
           // ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;

            //Sort
           List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
            //ViewBag.CurrentSortBy = sortBy;
            //ViewBag.CurrentSortOrder = sortOrder.ToString();
            
            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        //Executes when the user clicks on "Create Person" hyperLink (while opening view)
        [Route("[action]")]
        [HttpGet]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "my-key", "my-value", 4 })]
        public async Task<IActionResult> Create()
        {
          List<CountryResponse> countries =  await _countryService.GetAllCountries();

            ViewBag.Countries = countries;
            return View();
        }

        [HttpPost]
        //Url: persons/create
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisableResourceFilter), Arguments = new object[] {false})]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
          /*  if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countryService.GetAllCountries();

                ViewBag.Countries = countries;
               ViewBag.Error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View();
            }*/
            //call the servcie method
           PersonResponse personResponse = await _personService.AddPerson(personRequest);


            //navigate to Index() action
            return RedirectToAction("Index", "Persons");
            
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personService.GetPersonByPersonID(personRequest.PersonID);

            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            
            
                PersonResponse updatedPerson = await _personService.UpdatePerson(personRequest);
                return RedirectToAction("Index");
            
            /*else
            {
                List<CountryResponse> countries = await _countryService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.country, Value = temp.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }*/
        }

        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonPDF()
        {
            //get all persons list
            List<PersonResponse> people = await _personService.GetAllPersons();

            //Return view as PDF
            return new ViewAsPdf("PersonsPDF", people, ViewData) { PageMargins = new Margins() { Top=20, Right=20, Bottom=20, Left=20}, PageOrientation= Orientation.Landscape };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
