using ServiceContracts.DTO;
using ServiceContracts.Enums;
namespace ServiceContracts
{
    /// <summary>
    /// Reperesents business logic for manipulating
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Adds person into the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>Returns the same person details, along with newly generated PersonID</returns>
         Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Returns List of all persons
        /// </summary>
        /// <returns></returns>
         Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// returns the person object based on the given person id 
        /// </summary>
        /// <param name="personID">Person id to search</param>
        /// <returns>rRetrurns the matching person object</returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Returns all person objects that mathces with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search strong to search</param>
        /// <returns>Retruns all matching persons based on the given search field and search string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);


        /// <summary>
        /// Returnss sorted list persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortBy">Name of the property based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Returns sorted persons as PersonResponse list</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Updates the specified persondetails based on the given personID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update, includin person id</param>
        /// <returns>Returns the person response object after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        Task<bool> DeletePerson(Guid? pesonID);

        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns persons as Excel
        /// </summary>
        /// <returns>Returns the memory stream with Excel data of persons</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
