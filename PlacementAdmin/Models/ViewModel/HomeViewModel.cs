namespace PlacementAdmin.Models.ViewModel
{
    public class HomeViewModel
    {
        public SignUpViewModel SignUpViewModel { get; set; }
        public loginViewModel loginViewModel { get; set; }

        public List<CourseStream> CourseStreams { get; set; }
    }
}
