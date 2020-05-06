using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using tacchograaph_reader.Core.Entities;
using TachoReader.Data.Data;

namespace TachographReader.Web.Migration
{
    public class SeedDummyData
    {
        private readonly ApplicationDbContext context;

        public SeedDummyData(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task CreateRolesAsync(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            string[] roleNames = {"admin", "customer", "user"};

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false);
                if (!roleExist)
                    //create the roles and seed them to the database !!
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName)).ConfigureAwait(false);
            }
        }

        public async Task CreateUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<UserApp>>();

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var user = new UserApp
                {
                    UserName = "admin",
                    Email = "admin@smartfleet.net",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "a$123456").ConfigureAwait(false);
                if (result.Succeeded)
                {
                    var currentUser = await userManager.FindByNameAsync(user.UserName).ConfigureAwait(false);
                    await userManager.AddToRoleAsync(currentUser, "admin").ConfigureAwait(false);
                }
            }
            if (!context.Customers.Any(u => u.Name == "TR company"))
            {
                var user = new Customer()
                {
                    Name = "TR company",
                    Email = "TR-company@smartfleet.net",
                    CreationDate = DateTime.Now,
                    Id = Guid.Parse("13bce473-5e31-4f3f-87a0-8863b6a814f5")

                };

                context.Customers.Add(user);
                context.SaveChanges();
            }
        }
    }
}
