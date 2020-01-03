using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angular101.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using Angular101.Data;
using Mapster;

namespace Angular101.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public QuestionController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET api/question/all
        [HttpGet("All/{quizId}")]
        public IActionResult All(int quizId)
        {
            var sampleQuestions = new List<QuestionViewModel>();
            // add a first sample question
            sampleQuestions.Add(new QuestionViewModel()
            {
                Id = 1,
                QuizId = quizId,
                Text = "What do you value most in your life?",
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            });

            // add a bunch of other sample questions
            for (int i = 2; i <= 5; i++)
            {
                sampleQuestions.Add(new QuestionViewModel()
                {
                    Id = i,
                    QuizId = quizId,
                    Text = String.Format("Sample Question {0}", i),
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                });
            }

            // output the result in JSON format
            return new JsonResult(sampleQuestions, new JsonSerializerOptions
                                    {
                                        WriteIndented = true
                                    });
        }

        #region RESTful conventions methods
        /// <summary>
        /// Retrieves the Question with the given {id}
        /// </summary>
        /// &lt;param name="id">The ID of an existing Question</param>
        /// <returns>the Question with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var question = dbContext.Questions.FirstOrDefault(q=> q.Id == id);

            if (question == null)
            {
                return NotFound(new
                {
                    Error = $"Question ID {id} has not been fount"
                }); ;
            }

            return new JsonResult(question.Adapt<QuestionViewModel>(),
                    new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    });


        }

        /// <summary>
        /// Adds a new Question to the Database
        /// </summary>
        /// <param name="model">The QuestionViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put([FromBody]QuestionViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            var question = model.Adapt<Question>();

            question.QuizId = model.QuizId;
            question.Text = model.Text;
            question.Notes = model.Notes;

            question.CreatedDate = DateTime.Now;
            question.LastModifiedDate = question.CreatedDate;

            dbContext.Questions.Add(question);

            dbContext.SaveChanges();

            return new JsonResult(question.Adapt<QuestionViewModel>(),
                new JsonSerializerOptions()
                {
                    WriteIndented = true
                });
        }

        /// <summary>
        /// Edit the Question with the given {id}
        /// </summary>
        /// <param name="model">The QuestionViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody] QuestionViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            var question = dbContext.Questions.FirstOrDefault(q => q.Id == model.Id);

            if (question == null)
            {
                return NotFound(new
                {
                    Error = $"Question ID {model.Id} has not been found."
                });
            }

            question.QuizId = model.QuizId;
            question.Text = model.Text;
            question.Notes = model.Notes;

            question.LastModifiedDate = question.CreatedDate;

            dbContext.SaveChanges();

            return new JsonResult(question.Adapt<QuestionViewModel>(),
                new JsonSerializerOptions()
                {
                    WriteIndented = true
                });
        }

        /// <summary>
        /// Deletes the Question with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Question</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}