using System.ComponentModel.DataAnnotations;

namespace DisertatieIRIMIA.Models
{
    public class MEMEs
    {
        [Required]
        public IFormFile Meme { get; set; }
        [Required]
        public string NamePerson { get; set; }
    }
}
