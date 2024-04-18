using System.ComponentModel.DataAnnotations;


namespace Entities
{
    //Domain Model

    /// <summary>
    /// Domain Model for Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryID { get; set; }

        public string?  country { get; set; }

        //navigtion property
        public virtual ICollection<Person>? People { get; set; }


    }
}