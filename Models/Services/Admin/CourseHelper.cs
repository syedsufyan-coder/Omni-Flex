using System;
namespace OmniFlex.Models.Services
{
    public static class CourseHelper
    {
        /// <summary>
        /// @param dept: Department code (e.g., "CS") will be given/handled by the caller based on user input.
        /// @param level: Course level (valid values: "1", "2", "3", "4") will be given/handled by the caller based on user input.
        /// @param maxCourseId: The current maximum course ID for the given department and level (e.g., "CS2001"). This will be retrieved from the database by the caller before calling this method.
        /// @return: A new unique course ID in the format Dept + Level + 3-digit sequence (e.g., "CS1002"). The sequence is incremented from the maxCourseId. If maxCourseId is "CS1999", it will throw an exception since it cannot generate a new ID.
        /// </summary>
        public static string GenerateCourseId(string dept, string level, string maxCourseId)
        {
            // 1. Extract the last 3 digits (e.g., from "CS2001", get "001")
            string CourseNum = maxCourseId.Substring(maxCourseId.Length - 3);

            // 2. Convert to int and increment (e.g., 1 + 1 = 2)
            int NewNumber = int.Parse(CourseNum) + 1;

            if (NewNumber > 999)
            {
                throw new OverflowException($"Cannot generate ID: Sequence for {dept}{level} has reached the maximum (999).");
            }

            // 3. Format back to 3 digits with leading zeros (e.g., 2 becomes "002")
            string formattedNumber = NewNumber.ToString("D3");

            // 4. Combine: Dept + Level + New Sequence
            // Result: e.g. "CS" + "1" + "002" = "CS1002"
            return $"{dept}{level}{formattedNumber}";
        }
        public static string GetFullDegreeName(string degree)
        {
            return degree switch
            {
                "CS" => "Bachelors in Computer Science",
                "CSE" => "Bachelors in Computer Engineering",
                "EE" => "Bachelors in Electrical Engineering",
                "BA" => "Bachelors in Business Administration",
                "CY" => "Bachelors in Cybersecurity",
                "AI" => "Bachelors in Artificial Intelligence",
                "DS" => "Bachelors in Data Science",
                "IS" => "Bachelors in Information Systems",
                "SE" => "Bachelors in Software Engineering",
                "IT" => "Bachelors in Information Technology",
                "CE" => "Bachelors in Civil Engineering",
                "ME" => "Bachelors in Mechanical Engineering",
                _ => degree // Return the original code if no match is found
            };
        }
    }
}
