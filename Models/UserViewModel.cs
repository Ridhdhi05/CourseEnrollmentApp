namespace CourseEnrollmentApp.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public CompanyViewModel Company { get; set; } = new CompanyViewModel();
    }

    public class CompanyViewModel
    {
        public string Name { get; set; } = string.Empty;
    }
}