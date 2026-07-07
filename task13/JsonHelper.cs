using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

namespace task13
{
    public class JsonHelper
    {
        private JsonSerializerOptions Options;

        public JsonHelper()
        {
            Options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public string Serialize(Student Student)
        {
            if (Student == null)
            {
                throw new ArgumentNullException("Student", "Student cannot be null");
            }

            return JsonSerializer.Serialize(Student, Options);
        }

        public Student Deserialize(string Json)
        {
            if (string.IsNullOrEmpty(Json))
            {
                throw new ArgumentException("JSON string cannot be null or empty", "Json");
            }

            Student? Result = JsonSerializer.Deserialize<Student>(Json, Options);

            if (Result == null)
            {
                throw new InvalidOperationException("Failed to deserialize JSON");
            }

            ValidateStudent(Result);

            return Result;
        }

        public void SaveToFile(Student Student, string FilePath)
        {
            if (Student == null)
            {
                throw new ArgumentNullException("Student", "Student cannot be null");
            }

            if (string.IsNullOrEmpty(FilePath))
            {
                throw new ArgumentException("File path cannot be null or empty", "FilePath");
            }

            string Json = Serialize(Student);
            File.WriteAllText(FilePath, Json);
        }

        public Student LoadFromFile(string FilePath)
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                throw new ArgumentException("File path cannot be null or empty", "FilePath");
            }

            if (File.Exists(FilePath) == false)
            {
                throw new FileNotFoundException("File not found", FilePath);
            }

            string Json = File.ReadAllText(FilePath);
            return Deserialize(Json);
        }

        private void ValidateStudent(Student Student)
        {
            if (string.IsNullOrEmpty(Student.FirstName))
            {
                throw new InvalidOperationException("FirstName cannot be null or empty");
            }

            if (string.IsNullOrEmpty(Student.LastName))
            {
                throw new InvalidOperationException("LastName cannot be null or empty");
            }

            if (Student.BirthDate > DateTime.Today)
            {
                throw new InvalidOperationException("BirthDate cannot be in the future");
            }

            if (Student.BirthDate < new DateTime(1900, 1, 1))
            {
                throw new InvalidOperationException("BirthDate is too old");
            }

            if (Student.Grades == null)
            {
                throw new InvalidOperationException("Grades list cannot be null");
            }

            foreach (Subject Subj in Student.Grades)
            {
                if (Subj.Grade < 0 || Subj.Grade > 100)
                {
                    throw new InvalidOperationException("Grade must be between 0 and 100");
                }

                if (string.IsNullOrEmpty(Subj.Name))
                {
                    throw new InvalidOperationException("Subject name cannot be null or empty");
                }
            }
        }
    }
}
