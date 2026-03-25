using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.ViewModels.Student;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Services;
using OmniFlex.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using OmniFlex.Models.Domain.Admin;
using System.Threading.Tasks;

namespace OmniFlex.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUserRepository _users;
        private readonly IEnrollmentRepository _enrollments;

        public StudentController(IUserRepository users, IEnrollmentRepository enrollments)
        {
            _users = users;
            _enrollments = enrollments;
        }

        private string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            return hour < 12 ? "Good morning"
                 : hour < 17 ? "Good afternoon"
                 : "Good evening";
        }

        // TO-DO: Remove hardcoded UserId and get from User.Identity once authentication is implemented
        // TO-DO: Handle case when user is not found (null) and show appropriate message or redirect
        // TO-DO: Remove -Theory concatenation from course names in and handle it in view instead based on credit hours or course type
        public async Task<IActionResult> Dashboard()
        {
            // 1. Get the current User ID (Hardcoded for now, or get from User.Identity)
            string userId = "U010";

            // 2. Fetch data from your service/repository
            var studentDetails = await _users.GetDetailsByIdAsync(userId);
            var enrolledCourses = await _users.GetEnrolledCoursesAsync(userId);

            if (studentDetails == null)
            {
                return NotFound();
            }

            // 3. Map Data to DashboardViewModel
            var model = new DashboardViewModel
            {
                // Student Info
                StudentName = $"{studentDetails.FirstName} {studentDetails.LastName}",
                FullName = $"{studentDetails.FirstName} {studentDetails.LastName}",
                GreetingMessage = GetGreeting(),
                RollNo = studentDetails.UserId,
                Status = studentDetails.Status,
                BloodGroup = 0.ToString(), // Placeholder, as it's not in StudentDto
                Nationality = "Pakistani", // Placeholder, as it's not in StudentDto

                // Academic Info
                Degree = studentDetails.DegreeProgram,
                Batch = studentDetails.BatchYear,

                // Contact Info
                Email = studentDetails.Email,
                Gender = studentDetails.Gender,
                DateOfBirth = studentDetails.DOB.ToString("dd-MMM-yyyy"),
                MobileNo = studentDetails.PhoneNumber,
                Address = studentDetails.Address,
                City = studentDetails.City,
                Country = studentDetails.Country,

                // Courses List
                EnrolledCourses = enrolledCourses,

                // Additional Info (Map from first course if available, or leave defaults)
                Section = enrolledCourses.FirstOrDefault()?.SectionLabel.ToString() ?? "N/A"
            };
            if(model.EnrolledCourses != null)
            {
                foreach(var course in model.EnrolledCourses)
                {
                    course.SectionLabel = SectionHelper.GetFormattedSectionLabel(course.Degree, course.SectionLabel.ToString(), course.Batch);
                    course.CourseName += course.CreditHours == 1 ? " - Lab" : " - Theory";
                }
                model.Section = model.EnrolledCourses.FirstOrDefault()?.SectionLabel ?? "N/A";
            }

            ViewData["ActivePage"] = "Dashboard";
            ViewData["PageTitle"] = "Dashboard";

            return View(model);
        }

        public IActionResult CourseRegistration()
        {
            ViewData["ActivePage"] = "CourseRegistration";
            ViewData["PageTitle"] = "Course Registration";
            return View();
        }

        public async Task<IActionResult> Enrolled()
        {
            // 1. Get the current User ID (Hardcoded for now, or get from User.Identity)
            string userId = "U010";

            var enrolledCourses = await _enrollments.GetEnrolledClassesAsync(userId);

            var model = new EnrolledViewModel
            {
                ActiveClasses = enrolledCourses
            };
            if(model.ActiveClasses != null)
            {
                foreach (var course in model.ActiveClasses)
                {
                    course.Section = SectionHelper.GetFormattedSectionLabel(course.Degree, course.Section, course.Batch);
                    course.BannerColorClass = EnrollmentHelper.GetRandomDarkHex();
                }
            }
            ViewData["ActivePage"] = "Enrolled";
            ViewData["PageTitle"] = "Enrolled";
            return View(model);
        }

        public IActionResult ToDo()
        {
            ViewData["ActivePage"] = "ToDo";
            ViewData["PageTitle"] = "TO-DO";
            var model = new ToDoViewModel
            {
                AssignedItems = new List<ToDoItem>
                {
                    new() { CourseCode="CS3012", CourseName="Software Engineering", CourseColorClass="bg-primary", AssignmentTitle="SRS Document", DueDate=DateTime.Now.AddDays(2), Status="Assigned" },
                    new() { CourseCode="CS3014", CourseName="Database Systems", CourseColorClass="bg-success", AssignmentTitle="ER Diagram", DueDate=DateTime.Now.AddDays(3), Status="Assigned" },
                    new() { CourseCode="CS3016", CourseName="Operating Systems", CourseColorClass="bg-warning", AssignmentTitle="Process Scheduling", DueDate=DateTime.Now.AddDays(5), Status="Assigned" }
                },
                MissingItems = new List<ToDoItem>
                {
                    new() { CourseCode="CL3012", CourseName="Software Engineering Lab", CourseColorClass="bg-danger", AssignmentTitle="Unit Testing", DueDate=DateTime.Now.AddDays(-1), Status="Missing" },
                    new() { CourseCode="CS3018", CourseName="Computer Networks", CourseColorClass="bg-info", AssignmentTitle="Subnetting Quiz", DueDate=DateTime.Now.AddDays(-2), Status="Missing" }
                },
                DoneItems = new List<ToDoItem>
                {
                    new() { CourseCode="CS3012", CourseName="Software Engineering", CourseColorClass="bg-primary", AssignmentTitle="Version Control Setup", DueDate=DateTime.Now.AddDays(-10), Status="Done" },
                    new() { CourseCode="CS3014", CourseName="Database Systems", CourseColorClass="bg-success", AssignmentTitle="Normalization Exercise", DueDate=DateTime.Now.AddDays(-8), Status="Done" },
                    new() { CourseCode="CS3016", CourseName="Operating Systems", CourseColorClass="bg-warning", AssignmentTitle="Memory Management", DueDate=DateTime.Now.AddDays(-6), Status="Done" },
                    new() { CourseCode="CS3018", CourseName="Computer Networks", CourseColorClass="bg-info", AssignmentTitle="Network Layers", DueDate=DateTime.Now.AddDays(-4), Status="Done" }
                }
            };
            return View(model);
        }

        public IActionResult Calendar()
        {
            ViewData["ActivePage"] = "Calendar";
            ViewData["PageTitle"] = "Calendar";
            var now = DateTime.Now;
            var model = new CalendarViewModel
            {
                CurrentYear = now.Year,
                CurrentMonth = now.Month,
                Events = new List<CalendarEvent>
                {
                    new() { Title="SRS Deadline", DueDate= new DateTime(now.Year, now.Month, 5), CourseCode="CS3012", ColorClass="bg-primary", Type="Assignment" },
                    new() { Title="Database Quiz", DueDate= new DateTime(now.Year, now.Month, 9), CourseCode="CS3014", ColorClass="bg-success", Type="Quiz" },
                    new() { Title="OS Lab", DueDate= new DateTime(now.Year, now.Month, 14), CourseCode="CS3016", ColorClass="bg-warning text-dark", Type="Lab" },
                    new() { Title="Networks Test", DueDate= new DateTime(now.Year, now.Month, 21), CourseCode="CS3018", ColorClass="bg-danger", Type="Exam" },
                    new() { Title="Midterm", DueDate= new DateTime(now.Year, now.Month, 25), CourseCode="CS3012", ColorClass="bg-info", Type="Exam" },
                    new() { Title="Final Project", DueDate= new DateTime(now.Year, now.Month, 28), CourseCode="CL3012", ColorClass="bg-secondary", Type="Assignment" }
                }
            };
            return View(model);
        }

        public IActionResult Attendance()
        {
            ViewData["ActivePage"] = "Attendance";
            ViewData["PageTitle"] = "Attendance";
            var model = new AttendanceViewModel
            {
                Courses = new List<CourseAttendance>
                {
                    new() { CourseCode="CS3012", CourseName="Software Engineering", Section="BCS-2G", TotalClasses=30, ClassesAttended=26, ClassesMissed=4, AttendancePercentage=86.7,
                        Records=new List<AttendanceRecord>
                        {
                            new(){ Date=DateTime.Now.AddDays(-1), Day="Tuesday", Status="Present"},
                            new(){ Date=DateTime.Now.AddDays(-2), Day="Monday", Status="Absent"},
                            new(){ Date=DateTime.Now.AddDays(-3), Day="Sunday", Status="Present"}
                        }
                    },
                    new() { CourseCode="CS3014", CourseName="Database Systems", Section="BCS-2G", TotalClasses=30, ClassesAttended=21, ClassesMissed=9, AttendancePercentage=70.0,
                        Records=new List<AttendanceRecord>
                        {
                            new(){ Date=DateTime.Now.AddDays(-1), Day="Tuesday", Status="Present"},
                            new(){ Date=DateTime.Now.AddDays(-2), Day="Monday", Status="Present"},
                            new(){ Date=DateTime.Now.AddDays(-3), Day="Sunday", Status="Absent"}
                        }
                    },
                    new() { CourseCode="CS3016", CourseName="Operating Systems", Section="BCS-2G", TotalClasses=30, ClassesAttended=17, ClassesMissed=13, AttendancePercentage=56.7,
                        Records=new List<AttendanceRecord>
                        {
                            new(){ Date=DateTime.Now.AddDays(-1), Day="Tuesday", Status="Absent"},
                            new(){ Date=DateTime.Now.AddDays(-2), Day="Monday", Status="Present"},
                            new(){ Date=DateTime.Now.AddDays(-3), Day="Sunday", Status="Absent"}
                        }
                    }
                }
            };
            return View(model);
        }

        public IActionResult Marks()
        {
            ViewData["ActivePage"] = "Marks";
            ViewData["PageTitle"] = "Marks";
            var model = new MarksViewModel
            {
                Courses = new List<CourseMarks>
                {
                    new() { CourseCode="CS3012", CourseName="Software Engineering", Section="BCS-2G", EnrollmentStatus="Enrolled",
                        Assessments = new List<AssessmentMark>
                        {
                            new() { Component="Quiz 1", ObtainedMarks=18, TotalMarks=20, Remarks="Good" },
                            new() { Component="Mid", ObtainedMarks=45, TotalMarks=50, Remarks="Very Good" },
                            new() { Component="Final", ObtainedMarks=80, TotalMarks=100, Remarks="Excellent" }
                        },
                        TotalObtained=143, TotalMax=170
                    },
                    new() { CourseCode="CS3014", CourseName="Database Systems", Section="BCS-2G", EnrollmentStatus="Registered",
                        Assessments = new List<AssessmentMark>
                        {
                            new() { Component="Quiz 1", ObtainedMarks=15, TotalMarks=20, Remarks="Satisfactory" },
                            new() { Component="Mid", ObtainedMarks=40, TotalMarks=50, Remarks="Good" },
                            new() { Component="Final", ObtainedMarks=70, TotalMarks=100, Remarks="Good" }
                        },
                        TotalObtained=125, TotalMax=170
                    },
                    new() { CourseCode="CS3016", CourseName="Operating Systems", Section="BCS-2G", EnrollmentStatus="Enrolled",
                        Assessments = new List<AssessmentMark>
                        {
                            new() { Component="Quiz 1", ObtainedMarks=12, TotalMarks=20, Remarks="Needs Improvement" },
                            new() { Component="Mid", ObtainedMarks=34, TotalMarks=50, Remarks="Average" },
                            new() { Component="Final", ObtainedMarks=60, TotalMarks=100, Remarks="Satisfactory" }
                        },
                        TotalObtained=106, TotalMax=170
                    }
                }
            };
            return View(model);
        }

        public IActionResult Transcript()
        {
            ViewData["ActivePage"] = "Transcript";
            ViewData["PageTitle"] = "Transcript";
            var model = new TranscriptViewModel
            {
                RollNo = "22K-4567",
                StudentName = "Ahmed Raza",
                Degree = "BS Computer Science",
                Batch = "2022",
                Program = "Undergraduate",
                Semesters = new List<SemesterTranscript>
                {
                    new() {
                        SemesterLabel = "Spring 2023",
                        CreditHoursAttempted = 15,
                        CreditHoursEarned = 15,
                        SGPA = 3.75,
                        CGPA = 3.75,
                        Courses = new List<TranscriptCourse>
                        {
                            new(){ Code="CS2001", CourseName="Data Structures", Section="BCS-1A", CreditHours=3, Grade="A", GradePoints=4.0, Type="Core" },
                            new(){ Code="CS2002", CourseName="Discrete Math", Section="BCS-1A", CreditHours=3, Grade="A-", GradePoints=3.7, Type="Core" },
                            new(){ Code="CS2003", CourseName="Calculus", Section="BCS-1A", CreditHours=3, Grade="B+", GradePoints=3.3, Type="Core" }
                        }
                    },
                    new() {
                        SemesterLabel = "Fall 2023",
                        CreditHoursAttempted = 16,
                        CreditHoursEarned = 16,
                        SGPA = 3.85,
                        CGPA = 3.80,
                        Courses = new List<TranscriptCourse>
                        {
                            new(){ Code="CS3001", CourseName="Algorithms", Section="BCS-2A", CreditHours=3, Grade="A", GradePoints=4.0, Type="Core" },
                            new(){ Code="CS3002", CourseName="Object Oriented", Section="BCS-2A", CreditHours=3, Grade="A", GradePoints=4.0, Type="Core" },
                            new(){ Code="CS3003", CourseName="Statistics", Section="BCS-2A", CreditHours=3, Grade="B+", GradePoints=3.3, Type="Core" }
                        }
                    },
                    new() {
                        SemesterLabel = "Spring 2024",
                        CreditHoursAttempted = 15,
                        CreditHoursEarned = 15,
                        SGPA = 3.90,
                        CGPA = 3.82,
                        Courses = new List<TranscriptCourse>
                        {
                            new(){ Code="CS3012", CourseName="Software Engineering", Section="BCS-2G", CreditHours=3, Grade="A", GradePoints=4.0, Type="Core" },
                            new(){ Code="CS3014", CourseName="Database Systems", Section="BCS-2G", CreditHours=3, Grade="A-", GradePoints=3.7, Type="Core" },
                            new(){ Code="CS3016", CourseName="Operating Systems", Section="BCS-2G", CreditHours=3, Grade="B", GradePoints=3.0, Type="Core" }
                        }
                    }
                }
            };
            return View(model);
        }

        public IActionResult ArchivedClasses()
        {
            ViewData["ActivePage"] = "ArchivedClasses";
            ViewData["PageTitle"] = "Archived Classes";
            var model = new EnrolledViewModel
            {
                ActiveClasses = new List<ClassCard>
                {
                    new() { CourseCode="CS2010", CourseName="Intro to Python", TeacherName="Ms. Nadia Shah", Section="BCS-1G", BannerColorClass="#6f42c1", IsArchived=true },
                    new() { CourseCode="CS2020", CourseName="Web Technologies", TeacherName="Mr. Salman Rehman", Section="BCS-1G", BannerColorClass="#fd7e14", IsArchived=true }
                }
            };
            return View(model);
        }

        public IActionResult Settings()
        {
            ViewData["ActivePage"] = "Settings";
            ViewData["PageTitle"] = "Settings";
            return View();
        }
    }
}
