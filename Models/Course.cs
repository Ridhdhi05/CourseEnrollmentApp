namespace CourseEnrollmentApp.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public decimal Fees { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}