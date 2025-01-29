using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PlacementAdmin.DAL;
using PlacementAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlacementAdmin.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminDAL _adminDAL;
        private readonly UserDAL _userDAL;
        private readonly IServiceScopeFactory _scopeFactory;

        public AdminController(AdminDAL adminDAL, UserDAL userDAL, IServiceScopeFactory scopeFactory)
        {
            _adminDAL = adminDAL;
            _userDAL = userDAL;
            _scopeFactory = scopeFactory;
        }
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Index","Home");
            }
            
            List<TopThreeStudents> topStudents = await _adminDAL.GetTopThreeStudents();
            return View(topStudents);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddCourseStream()
        {

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddCourseStream(CourseStream courseStream)
        {
            try
            {
                _adminDAL.AddCourseStream(courseStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex} has occurred while adding new course");
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateTest()
        {
            var viewModel = new TestCreationViewModel();

            try
            {
                using (var scope = _scopeFactory.CreateScope()) 
                {
                    var _adminDALScoped = scope.ServiceProvider.GetRequiredService<AdminDAL>();
                    viewModel.IncompleteTests = await _adminDALScoped.GetIncompleteTests();
                }
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _userDALScoped = scope.ServiceProvider.GetRequiredService<UserDAL>();
                    Console.WriteLine(scope.GetType());
                    viewModel.CourseStreams = await _userDALScoped.GetAvailableCourseStreamsAsync();
                }
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching course streams: {ex.Message}");
            }

            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateTest(TestCreationViewModel viewModel)
        {
            try
            {
                viewModel.Test.CreatedDate = DateTime.Now;
                int testId = await _adminDAL.AddTest(viewModel.Test);

                if (testId > 0)
                {
                    TempData["SuccessMessage"] = "Test created successfully!";
                    return RedirectToAction(nameof(CreateTest));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create test.");
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while creating a test: {ex.Message}");
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQuestion()
        {
            var viewModel = new AddQuestionViewModel();

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var userDAL = scope.ServiceProvider.GetRequiredService<UserDAL>();
                    viewModel.CourseStreams = await userDAL.GetAvailableCourseStreamsAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching course streams: {ex.Message}");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQuestion(AddQuestionViewModel viewModel)
        {
                if (viewModel.QuestionType =="Code")
                {
                    foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Options")).ToList()) 
                    { 
                        ModelState.Remove(key); 
                    }
                
                }
                
                try
                {
                    var question = new QuestionBank
                    {
                        QuestionText = viewModel.QuestionText,
                        QuestionType = viewModel.QuestionType,
                        CourseStreamId = viewModel.CourseStreamId,
                        DifficultyLevel = viewModel.DifficultyLevel,
                        Options = viewModel.QuestionType == "MCQ" ? viewModel.Options : new List<Option> { new Option {CodeAnswer = viewModel.AnswerFieldForCoding } }
                        //Options = viewModel.QuestionType == "Coding" ? viewModel.AnswerFieldForCoding : null

                    };


                    int questionId = await _adminDAL.AddQuestionWithOptions(question, viewModel.Options);
                    if (questionId > 0)
                    {
                        TempData["SuccessMessage"] = "Question added successfully!";
                        return RedirectToAction(nameof(AddQuestion));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to add question.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while adding the question: {ex.Message}");
                }
            

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var userDAL = scope.ServiceProvider.GetRequiredService<UserDAL>();
                    viewModel.CourseStreams = await userDAL.GetAvailableCourseStreamsAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching course streams: {ex.Message}");
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQuestionsToTest()
        {
            var viewModel = new TestQuestionViewModel();

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var userDAL = scope.ServiceProvider.GetRequiredService<UserDAL>();
                    viewModel.CourseStreams = await userDAL.GetAvailableCourseStreamsAsync();
                }
                using (var scope = _scopeFactory.CreateScope())
                {
                    var adminDAL = scope.ServiceProvider.GetRequiredService<AdminDAL>();
                    viewModel.Tests = await _adminDAL.GetAllTests();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching data: {ex.Message}");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQuestionsToTest(TestQuestionViewModel viewModel)
        {
            
                try
                {
                    var testQuestions = viewModel.SelectedQuestionIds
                        .Select(questionId => new TestQuestion
                        {
                            TestId = viewModel.TestId,
                            QuestionId = questionId
                        })
                        .ToList();

                    await _adminDAL.AddQuestionsToTest(viewModel.TestId, testQuestions);
                    TempData["SuccessMessage"] = "Questions added successfully!";
                    return RedirectToAction(nameof(AddQuestionsToTest));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while adding questions to the test: {ex.Message}");
                }
            

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FetchQuestions(string courseStream, string difficultyLevel, string questionType)
        {
            var questions = await _adminDAL.GetQuestionsByCriteria(courseStream, difficultyLevel, questionType);
            return PartialView("_QuestionList", questions);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTestToStudents()
        {
            var viewModel = new TestAssignmentModel();

            try
            {
                viewModel.CourseStreams = await _userDAL.GetAvailableCourseStreamsAsync(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching data: {ex.Message}");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTestToStudents(TestAssignmentModel viewModel)
        {
            try
            {
                DateTime assignedDate = DateTime.Now;

                var studentTests = new List<StudentTest>();
                foreach (var userId in viewModel.UserIds)
                {
                    studentTests.Add(new StudentTest { TestId = viewModel.TestId, UserId = userId, AssignedDate = assignedDate, Completed = false });
                }

                await _adminDAL.AssignTestToStudents(viewModel.TestId, studentTests, assignedDate);

                TempData["SuccessMessage"] = "Test assigned to students successfully!";
                return RedirectToAction(nameof(AssignTestToStudents));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while assigning the test to students: {ex.Message}");
                return View(viewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> FetchStudents(int courseStreamId)
        {
            var students = await _adminDAL.GetStudentsByCourseStream(courseStreamId);
            return Json(students);
        }

        
        [HttpPost]
        public async Task<IActionResult> FetchTests(int courseStreamId)
        {
            var tests = await _adminDAL.GetTestsByCourseStream(courseStreamId);
            return Json(tests);
        }

       

    }
}
