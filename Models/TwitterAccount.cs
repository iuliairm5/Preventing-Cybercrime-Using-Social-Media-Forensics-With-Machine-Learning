using System.ComponentModel.DataAnnotations;

namespace DisertatieIRIMIA.Models
{
    public class TwitterAccount
    {
        [Required]
        public string Username { get; set; }
        
    }
}
