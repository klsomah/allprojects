using System;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(EducationalFundingCo.Areas.Identity.IdentityHostingStartup))]
namespace EducationalFundingCo.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            //builder.ConfigureServices((context, services) => {
            //    services.AddDbContext<EducationalFundingCoContext>(options =>
            //        options.UseSqlServer(
            //            context.Configuration.GetConnectionString("EducationalFundingCoContextConnection")));

            //    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //        .AddEntityFrameworkStores<EducationalFundingCoContext>();
            //});
        }
    }
}