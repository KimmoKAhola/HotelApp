using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.DatabaseSeeding
{
    public class UserSeeding(HotelContext dbContext) : IDataSeeding
    {
        private readonly int _numberOfSeededUsers = 50;

        public HotelContext DbContext { get; set; } = dbContext;
        public void SeedData()
        {
            using (var ctx = new HotelContext())
            {
                var faker = new Faker();
                if (!ctx.UserRole.Any())
                {
                    var systemRole = new UserRole
                    {
                        RoleName = UserRoles.System.ToString(),
                    };
                    var adminRole = new UserRole
                    {
                        RoleName = UserRoles.Admin.ToString(),
                    };
                    var guestRole = new UserRole
                    {
                        RoleName = UserRoles.Guest.ToString(),
                    };
                    ctx.Add(systemRole);
                    ctx.Add(adminRole);
                    ctx.Add(guestRole);
                    ctx.SaveChanges();
                }
                if (!ctx.User.Any(u => u.UserRole.RoleName == UserRoles.System.ToString()))
                {
                    var systemRole = ctx.UserRole.Where(ur => ur.RoleName == UserRoles.System.ToString()).First();
                    var system = new User
                    {
                        FirstName = "System",
                        LastName = "System",
                        UserName = "System",
                        Password = "System",
                        Email = "System@TheSuiteSpot.net",
                        IsAdmin = true,
                        UserRole = systemRole,
                        UserInbox = new UserInbox
                        {

                        }
                    };
                    ctx.Add(system);
                    ctx.SaveChanges();
                    var adminRole = ctx.UserRole.Where(ur => ur.RoleName == UserRoles.Admin.ToString()).First();
                    var admin = new User
                    {
                        FirstName = "Kimmo",
                        LastName = "Ahola",
                        UserName = "admin",
                        Email = "admin@TheSuiteSpot.net",
                        Password = "admin",
                        IsAdmin = true,
                        UserRole = adminRole,
                        UserInbox = new UserInbox
                        {

                        }
                    };
                    ctx.Add(admin);
                    ctx.SaveChanges();
                    CurrentUser.SetCurrentUser(admin);
                }


                if (ctx.User.Count() < _numberOfSeededUsers)
                {
                    var specialGuest = new User
                    {
                        FirstName = "Richard",
                        LastName = "Chalk",
                        UserName = "Richard",
                        Email = "RichChalk@example.com",
                        Password = "123456",
                        UserRole = ctx.UserRole.Where(ur => ur.RoleName == UserRoles.Guest.ToString()).First(),
                        UserInbox = new UserInbox { }
                    };
                    ctx.Add(specialGuest);
                    ctx.SaveChanges();
                    for (int i = 0; i < _numberOfSeededUsers - 1; i++)
                    {
                        var userAsGuest = ctx.UserRole.Where(ur => ur.RoleName == UserRoles.Guest.ToString()).First();
                        var firstName = faker.Name.FirstName();
                        var lastName = faker.Name.LastName();
                        var guest = new User
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            UserName = faker.Internet.UserName(firstName, lastName),
                            Email = faker.Internet.Email(firstName, lastName, "example.com", faker.UniqueIndex.ToString()),
                            Password = faker.Internet.Password(6),
                            UserRole = userAsGuest,
                            UserInbox = new UserInbox { }
                        };
                        ctx.Add(guest);
                        ctx.SaveChanges();
                        SystemMessage.SendCreatedUserMessage(guest, ctx);
                    }
                    ctx.SaveChanges();
                }
            }
            CurrentUser.SetCurrentUser(DbContext.User.Where(u => u.IsAdmin && u.UserName == "admin").First());
        }
    }
}