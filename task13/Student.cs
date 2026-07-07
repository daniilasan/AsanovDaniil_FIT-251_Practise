using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace task13
{
    public class Student
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("birth_date")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("grades")]
        public List<Subject> Grades { get; set; }

        [JsonIgnore]
        public int Age
        {
            get
            {
                DateTime Today = DateTime.Today;
                int Age = Today.Year - BirthDate.Year;

                if (BirthDate.Date > Today.AddYears(-Age))
                {
                    Age--;
                }

                return Age;
            }
        }

        public Student()
        {
            FirstName = "";
            LastName = "";
            BirthDate = DateTime.Today;
            Grades = new List<Subject>();
        }

        public Student(string FirstName, string LastName, DateTime BirthDate, List<Subject> Grades)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.BirthDate = BirthDate;
            this.Grades = Grades;
        }
    }
}
