using Hidayah.Domain.Models;
using Hidayah.Domain.Models.NoraniPrimer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> BGS_HA_TBL_USERS { get; set; }
        public DbSet<Institution> BGS_HA_TBL_INSTITUTION {  get; set; }
        public DbSet<InstitutionType> BGS_HA_TBL_INSTITUTION_TYPES { get; set; }
        public DbSet<Country> BGS_HA_TBL_COUNTRY { get; set; }
        public DbSet<City> BGS_HA_TBL_CITY { get; set; }
        public DbSet<BranchModel> BGS_HA_TBL_BRANCHES { get; set; }
        public DbSet<LabModel> BGS_HA_TBL_LABS { get; set; }
        public DbSet<UserLoginLog> BGS_HA_TBL_USER_LOGIN_LOGS { get; set; }
        public DbSet<IndividualLetters> BGS_HA_TBL_INDIVIDUAL_LETTERS { get; set; }
        public DbSet<CompoundLetters> BGS_HA_TBL_COMPOUND_LETTERS { get; set; }
        public DbSet<UserRoles> BGS_HA_TBL_USER_ROLES { get; set; }


    }
}
