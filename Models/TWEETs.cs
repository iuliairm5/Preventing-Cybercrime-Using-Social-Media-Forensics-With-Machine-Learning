using System.ComponentModel.DataAnnotations;

namespace DisertatieIRIMIA.Models
{
    public class TWEETs
    {
        [Required]
        //[Required(ErrorMessage = "Please enter some text")]
        public string Text { get; set; }
        [Required]
        //[Required(ErrorMessage = "Please enter social media name")]
        public string NamePerson { get; set; }
    }
}
