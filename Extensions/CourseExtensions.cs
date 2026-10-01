using CourseEnrollmentApp.Models;

namespace CourseEnrollmentApp.Extensions
{
    public static class CourseExtensions
    {
        // Extension method to calculate final fees after discount
        public static decimal CalculateFinalFees(this Course course)
        {
            if (course == null) return 0;
            decimal discountAmount = course.Fees * (course.DiscountPercentage / 100);
            return course.Fees - discountAmount;
        }
    }
}