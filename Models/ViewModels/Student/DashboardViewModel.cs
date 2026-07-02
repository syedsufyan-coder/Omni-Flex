using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class DashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string GreetingMessage { get; set; } = string.Empty;
        public DateTime CurrentDateTime { get; set; } = DateTime.Now;

        public string RollNo { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int Batch { get; set; }
        public string Status { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string HomePhone { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public List<EnrolledCourseRow>? EnrolledCourses { get; set; }
    }

    public class EnrolledCourseRow
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SectionLabel { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int Batch { get; set; }
        public int CreditHours { get; set; }
        public string Category { get; set; } = string.Empty;
        public double AttendancePercentage { get; set; }
    }
}
