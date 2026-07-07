using System.Text.Json.Serialization;

namespace task13
{
    public class Subject
    {
        [JsonPropertyName("subject_name")]
        public string Name { get; set; }

        [JsonPropertyName("grade")]
        public int Grade { get; set; }

        public Subject()
        {
            Name = "";
            Grade = 0;
        }

        public Subject(string Name, int Grade)
        {
            this.Name = Name;
            this.Grade = Grade;
        }
    }
}
