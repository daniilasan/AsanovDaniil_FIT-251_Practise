using Xunit;
using task13;
using System;
using System.Collections.Generic;
using System.IO;

namespace task13tests
{
    public class JsonSerializationTests
    {
        [Fact]
        public void Serialize_ValidStudent_ReturnsJson()
        {
            JsonHelper Helper = new JsonHelper();
            Student Student = new Student(
                "Иван",
                "Петров",
                new DateTime(2000, 5, 15),
                new List<Subject>
                {
                    new Subject("Математика", 85),
                    new Subject("Физика", 90)
                }
            );

            string Json = Helper.Serialize(Student);

            Assert.NotNull(Json);
            Assert.Contains("first_name", Json);
            Assert.Contains("Иван", Json);
        }

        [Fact]
        public void Deserialize_ValidJson_ReturnsStudent()
        {
            JsonHelper Helper = new JsonHelper();
            string Json = @"{
                ""first_name"": ""Иван"",
                ""last_name"": ""Петров"",
                ""birth_date"": ""2000-05-15"",
                ""grades"": [
                    {""subject_name"": ""Математика"", ""grade"": 85},
                    {""subject_name"": ""Физика"", ""grade"": 90}
                ]
            }";

            Student Result = Helper.Deserialize(Json);

            Assert.Equal("Иван", Result.FirstName);
            Assert.Equal("Петров", Result.LastName);
            Assert.Equal(2, Result.Grades.Count);
            Assert.Equal(85, Result.Grades[0].Grade);
        }

        [Fact]
        public void SaveAndLoad_FileRoundTrip_Works()
        {
            JsonHelper Helper = new JsonHelper();
            string FilePath = Path.Combine(Path.GetTempPath(), "test_student_" + Guid.NewGuid().ToString() + ".json");

            Student Original = new Student(
                "Анна",
                "Сидорова",
                new DateTime(2001, 3, 20),
                new List<Subject>
                {
                    new Subject("История", 78)
                }
            );

            Helper.SaveToFile(Original, FilePath);
            Student Loaded = Helper.LoadFromFile(FilePath);

            Assert.Equal(Original.FirstName, Loaded.FirstName);
            Assert.Equal(Original.LastName, Loaded.LastName);
            Assert.Equal(Original.BirthDate.Date, Loaded.BirthDate.Date);
            Assert.Single(Loaded.Grades);

            File.Delete(FilePath);
        }

        [Fact]
        public void Deserialize_InvalidBirthDate_ThrowsException()
        {
            JsonHelper Helper = new JsonHelper();
            string Json = @"{
                ""first_name"": ""Иван"",
                ""last_name"": ""Петров"",
                ""birth_date"": ""2050-05-15"",
                ""grades"": []
            }";

            Assert.Throws<InvalidOperationException>(() => Helper.Deserialize(Json));
        }

        [Fact]
        public void Deserialize_InvalidGrade_ThrowsException()
        {
            JsonHelper Helper = new JsonHelper();
            string Json = @"{
                ""first_name"": ""Иван"",
                ""last_name"": ""Петров"",
                ""birth_date"": ""2000-05-15"",
                ""grades"": [
                    {""subject_name"": ""Математика"", ""grade"": 150}
                ]
            }";

            Assert.Throws<InvalidOperationException>(() => Helper.Deserialize(Json));
        }

        [Fact]
        public void Serialize_NullStudent_ThrowsException()
        {
            JsonHelper Helper = new JsonHelper();

            Assert.Throws<ArgumentNullException>(() => Helper.Serialize(null!));
        }

        [Fact]
        public void Deserialize_EmptyString_ThrowsException()
        {
            JsonHelper Helper = new JsonHelper();

            Assert.Throws<ArgumentException>(() => Helper.Deserialize(""));
        }

        [Fact]
        public void LoadFromFile_NonExistentFile_ThrowsException()
        {
            JsonHelper Helper = new JsonHelper(); 
            string FilePath = Path.Combine(Path.GetTempPath(), "nonexistent_" + Guid.NewGuid().ToString() + ".json");

            Assert.Throws<FileNotFoundException>(() => Helper.LoadFromFile(FilePath));
        }
    }
}
