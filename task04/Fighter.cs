using System;

namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed => 100;
        public int FirePower => 30;

        public void MoveForward()
        {
            Console.WriteLine("Fighter moves forward quickly.");
        }

        public void Rotate(int angle)
        {
            Console.WriteLine($"Fighter rotates sharply {angle} degrees.");
        }

        public void Fire()
        {
            Console.WriteLine("Fighter fires a weak rocket.");
        }
    }
}
