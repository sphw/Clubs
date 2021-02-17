using System;
using System.Collections.Generic;
using System.Text;
using Clubs.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Clubs.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trip>();

            modelBuilder.Entity<TripUser>()
    .HasKey(t => new { t.TripId, t.UserId });

            modelBuilder.Entity<TripUser>()
                .HasOne(tu => tu.Trip)
                .WithMany(t => t.TripUsers)
                .HasForeignKey(tu => tu.TripId);

            modelBuilder.Entity<TripUser>()
                .HasOne(tu => tu.User)
                .WithMany(t => t.TripUsers)
                .HasForeignKey(tu => tu.UserId);
            
            modelBuilder.Entity<ApplicationUser>().Property(nameof(ApplicationUser.Qualifications)).HasConversion(new ValueConverter<IList<string>, string>(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<IList<string>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })));
            
            modelBuilder.Entity<Clubs.Models.Trip>().Property(nameof(Clubs.Models.Trip.RequiredQualifications)).HasConversion(new ValueConverter<IList<string>, string>(
                                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                                v => JsonConvert.DeserializeObject<IList<string>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })));
        }
        public DbSet<Clubs.Models.Trip> Trip { get; set; }
    }
}
