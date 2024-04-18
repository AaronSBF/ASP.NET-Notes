using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class CountriesService : ICountryService
    {
        //private field
        private readonly PersonsDbContext _db;

        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
            
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddrequest parameter can't be null
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName cant be null
            if (countryAddRequest.country == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.country));
            }
            //Validation: CountryNAme cant be duplicate
            if(await _db.Countries.CountAsync(country=>country.country == countryAddRequest.country) > 0)
            {
                throw new ArgumentException("Given Country name already exist");
            }

            //COnvert object from COuntryAddRequest to Country type
           Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add Country object into _countries
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();


            return country.ToCountryResponse();


        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            //
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryid)
        {
            if (countryid == null) return null;

           Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(country => country.CountryID == countryid);

            if (country_response_from_list == null) return null;

          return country_response_from_list.ToCountryResponse();
        }
    }
}