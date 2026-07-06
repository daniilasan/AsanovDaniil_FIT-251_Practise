using System.Collections.Generic;

namespace task02
{
    public class Student
    {
        public string Name { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public List<int> Grades { get; set; } = new List<int>();//сделал так потому что в наших тестах вылазит ошибки из-за того что dotnet 8+ строго следил за null значениями
    }
}
