

namespace SaveCourseProvider.Model;

public class CourseModel
{
    public int UserId { get; set; }
    public string CourseId { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Title { get; set; } = null!;
}
