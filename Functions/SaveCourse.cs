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
using SaveCourseProvider.Services;


namespace SaveCourseProvider.Functions
{
    public class SaveCourse(ILogger<SaveCourse> logger, IServiceProvider serviceProvider, SendEmailConfirmation emailService)
    {
        private readonly ILogger<SaveCourse> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly SendEmailConfirmation emailService = emailService;

        [Function("SaveCourse")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();

                if (body != null)
                {
                    try
                    {
                        var requestBody = JsonConvert.DeserializeObject<CourseModel>(body)!;

                        if (requestBody != null)
                        {
                            SavedCoursesEntity entity = new SavedCoursesEntity
                            {
                                UserId = requestBody.UserId,
                                CourseId = requestBody.CourseId,

                            };


                            using var context = _serviceProvider.GetRequiredService<DataContext>();

                            var exists = await context.SavedCourses.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == entity.UserId && x.CourseId == entity.CourseId);

                            if(exists != null)
                            {
                                context.SavedCourses.Remove(entity);
                                await context.SaveChangesAsync();

                                return new OkObjectResult("removed");
                            }


                            context.SavedCourses.Add(entity);
                            await context.SaveChangesAsync();

                            var sendEmail = await emailService.GenerateEmailRequestAsync(requestBody);

                            if(sendEmail)
                            {
                                return new OkResult();
                            }
                            else
                            {
                                return new BadRequestResult();
                            }
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
