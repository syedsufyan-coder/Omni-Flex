using System;

namespace OmniFlex.Models.Services
{
    public static class UserHelper
    {
        public static string GenerateStudentId(int batchYear, int maxId)
        {
            // Example: batchYear = 2024, maxId = 15 -> StudentId = "24K-0016"
            string StudentId = (batchYear % 100).ToString();
            StudentId += "K-" + (maxId + 1).ToString("D4");
            return StudentId;
        }

        public static string GenerateFacultyId(int maxId)
        {
            // Example: maxId = 15 -> FacultyId = "F-0016"
            string FacultyId = "F-" + (maxId + 1).ToString("D4");
            return FacultyId;
        }

        public static string GenerateAdminId(int maxId)
        {
            // Example: maxId = 15 -> AdminId = "A-0016"
            string AdminId = "A-" + (maxId + 1).ToString("D4");
            return AdminId;
        }
    }
}