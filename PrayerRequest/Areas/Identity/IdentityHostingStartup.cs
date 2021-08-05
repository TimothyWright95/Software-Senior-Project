using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrayerRequest.Areas.Identity.Data;
using PrayerRequest.Models;

[assembly: HostingStartup(typeof(PrayerRequest.Areas.Identity.IdentityHostingStartup))]
namespace PrayerRequest.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<PrayerRequestContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("PrayerRequestContextConnection")));

                services.AddIdentity<PrayerRequestUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;
                })
                .AddRoleManager<RoleManager<IdentityRole>>()

                .AddUserManager<UserManager<PrayerRequestUser>>()

                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<PrayerRequestContext>();
            });

        }
    }
}