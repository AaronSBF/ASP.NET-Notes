using System;
using System.Collections.Generic;
using Entities;



namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class for adding a new country
    /// </summary>
    public class CountryAddRequest
    {
        public string? country { get; set; }

        //convert to country class
        public Country ToCountry()
        {
            return new Country() { country = country };
        }
    }
}
