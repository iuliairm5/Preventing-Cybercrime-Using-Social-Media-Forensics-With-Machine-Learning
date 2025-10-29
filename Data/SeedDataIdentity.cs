using Microsoft.AspNetCore.Identity;

namespace DisertatieIRIMIA.Data
{
    public class SeedDataIdentity
    {
        //private const string adminphone = "0123456789";
        //private const string adminname = "ADMIN";
        private const string adminEmail = "admin1@test.com";
        private const string adminPassword = "Secret123$";
        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices
            .CreateScope().ServiceProvider;

            using (var userManager = serviceProvider
                .GetRequiredService<UserManager<IdentityUser>>())
            {
                IdentityUser user = await userManager.FindByEmailAsync(adminEmail);

                if (user == null)
                {
                    //user = new IdentityUser { UserName = adminname, Email = adminEmail, PhoneNumber = adminphone };
                    user = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                    await userManager.CreateAsync(user, adminPassword);
                }
            }
        }
    }
}
