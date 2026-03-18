using System;
namespace OmniFlex.Models.Services
{
    public static class SectionHelper
    {
        public static string GenerateSectionId(string deptId, string degreeProgram, int batchYear)
        {
            // Generate a unique section ID based on department, degree program, and batch year
            return $"{deptId}-{degreeProgram.Substring(0, 3).ToUpper()}-{batchYear}";
        }

        public static string GetFormattedSectionLabel(string degree, string label, int batchYear)
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            int yearDiff = currentYear - batchYear;

            // Logic: July-Dec is Odd, Jan-June is Even
            int semester = (currentMonth >= 7) ? (yearDiff * 2) + 1 : (yearDiff * 2);

            if (semester < 1) semester = 1;

            return $"{degree}-{semester}{label}"; // Result: BSCS-3A
        }
    }
}
