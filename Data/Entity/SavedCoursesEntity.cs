

namespace SaveCourseProvider.Data.Entity;

public class SavedCoursesEntity
{
    public int UserId { get; set; }
    public string CourseId { get; set; } = null!;
}
