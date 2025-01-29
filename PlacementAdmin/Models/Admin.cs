namespace PlacementAdmin.Models
{
    public class QuestionBank
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int CourseStreamId { get; set; }
        public string DifficultyLevel { get; set; }
        public ICollection<Option> Options { get; set; }
    }

    public class Option
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
        public string CodeAnswer { get; set; }
    }

    public class Test
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int CourseStreamId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class TestQuestion
    {
        public int TestQuestionId { get; set; }
        public int TestId { get; set; }
        public int QuestionId { get; set; }
    }

    public class StudentTest
    {
        public int StudentTestId { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool Completed { get; set; }
    }

    public class StudentAnswer
    {
        public int AnswerId { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; } 
        public string AnswerText { get; set; } 
    }
    // for fronr end


    public class TestAssignmentModel
    {
        public int TestId { get; set; }
        public List<int> UserIds { get; set; }
        public List<CourseStream> CourseStreams { get; set; } 
    }




    public class TestQuestionsModel
   {
       public int TestId { get; set; }
       public List<int> QuestionIds { get; set; }
   }



    public class TestDetails
    {
        public int TestId { get; set; }
        public List<QuestionWithOptions> Questions { get; set; }
    }

    public class QuestionWithOptions
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public List<Option> Options { get; set; }
    }




    public class TestCreationViewModel
    {
        public Test Test { get; set; }
        public List<CourseStream> CourseStreams { get; set; }
        public List<dynamic> IncompleteTests { get; set; }
    }

    public class AddQuestionViewModel
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; } //'Mcq'or 'code'
        public int CourseStreamId { get; set; }
        public string AnswerFieldForCoding { get; set; }
        public string DifficultyLevel { get; set; } 
        public List<Option> Options { get; set; }
        public List<CourseStream> CourseStreams { get; set; }

        public AddQuestionViewModel()
        {
            Options = new List<Option>();
            CourseStreams = new List<CourseStream>();
        }
    }




    public class TestQuestionViewModel
    {
        public int TestId { get; set; }
        public List<int> SelectedQuestionIds { get; set; }
        public List<QuestionBank> Questions { get; set; }
        public List<Test> Tests { get; set; }
        public List<CourseStream> CourseStreams { get; set; } 
        public List<string> DifficultyLevels { get; set; } 
        public List<string> QuestionTypes { get; set; }
        public string SelectedCourseStream { get; set; }
        public string SelectedDifficultyLevel { get; set; }
        public string SelectedQuestionType { get; set; }

        public TestQuestionViewModel()
        {
            SelectedQuestionIds = new List<int>();
            Questions = new List<QuestionBank>();
            Tests = new List<Test>();
            CourseStreams = new List<CourseStream>();
            DifficultyLevels = new List<string> { "Easy", "Medium", "Hard" };
            QuestionTypes = new List<string> { "MCQ", "Code" };
        }
    }

    public class TopThreeStudents 
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public int TotalScore { get; set; }
    
    }







}
