using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Angular101.Data;
using Angular101.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Angular101.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public QuizController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region Attribute-based routing methods
        /// <summary>
        /// GET: api/quiz/latest
        /// Retrieves the {num} latest Quizzes
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>the {num} latest Quizzes</returns>
        [HttpGet("Latest/{num:int?}")]
        public IActionResult Latest(int num = 10)
        {
            var latest = dbContext.Quizzes.OrderByDescending(q => q.CreatedDate)
                        .Take(num)
                        .ToArray();

            return new JsonResult(latest.Adapt<QuizViewModel[]>(), serializerSettings: new JsonSerializerOptions() { WriteIndented = true });
        }
        #endregion

        /// <summary>
        /// GET: api/quiz/ByTitle
        /// Retrieves the {num} Quizzes sorted by Title (A to Z)
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>{num} Quizzes sorted by Title</returns>
        [HttpGet("ByTitle/{num:int?}")]
        public IActionResult ByTitle(int num = 10)
        {
            //var sampleQuizzes = ((JsonResult)Latest(num)).Value  as List<QuizViewModel>;

            var byTitle = dbContext.Quizzes.OrderBy(q => q.Title)
                .Take(num)
                .ToArray();


            return new JsonResult(byTitle.Adapt<QuizViewModel[]>(), new JsonSerializerOptions() { WriteIndented = true });

        }

        /// <summary>
        /// GET: api/quiz/mostViewed
        /// Retrieves the {num} random Quizzes
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>{num} random Quizzes</returns>
        [HttpGet("Random/{num:int?}")]
        public IActionResult Random(int num = 10)
        {
            //var sampleQuizzes = ((JsonResult)Latest(num)).Value as List<QuizViewModel>;

            var random = dbContext.Quizzes
                .OrderBy(q => Guid.NewGuid())
                .Take(num)
                .ToArray();


            return new JsonResult(random.Adapt<QuizViewModel[]>(), new JsonSerializerOptions() { WriteIndented = true });

        }


        #region RESTful conventions methods

        /// <summary>
        /// GET: api/quiz/{}id
        /// Retrieves the Quiz with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Quiz</param>
        /// <returns>the Quiz with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            var quiz = dbContext.Quizzes.FirstOrDefault(i => i.Id == id);

            if (quiz == null)
            {
                return NotFound(new { Error = $"Quiz ID {id} has not been found"});
            }


            return new JsonResult(
            quiz.Adapt<QuizViewModel>(),
            new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }

        /// <summary>
        /// Adds a new Quiz to the Database
        /// </summary>
        /// <param name="model">The QuizViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put([FromBody] QuizViewModel model)
        {
            
            if (model == null) return new StatusCodeResult(500);

            var quiz = new Quiz() { 
               Title = model.Title,
               Description = model.Description,
               Text = model.Text,
               Notes = model.Notes
            };

            quiz.CreatedDate = DateTime.Now;
            quiz.LastModifiedDate = quiz.CreatedDate;

            quiz.UserId = dbContext.Users.FirstOrDefault(u => u.UserName == "Admin").Id;

            dbContext.Quizzes.Add(quiz);

            dbContext.SaveChanges();

            return new JsonResult(quiz.Adapt<QuizViewModel>(), new JsonSerializerOptions()
            {
                WriteIndented = true
            });
            

        }

        /// <summary>
        /// Edit the Quiz with the given {id}
        /// </summary>
        /// <param name="model">The QuizViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] QuizViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            var quiz = dbContext.Quizzes.FirstOrDefault(q => q.Id == model.Id);

            if (quiz == null)
            {
                return NotFound(new { Error = $"Quiz ID {model.Id} has not been found." });
            }

            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;

            quiz.LastModifiedDate = quiz.CreatedDate;

            dbContext.SaveChanges();

            return new JsonResult(quiz.Adapt<QuizViewModel>(), 
                   new JsonSerializerOptions() { WriteIndented = true });
            
        }

        /// <summary>
        /// Deletes the Quiz with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Quiz</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var quiz = dbContext.Quizzes.FirstOrDefault(i => i.Id == id);

            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = $"Quiz ID {id} has not been found"
                });
            }

            dbContext.Quizzes.Remove(quiz);

            dbContext.SaveChanges();

            return new OkResult();

        }

        

        #endregion

    }
}