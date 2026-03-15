using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace asp_all.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entities.UserAccess> UserAccesses { get; set; }

        public DbSet<Entities.UserData> UsersData { get; set; }

        public DbSet<Entities.UserRole> UserRoles { get; set; }

        public DataContext(DbContextOptions options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // налаштування моделі БД: а) відношення між сутностями
            modelBuilder.Entity<Entities.UserAccess>()
                .HasIndex(a => a.Login)
                .IsUnique();

            modelBuilder.Entity<Entities.UserAccess>()
                .HasOne(a => a.UserData)
                .WithMany(d => d.UserAccesses)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Entities.UserAccess>()
                .HasOne(a => a.UserRole)
                .WithMany();

            // б) початкові дані
            modelBuilder.Entity<Entities.UserRole>().HasData(
                new Entities.UserRole
                {
                    Id = Guid.Parse("BC84C3AA-F62F-44C6-B822-AE954F450A53"),
                    Name = "Self Registered",
                    Description = "Користувачі, що самі зареєструвались на сайті. Мінімальні права доступу",
                    CreateLevel = 0,
                    ReadLevel = 0,
                    UpdateLevel = 0,
                    DeleteLevel = 0
                },
                new Entities.UserRole
                {
                    Id = Guid.Parse("56D473BA-ED6B-4695-AEBF-439E2102F2C3"),
                    Name = "Root Administrator",
                    Description = "Користувач з максимальним доступом, через якого вводяться інші ролі та доступи",
                    CreateLevel = -1,
                    ReadLevel = -1,
                    UpdateLevel = -1,
                    DeleteLevel = -1
                }
            );

            modelBuilder.Entity<Entities.UserData>().HasData(
                new Entities.UserData
                {
                    Id = Guid.Parse("41E5ED40-AB13-4B5C-B1D0-3722023EA5C7"),
                    Name = "Default Administrator",
                    Email = "admin@change.me",
                    Birthdate = new DateTime(2026, 3, 12),

                }
            );

            modelBuilder.Entity<Entities.UserAccess>().HasData(
                new Entities.UserAccess
                {
                    Id = Guid.Parse("F0E98EF0-917F-4BF7-90E9-CBA9BBD86C04"),
                    UserId = Guid.Parse("41E5ED40-AB13-4B5C-B1D0-3722023EA5C7"),
                    UserRoleId = Guid.Parse("56D473BA-ED6B-4695-AEBF-439E2102F2C3"),
                    Login = "DefaultAdministrator",
                    Salt = "4009BA69-7EFC-4E4F-A9AF-FEC77B759BC6",
                    CreatedAt = new DateTime(2026, 3, 12),
                    Dk = "BA8C615C65B7A66750C12605DF7602FDB037A12C"
                }
            );
        }
    }
}
