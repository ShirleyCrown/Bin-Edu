using Bin_Edu.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bin_Edu.Infrastructure.Database.Seeders
{
    public class SeederRunner
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDBContext _context;




        public SeederRunner(UserManager<AppUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            AppDBContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }





        public async Task ExecuteGeneration()
        {
            Console.WriteLine("🌱 Executing Seeder...");

            this.CleanUpData();

            await GenerateRoleData();
            await GenerateUserData();

            Console.WriteLine("✅ Seeder completed successfully!");
        }

        // ================================================================
        // CLEANUP — Giữ lại admin, xóa an toàn theo thứ tự FK
        // ================================================================
        private void CleanUpData()
        {
            try
            {
                var skipTables = new[] { "AspNetUsers", "AspNetRoles", "AspNetUserRoles", "AspNetUserClaims", "AspNetUserLogins", "AspNetUserTokens" };

                var tables = _context.Model.GetEntityTypes()
                    .Select(t => t.GetTableName())
                    .Distinct()
                    .ToList();

                // Disable all constraints
                foreach (var table in tables)
                {
                    _context.Database.ExecuteSqlRaw($"ALTER TABLE [{table}] NOCHECK CONSTRAINT ALL;");
                }

                // Delete all data and reset identity
                foreach (var table in tables)
                {
                    if (skipTables.Contains(table))
                    {
                        _context.Database.ExecuteSqlRaw($@"
                            DELETE FROM [{table}];"
                        );

                        continue;
                    }
                    _context.Database.ExecuteSqlRaw($@"
                        DELETE FROM [{table}];
                        DBCC CHECKIDENT ('[{table}]', RESEED, 0);"
                    );
                }

                // Re-enable constraints
                foreach (var table in tables)
                {
                    _context.Database.ExecuteSqlRaw($"ALTER TABLE [{table}] WITH CHECK CHECK CONSTRAINT ALL;");
                }

                Console.WriteLine("Clean Up Data Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during cleanup:");
                Console.WriteLine(ex.ToString());
            }
        }


        // ================================================================
        // ROLE
        // ================================================================
        private async Task GenerateRoleData()
        {
            string[] roles = { "ADMIN", "STUDENT" };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }

            Console.WriteLine("👥 Roles generated.");
        }

        // // ================================================================
        // // USERS
        // // ================================================================
        private async Task GenerateUserData()
        {

            // Admin 
            var admin = new AppUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FullName = "Quản trị viên",
                PhoneNumber = "N/A",
                Grade = "N/A",
                School = "N/A",
                Dob = new DateOnly(1990, 1, 1),
                EmailConfirmed = true,
            };

            await _userManager.CreateAsync(admin, "123");
            await _userManager.AddToRoleAsync(admin, "ADMIN");

            Console.WriteLine("👨‍🏫 Users generated (admin kept).");
        }


    }
}
