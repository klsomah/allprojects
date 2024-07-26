using EducationalFundingCo.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EducationalFundingCo.Data
{
    public class EducationalFundingCoContext : IdentityDbContext<IdentityUser>
    {
        public EducationalFundingCoContext(DbContextOptions<EducationalFundingCoContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<AcademyProgram> AcademyPrograms { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Communication> Communications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<EmploymentQuestionnaire> EmploymentQuestionnaires { get; set; }
        public DbSet<ConfigValue> ConfigValues { get; set; }
        public DbSet<School> School { get; set; }

        public DbSet<LearningSolution> LearningSolution { get; set; }
        public DbSet<SchoolLearningSolution> SchoolLearningSolution { get; set; }
        public DbSet<OTPVerification> OTPVerification { get; set; }
        public DbSet<USState> USState { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

    }
}

