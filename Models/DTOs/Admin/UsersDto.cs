using System;
namespace OmniFlex.Models.DTOs
{
    public class UsersDto
    {
        public string UserId {get; set;} = string.Empty;
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        public string Role {get; set;} = string.Empty;
        public string Department {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string PhoneNumber {get; set;} = string.Empty;

    }
    public class StudentDto
    {
        public string UserId {get; set;} = string.Empty;
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        public string Department {get; set;} = string.Empty;
        public string Gender {get; set;} = string.Empty;
        public DateTime DOB {get; set;}
        public string Address {get; set;} = string.Empty;
        public string City {get; set;} = string.Empty;
        public string Country {get; set;} = string.Empty;   
        public string Email {get; set;} = string.Empty;
        public string PhoneNumber {get; set;} = string.Empty;
        public int BatchYear {get; set;}
        public string DegreeProgram {get; set;} = string.Empty;
        public string Status {get; set;} = string.Empty;

    }
    public class InstructorDto
    {
        public string UserId {get; set;} = string.Empty;
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        public string Department {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string PhoneNumber {get; set;} = string.Empty;
        public string Designation {get; set;} = string.Empty;
        public string OfficeRoom {get; set;} = string.Empty;
        public string Status {get; set;} = string.Empty;

    }
    public class AdminDto
    {
        public string UserId {get; set;} = string.Empty;
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        public string Department {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string PhoneNumber {get; set;} = string.Empty;
        public string Designation {get; set;} = string.Empty;
        public string OfficeRoom {get; set;} = string.Empty;
        public string Status {get; set;} = string.Empty;

    }
}