using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represent business logic for manipulating country entity
    /// </summary>
    public interface ICountryService
    {
        /// <summary>
        /// Adds country object into list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add(including newly genarated country id)</param>
        /// <returns>Returns the country object after adding it </returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All countries fromm the list as List of countryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Retruns country object based on given country id
        /// </summary>
        /// <param name="countryid">CountryID to search</param>
        /// <returns>Matching country as CountryResponse</returns>
        Task<CountryResponse?> GetCountryByCountryId(Guid? countryid);
    }
}