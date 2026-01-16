using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bin_Edu.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bin_Edu.Infrastructure
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        private readonly IConfiguration _configuration;

        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }

        // ----------------- TABLES -----------------------------
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTimetable> CourseTimetables { get; set; }
        public DbSet<CourseRegistration> CourseRegistrations { get; set; }
        public DbSet<CourseExercise> CourseExercises { get; set; }
        public DbSet<CourseAttendance> CourseAttendances { get; set; }
        public DbSet<ExerciseSubmission> ExerciseSubmissions { get; set; }
        public DbSet<CoursePayment> CoursePayments { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ExerciseSubmission>()
                .HasOne(es => es.CourseRegistration)
                .WithMany(cr => cr.ExerciseSubmissions)
                .HasForeignKey(es => es.CourseRegistrationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExerciseSubmission>()
                .HasOne(es => es.CourseExercise)
                .WithMany(ce => ce.ExerciseSubmissions)
                .HasForeignKey(es => es.CourseExerciseId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<CourseAttendance>()
                .HasOne(ca => ca.CourseTimetable)
                .WithMany(ct => ct.CourseAttendances)
                .HasForeignKey(ca => ca.CourseTimetableId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CourseAttendance>()
                .HasOne(ca => ca.CourseRegistration)
                .WithMany(cr => cr.CourseAttendances)
                .HasForeignKey(ca => ca.CourseRegistrationId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}