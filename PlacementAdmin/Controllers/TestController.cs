using Microsoft.AspNetCore.Mvc;
using PlacementAdmin.DAL;
using PlacementAdmin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlacementAdmin.Controllers
{
    public class TestsController : Controller
    {
        private readonly AdminDAL _adminDAL;
        private readonly UserDAL _userDAL;

        public TestsController(AdminDAL adminDAL, UserDAL userDAL)
        {
            _adminDAL = adminDAL;
            _userDAL = userDAL;
        }
        public async Task<IActionResult> AvailableTests()
        {
            int userId = GetLoggedInUserId();
            List<Test> availableTests = await _userDAL.GetAvailableTestsForUser(userId);
            return View(availableTests);
        }

        private int GetLoggedInUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            throw new System.Exception("User is not authenticated or user ID claim is missing.");
        }
        public async Task<IActionResult> TakeTest(int id)
        {
            var testDetails = await _userDAL.GetTestQuestionsAsync(id);
            return View(testDetails);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTest(int testId, int userId, Dictionary<int, string> answers)
        {
            var studentAnswers = answers.Select(answer =>
            {
                int questionId = answer.Key;
                string value = answer.Value;
                int selectedOptionId;
                if (int.TryParse(value, out selectedOptionId))
                {
                    return new StudentAnswer
                    {
                        UserId = userId,
                        TestId = testId,
                        QuestionId = questionId,
                        SelectedOptionId = selectedOptionId
                    };
                }
                else
                {
                    return new StudentAnswer
                    {
                        UserId = userId,
                        TestId = testId,
                        QuestionId = questionId,
                        AnswerText = value
                    };
                }
            }).ToList();

            await _userDAL.SaveStudentAnswersAsync(studentAnswers);
            return RedirectToAction("TestResults", "User");
        }
        public IActionResult TestResults()
        {
            
            return View();
        }
    }
}
