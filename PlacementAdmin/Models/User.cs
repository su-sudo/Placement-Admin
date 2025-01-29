namespace PlacementAdmin.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int CourseStreamId { get; set; }
        public string Role { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime DateOfBirth { get; set; }
    }


    public class TestResult
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double ScorePercentage { get; set; }
    }


}
