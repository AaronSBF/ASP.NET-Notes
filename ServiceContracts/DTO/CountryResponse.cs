using System;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountryResponse methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? country { get; set; }

        //it compares the cuurent object to another object of COuntryResponse type and reutrns true, if both values are same otherwise return false
        public override bool Equals(object? obj)
        {
            if (obj == null) return false; 
            if (obj.GetType() != typeof(CountryResponse)) return false;

            CountryResponse country_to_compare = (CountryResponse)obj;

            return this.CountryID == country_to_compare.CountryID && this.country == country_to_compare.country;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class CountryExtensions
    {
        //COnverts form Country object to CountryResponse
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse() { CountryID = country.CountryID, country = country.country };
        }
    }
}
