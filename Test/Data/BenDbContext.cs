using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data
{
    public class BenDbContext : DbContext
    {
        public BenDbContext(DbContextOptions<BenDbContext> options) : base(options) { }

        public DbSet<Beneficiari> Beneficiaris { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Registration> Registrations { get; set; }
    }
}
