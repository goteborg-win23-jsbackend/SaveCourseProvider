using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaveCourseProvider.Data.Context;
using SaveCourseProvider.Data.Entity;
using SaveCourseProvider.Model;

namespace SaveCourseProvider.Functions
{
    public class GetSavedCourse(ILogger<GetSavedCourse> logger, IServiceProvider serviceProvider)
    {
        private readonly ILogger<GetSavedCourse> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [Function("GetSavedCourse")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                if(body != null) {

                    var requestBody = JsonConvert.DeserializeObject<CourseRequest>(body)!;

                    using var context = _serviceProvider.GetRequiredService<DataContext>();

                    List<SavedCoursesEntity> savedCourses = await context.SavedCourses.Where(uc => uc.UserId == requestBody.UserId).ToListAsync();


                    if(savedCourses != null)
                    {
                        List<Course> courses = new List<Course>();

                        foreach(var course in savedCourses)
                        {
                            courses.Add(new Course
                            {
                                CourseId = course.CourseId
                            });
                        }

                        return new OkObjectResult(courses);
                    }
                    return new BadRequestResult();
                }
                


            }
            catch (Exception ex)
            {
                _logger.LogError($"StreamReader :: {ex.Message}");
            }
            return new BadRequestResult();
        }
    }
}
