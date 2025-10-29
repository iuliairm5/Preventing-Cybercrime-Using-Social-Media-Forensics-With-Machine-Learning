using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DisertatieIRIMIA.Models
{
    public class MonitorizedUser
    {
        [Key]
        public int MonitorizationID { get; set; }
        public string Username { get; set; }
        
        public DateTime DateTime { get; set; }

        public string? AccountDetails { get; set; }
        public string? AccountLatestTweets { get; set; }
        public string? Location  { get; set; }

        //NEW
        //public string? WebSites { get; set; } // List to store multiple JSONs
        //public int? MaximumWebsites { get; set; }
        //

        [ForeignKey("UserId")]
        public string UserId { get; set; } //the foreign key
        //public ApplicationUser User { get; set; }
        //public virtual IdentityUser? User { get; set; }

        
    }
}
