using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    public partial class KinshipDb : DbContext
    {
        public KinshipDb()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Personage> Personage { get; set; }
        public virtual DbSet<Lblhelpmessage> Lblhelpmessage { get; set; }
        public virtual DbSet<Picture> Picture { get; set; }
        public virtual DbSet<PictureType> PictureType { get; set; }
        public virtual DbSet<ConsumptionExpenditure> ConsumptionExpenditure { get; set; }

        public virtual DbSet<Menus> Menus { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personage>()
                .Property(e => e.Email)
                .IsUnicode(false);
            modelBuilder.Entity<Personage>()
              .Property(e => e.Weight);
            modelBuilder.Entity<PictureType>()
           .Property(e => e.PictureTypeName);
            modelBuilder.Entity<ConsumptionExpenditure>()
          .Property(e => e.Id);
            modelBuilder.Entity<Menus>()
        .Property(e => e.Id);
        }
    }
}