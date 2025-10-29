using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisertatieIRIMIA.Models
{
    public class PREDICTION
    {
        public int PREDICTIONID { get; set; }
        public string NamePersonSocialMedia { get; set; }
        public string InputType { get; set; }
        public string Prediction { get; set; }
        public DateTime DateTime { get; set; }


        public string? Input { get; set; }
        public byte[]? InputImage { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; } //the foreign key
        //public ApplicationUser User { get; set; }
        //public virtual IdentityUser? User { get; set; }

    }
}
