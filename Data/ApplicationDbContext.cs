
using DisertatieIRIMIA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DisertatieIRIMIA.Data
{
	//public class ApplicationDbContext : DbContext
	public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { } //constructor
		public DbSet<PREDICTION> Predictions { get; set; } //a table that will corespond with the Prediction Model

		/////////////////////////////////
		
	}
}
