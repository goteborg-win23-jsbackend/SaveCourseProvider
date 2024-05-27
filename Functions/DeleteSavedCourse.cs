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
    public class DeleteSavedCourse(ILogger<DeleteSavedCourse> logger, IServiceProvider serviceProvider)
    {
        private readonly ILogger<DeleteSavedCourse> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [Function("DeleteSavedCourse")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)

        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();

                if (body != null)
                {
                    try
                    {
                        var requestBody = JsonConvert.DeserializeObject<List<Course>>(body);

                        if (requestBody != null)
                        {

                            using var context = _serviceProvider.GetRequiredService<DataContext>();

                            foreach(var course in requestBody)
                            {
                                var exists = await context.SavedCourses.AsNoTracking().FirstOrDefaultAsync(x => x.CourseId == course.CourseId);
                                if (exists != null)
                                {
                                    context.SavedCourses.Remove(exists);
                                    await context.SaveChangesAsync();

                                }
                            }

                            return new OkResult();
                            
                        }



                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"JsonConvert.DeserializeObject<CourseModel>(body) :: {ex.Message}");
                    }
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

