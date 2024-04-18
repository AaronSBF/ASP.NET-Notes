using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountryService _countryService;

        //constructor
        public CountriesServiceTest()
        {
            _countryService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }


        #region AddCountry
        //When COuntryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
               await _countryService.AddCountry(request);
            });

           
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //arrange
            CountryAddRequest? request = new CountryAddRequest() { country =null};

            //Assert
           await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
               await _countryService.AddCountry(request);
            });


        }


        //When countryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //arrange
            CountryAddRequest? request1 = new CountryAddRequest() { country = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { country = "USA" };

            //Assert
           await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
               await _countryService.AddCountry(request1);
               await _countryService.AddCountry(request2);
            });


        }


        //When you supple proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //arrange
            CountryAddRequest? request = new CountryAddRequest() { country ="Japan"};
 
            //Act
            CountryResponse response = await _countryService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCountries = await _countryService.GetAllCountries();


            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);

        }
        #endregion

        #region GetAllCountries

        [Fact]
        //list of countries should be empty by default
        public async Task GetAllCountries_EmptyList()
        {
            //Arrange not there

            //Act
            List<CountryResponse> actual_country_response_list = await _countryService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest(){country="USA"},
                new CountryAddRequest(){country="UK"},
            };

            //act
            List<CountryResponse> countries_list_from_add_new_country = new List<CountryResponse>();
            foreach(CountryAddRequest country_request in country_request_list)
            {
                countries_list_from_add_new_country.Add(await _countryService.AddCountry(country_request));
            }

            List<CountryResponse> actualCountryResponseList = await _countryService.GetAllCountries();

            //read each element from countries_list_from_add_new_country
            //Assert
            foreach(CountryResponse expected_country in countries_list_from_add_new_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }

        }

        #endregion


        #region GetCountryByCountryID

        [Fact]
        //If we supply null as CountryID, it sould return null as CountryResponse Object
        public async Task GetCountryByCountryID_Null()
        {
            //Arrange
            Guid countryID = Guid.Empty;

            //Act
         CountryResponse? country_response_from_get_method = await _countryService.GetCountryByCountryId(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);
        }

        [Fact]
        //if we supply a valid country id, it should return thr mathing country details as CountryResponse object
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //arrange
            CountryAddRequest? country_add_request = new CountryAddRequest() { country ="China"};
            //Guid? countryID = new Guid();
           CountryResponse country_response_from_add_request = await _countryService.AddCountry(country_add_request);

            //act
           CountryResponse? country_response_from_get = await _countryService.GetCountryByCountryId(country_response_from_add_request.CountryID);

            //assert
            Assert.Equal(country_response_from_add_request, country_response_from_get);
        }


        #endregion
    }

}
