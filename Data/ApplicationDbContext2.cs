
using DisertatieIRIMIA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DisertatieIRIMIA.Data
{
	//public class ApplicationDbContext : DbContext
	public class ApplicationDbContext2 : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext2(DbContextOptions<ApplicationDbContext2> options) : base(options) { } //constructor
		
		
		public DbSet<MonitorizedUser> MonitorizedUsers { get; set; }
	}
}
