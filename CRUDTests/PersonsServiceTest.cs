using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using Serilog.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
	public class PersonsServiceTest
	{
		//private fields
		private readonly IPersonService _personService;
		private readonly ICountryService _countryService;
		private readonly ITestOutputHelper _testOutputHelper;

		//constructor
		public PersonsServiceTest(ITestOutputHelper testOutputHelper)
		{

            _countryService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));

            var diagnosticContextMock = new Mock<IDiagnosticContext>();
			var loggerMock = new Mock<ILogger<PersonService>>();


            _personService = new PersonService( new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countryService, diagnosticContextMock.Object, loggerMock.Object);
			
			_testOutputHelper = testOutputHelper;
		}

		#region AddPerson
		//When  we supply null value as PErsonAddRequest, it should throw ArgumentNullException

		[Fact]
		public async Task AddPerson_NullPerson()
		{
			//Arrange
			PersonAddRequest? personAddRequest = null;

			
			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () => {
				//Act
				await _personService.AddPerson(personAddRequest);

			});
		}

		//When  we supply null value as PersonNAme, it should throw ArgumentException

		[Fact]
		public async Task AddPerson_PersonNameIsNull()
		{
			//Arrange
			PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null};


			//Assert
			await Assert.ThrowsAsync<ArgumentException>(async () => {
				//Act
				await _personService.AddPerson(personAddRequest);

			});
		}


		//When  we supply proper person details, it should insert the person into the persons list
		//and it should return an object personResponse, which includes with newly generated person id
		[Fact]
		public async Task AddPerson_ProperPersonDetails()
		{
			//Arrange
			PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName="Person name...", Email="person@example.com", Address="sample address", CountryID=Guid.NewGuid(), Gender= GenderOptions.Male, DateOfBirth=DateTime.Parse("2000-01-01"), ReceiveNewsLetters=true };


			
				//Act
			   PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
			List<PersonResponse> persons_list = await _personService.GetAllPersons();

			//assert
			Assert.True(person_response_from_add.PersonID != Guid.Empty);
			Assert.Contains(person_response_from_add, persons_list);
		   
		}
		#endregion

		#region GetPersonByPersonID

		//if we supply null as personID, it should return null as PersonResponse
		[Fact]
		public async Task GetPersonByPersonID_NullPersonID()
		{
			//arrange
			Guid? personID = null;

			//act
		   PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

			//Assert
			Assert.Null(person_response_from_get);
		}

		//if we supply a valid person id, it should return the valid person details as PErsonREsponse object
		[Fact]
		public async Task GetPersonByPersonID_WithPersonID()
		{
			//arrange
			CountryAddRequest country_request = new CountryAddRequest() { country = "Canada" };
			CountryResponse country_response = await _countryService.AddCountry(country_request);

			//act
			PersonAddRequest person_request = new PersonAddRequest() { PersonName = "person name..", Email = "person@example.com", Address = "address", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };
		   PersonResponse person_response_from_add = await _personService.AddPerson(person_request);
		   PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

			//assert
			Assert.Equal(person_response_from_add, person_response_from_get);
			
		}
		#endregion

		#region GetAllPersons

		//the getAllPersons() should return an empty list by default
		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			//act
		   List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

			//Assert
			Assert.Empty(persons_from_get);
		}

		//First, we  will add few person; and then when we call GetAllPersons(), it should return the same persons that were added
		[Fact]
		public async Task GetAllPersons_AddFewPersons()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { country = "USA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { country = "India" };

			CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
			CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };
			PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };
			PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
			
			foreach(PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}


			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach(PersonResponse person_response_from_add in person_response_list_from_add){
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}


			//Act
			List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_get in persons_list_from_get)
			{
				_testOutputHelper.WriteLine(person_response_from_get.ToString());
			}


			//Assert
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				Assert.Contains(person_response_from_add, persons_list_from_get);
			}
			

		
		}
		#endregion


		#region GetFilteredPersons

		//if the search text is empty and search by is "PersonName", it should return all persons
		[Fact]
		public async Task GetFilterdPErsons_EmptySearchText()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { country = "USA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { country = "India" };

			CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
			CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };
			PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };
			PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}


			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}


			//Act
			List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

			//print persons_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_get in persons_list_from_search)
			{
				_testOutputHelper.WriteLine(person_response_from_get.ToString());
			}


			//Assert
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				Assert.Contains(person_response_from_add, persons_list_from_search);
			}



		}


		//first we add few persons; and then we will search based on the person namewith some search string. It should return the matching persons 
		[Fact]
		public async Task GetFilterdPErsons_SearchByPersonName()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { country = "USA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { country = "India" };

			CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
			CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };
			PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };
			PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}


			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}


			//Act
			List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

			//print persons_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_get in persons_list_from_search)
			{
				_testOutputHelper.WriteLine(person_response_from_get.ToString());
			}


			//Assert
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				if (person_response_from_add.PersonName != null)
				{
					if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
					{
						Assert.Contains(person_response_from_add, persons_list_from_search);
					}
				}
				
				
			}



		}




		#endregion

		#region GetSortedPersons

		//When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
		[Fact]
		public async Task GetSortedPersons()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { country = "USA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { country = "India" };

			CountryResponse country_response_1 = await _countryService.AddCountry(country_request_1);
			CountryResponse country_response_2 = await _countryService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };
			PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };
			PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
			
			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}


			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}


			 List<PersonResponse> allPersons = await _personService.GetAllPersons();

			//Act
			List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons,nameof(Person.PersonName), SortOrderOptions.DESC);

			//print persons_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_get in persons_list_from_sort)
			{
				_testOutputHelper.WriteLine(person_response_from_get.ToString());
			}
			person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

			//Assert
		   for(int i = 0; i<person_response_list_from_add.Count; i++)
			{
				Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
			}



		
	}

		#endregion

		#region UpdatePerson
		//When we supply null as PersonUpdateRequest, it should throw ArgumentNullException

		[Fact]
		public async Task UpdatePerson_NullPerson()
		{
			//arrange
			PersonUpdateRequest? person_update_request = null;

			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () => 
			
			//act
			await _personService.UpdatePerson(person_update_request
			));
		}

		//if person ID is invalid it should throw argument exception
		[Fact]
		public async Task UpdatePerson_InvalidPersonID()
		{
			//arrange
			PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonID= Guid.NewGuid()};

			//Assert
			await Assert.ThrowsAsync<ArgumentException>(async () =>

			//act
			await _personService.UpdatePerson(person_update_request
			));
		}

		//when person name is null, it should throw ArgumentException
		[Fact]
		public async Task UpdatePerson_PersonNameIsNull()
		{
			//arrange
			CountryAddRequest country_add_request = new CountryAddRequest() { country = "UK" };
			CountryResponse country_response_from_add = await _countryService.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID, Email="john@example.com", Address="address", Gender =GenderOptions.Male };
			PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);


			PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
			person_update_request.PersonName = null;

			//PersonUpdateRequest person_update_request = new PersonUpdateRequest() { }

			//Assert
			await Assert.ThrowsAsync<ArgumentException>(async () =>

			//act
			await _personService.UpdatePerson(person_update_request
			));
		}

		//First, add a new person and try to update the person name and email
		[Fact]
		public async Task UpdatePerson_PersonFullDetails()
		{
			//arrange
			CountryAddRequest country_add_request = new CountryAddRequest() { country = "UK" };
			CountryResponse country_response_from_add = await _countryService.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID, Address = "ABC road", DateOfBirth= DateTime.Parse("2000-01-01"), Email ="abc@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters=true };
			PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);


			PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
			person_update_request.PersonName = "William";
			person_update_request.Email = "william@example.com";

			//PersonUpdateRequest person_update_request = new PersonUpdateRequest() { }

			//act
			PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

			PersonResponse? persons_response_from_get = await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

			//Assert
			Assert.Equal(persons_response_from_get, person_response_from_update);



		}

		#endregion

		#region DeletePerson

		//if valid PersonId is supplied it shiuld return true
		[Fact]
		public async Task DeletePerson_ValidPersonID()
		{
			//Arrange
			CountryAddRequest country_add_request = new CountryAddRequest() { country = "USA" };

			CountryResponse country_response_from_add = await _countryService.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "Jones", Address = "address", CountryID = country_response_from_add.CountryID, DateOfBirth = Convert.ToDateTime("2010-01-01"), Email = "jones@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };

			PersonResponse person_responsse_from_add = await _personService.AddPerson(person_add_request);

			//Act
			bool isDeleted =await  _personService.DeletePerson(person_responsse_from_add.PersonID);

			//assert
			Assert.True(isDeleted);

		}

        //if invalid person id is supplied it should return false
        [Fact]
        public async Task DeletePerson_invalidPersonID()
        {
            
           
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //assert
            Assert.False(isDeleted);

        }

        #endregion

    }
}
