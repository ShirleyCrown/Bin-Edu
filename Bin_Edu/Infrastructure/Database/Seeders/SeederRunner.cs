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
            await GenerateSubjectData();
            await GenerateCourseData();
            // await GenerateCourseExerciseData();
            // await GenerateCourseRegistrationData();
            // await GenerateExerciseSubmissionData();

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
            // ===== ADMIN =====
            var admin = new AppUser
            {
                UserName = "admin",
                Email = "N/A",
                FullName = "Quản trị viên",
                PhoneNumber = "N/A",
                Grade = "N/A",
                School = "N/A",
                Dob = new DateOnly(1990, 1, 1),
                EmailConfirmed = true,
            };

            await _userManager.CreateAsync(admin, "123");
            await _userManager.AddToRoleAsync(admin, "ADMIN");

            // ===== STUDENTS =====
            for (int i = 1; i <= 5; i++)
            {
                var student = new AppUser
                {
                    UserName = $"student{i}@example.com",
                    Email = $"student{i}@example.com",
                    FullName = $"Sinh viên {i}",
                    PhoneNumber = $"09000000{i}",
                    Grade = "12",
                    School = "THPT ABC",
                    Dob = new DateOnly(2006, 1, i),
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(student, "123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(student, "STUDENT");
                }
            }

            Console.WriteLine("👨‍🏫 Admin + 10 Students generated.");
        }


        // // ================================================================
        // // USERS
        // // ================================================================
        private async Task GenerateSubjectData()
        {
            var subjects = new List<Subject>
            {
                new Subject { SubjectName = "Math", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { SubjectName = "English", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Subject { SubjectName = "Literature", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            await _context.Subjects.AddRangeAsync(subjects);
            await _context.SaveChangesAsync();

            Console.WriteLine("Subjects generated.");
        }


        // // ================================================================
        // // COURSES
        // // ================================================================
        private async Task GenerateCourseData()
        {

            var courses = new List<Course>
            {
                new Course
                {
                    TeachingTeacherName = "Nguyen Van An",
                    CourseTitle = "Basic Algebra",
                    CourseDescription = "Learn foundational algebra concepts and problem-solving skills.",
                    Subject = _context.Subjects.FirstOrDefault(s => s.SubjectName == "Math"),
                    CoursePrice = 1200000,
                    OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                    EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(10 * 7)
                },
                // new Course
                // {
                //     TeachingTeacherName = "Tran Thi Bich",
                //     CourseTitle = "Geometry Mastery",
                //     CourseDescription = "Understand shapes, angles, and geometric proofs.",
                //     CourseSubject = "Math",
                //     CoursePrice = 1500000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(12 * 7)
                // },
                // new Course
                // {
                //     TeachingTeacherName = "Le Thanh Hai",
                //     CourseTitle = "English Grammar for Beginners",
                //     CourseDescription = "Master basic English grammar rules and sentence structures.",
                //     CourseSubject = "English",
                //     CoursePrice = 1000000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(20 * 7)
                // },
                // new Course
                // {
                //     TeachingTeacherName = "Pham Minh Chau",
                //     CourseTitle = "Spoken English Essentials",
                //     CourseDescription = "Improve daily communication and pronunciation skills.",
                //     CourseSubject = "English",
                //     CoursePrice = 1800000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(15 * 7)
                // },
                // new Course
                // {
                //     TeachingTeacherName = "Vo Hoang Long",
                //     CourseTitle = "Advanced English Writing",
                //     CourseDescription = "Learn to write essays, reports, and professional emails.",
                //     CourseSubject = "English",
                //     CoursePrice = 2000000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(12 * 7)
                // },
                // new Course
                // {
                //     TeachingTeacherName = "Nguyen Thi Lan",
                //     CourseTitle = "Vietnamese Literature Basics",
                //     CourseDescription = "Explore classic Vietnamese literary works and writers.",
                //     CourseSubject = "Literature",
                //     CoursePrice = 900000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(17 * 7)
                // },
                // new Course
                // {
                //     TeachingTeacherName = "Do Quang Minh",
                //     CourseTitle = "Modern Literature Analysis",
                //     CourseDescription = "Analyze modern novels, poems, and short stories.",
                //     CourseSubject = "Literature",
                //     CoursePrice = 1400000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(16 * 7)
                // },
                // new Course
                // {
                //     TeachingTeacherName = "Bui Thu Ha",
                //     CourseTitle = "Trigonometry Made Easy",
                //     CourseDescription = "Learn sine, cosine, tangent, and practical applications.",
                //     CourseSubject = "Math",
                //     CoursePrice = 1600000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(10 * 7)
                // },
                // new Course
                // {
                //     TeachingTeacherName = "Pham Duc Khang",
                //     CourseTitle = "IELTS English Preparation",
                //     CourseDescription = "Full preparation for all four IELTS skills.",
                //     CourseSubject = "English",
                //     CoursePrice = 3200000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(10 * 7)
                // },
                // new Course
                // {
                //     TeachingTeacherName = "Ngo Thi Mai",
                //     CourseTitle = "Poetry Appreciation",
                //     CourseDescription = "Understand and enjoy poetry through guided analysis.",
                //     CourseSubject = "Literature",
                //     CoursePrice = 1100000,
                //     OpeningDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today).AddDays(10 * 7)
                // }
            };


            foreach (var course in courses)
            {

                // Course timetables
                course.CourseTimetables = new List<CourseTimetable>();

                List<DateOnly> courseStartDates = this
                    .GetDatesByDayOfWeek(course.OpeningDate, course.EndDate, DayOfWeek.Monday);

                foreach (var courseStartDate in courseStartDates)
                {
                    CourseTimetable courseTimetable = new CourseTimetable
                    {
                        StartTime = TimeOnly.Parse("08:00:00"),
                        EndTime = TimeOnly.Parse("11:00:00"),
                        DayOfWeek = "Monday",
                        StartDate = courseStartDate,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    course.CourseTimetables.Add(courseTimetable);
                }


                // // Course execises
                // course.CourseExercises = new List<CourseExercise>();
            }


            await _context.Courses.AddRangeAsync(courses);
            await _context.SaveChangesAsync();

            Console.WriteLine("Courses + Timetables generated.");

        }


        private async Task GenerateCourseExerciseData()
        {
            var courses = await _context.Courses.ToListAsync();

            var exercises = new List<CourseExercise>();

            foreach (var course in courses)
            {
                for (int i = 1; i <= 10; i++)
                {
                    exercises.Add(new CourseExercise
                    {
                        CourseId = course.Id,
                        ExerciseName = $"Exercise {i} - {course.CourseTitle}",
                        ExerciseDescription = $"Exercise {i} for course {course.CourseTitle}",
                        ExerciseSubmitDeadline = DateTime.UtcNow.AddDays(i * 7),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.CourseExercises.AddRangeAsync(exercises);
            await _context.SaveChangesAsync();

            Console.WriteLine("Course exercises generated.");
        }

        public async Task GenerateCourseRegistrationData()
        {
            var courses = await _context.Courses.ToListAsync();
            var students = await _userManager.GetUsersInRoleAsync("STUDENT");

            var registrations = new List<CourseRegistration>();

            foreach (var course in courses)
            {
                foreach (var student in students)
                {
                    registrations.Add(new CourseRegistration
                    {
                        CourseId = course.Id,
                        StudentId = student.Id,
                        RegisteredAt = DateTime.UtcNow
                    });
                }
            }

            await _context.CourseRegistrations.AddRangeAsync(registrations);
            await _context.SaveChangesAsync();

            Console.WriteLine("Course registrations generated.");
        }

        private async Task GenerateExerciseSubmissionData()
        {
            var exercises = await _context.CourseExercises.ToListAsync();
            var students = await _userManager.GetUsersInRoleAsync("STUDENT");

            var submissions = new List<ExerciseSubmission>();

            foreach (var exercise in exercises)
            {
                foreach (var student in students)
                {
                    submissions.Add(new ExerciseSubmission
                    {
                        CourseExerciseId = exercise.Id,
                        CourseRegistrationId = 1, // Giả sử có CourseRegistrationId hợp lệ
                        SubmittedAt = DateTime.UtcNow,
                        FileName = $"submission_{student.UserName}_exercise_{exercise.Id}.pdf",
                    });
                }
            }

            await _context.ExerciseSubmissions.AddRangeAsync(submissions);
            await _context.SaveChangesAsync();

            Console.WriteLine("Exercise submissions generated.");
        }

        // ========== PRIVATE METHODS ==============
        private List<DateOnly> GetDatesByDayOfWeek(DateOnly start, DateOnly end, DayOfWeek targetDay)
        {
            var result = new List<DateOnly>();

            // Move start forward to the first matching day
            int daysToAdd = ((int)targetDay - (int)start.DayOfWeek + 7) % 7;
            var firstMatch = start.AddDays(daysToAdd);

            // If the first matching day is after end date, return empty
            if (firstMatch > end)
                return result;

            // Add weekly occurrences
            for (var date = firstMatch; date <= end; date = date.AddDays(7))
            {
                result.Add(date);
            }

            return result;
        }

    }
}
