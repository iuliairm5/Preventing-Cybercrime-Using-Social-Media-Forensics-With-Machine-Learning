using System.ComponentModel.DataAnnotations;

namespace DisertatieIRIMIA.Models
{
    public class URLs
    {
        [Required]
        public string Link { get; set; }
        [Required]
        public string NamePerson { get; set; }  
    }
}
