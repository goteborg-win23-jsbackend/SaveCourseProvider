

using Microsoft.EntityFrameworkCore;
using SaveCourseProvider.Data.Entity;

namespace SaveCourseProvider.Data.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<SavedCoursesEntity> SavedCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavedCoursesEntity>()
     .HasKey(uc => new { uc.UserId, uc.CourseId });

    }
}
