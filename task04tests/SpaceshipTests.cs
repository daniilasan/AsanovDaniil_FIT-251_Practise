using Xunit;
using task04;

namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            ISpaceship cruiser = new Cruiser();
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }

        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            ISpaceship fighter = new Fighter();
            Assert.Equal(100, fighter.Speed);
            Assert.True(fighter.FirePower < 100); //Должен быть слабее крейсера
        }

        [Fact]
        public void Spaceships_ShouldImplementInterface()
        {
            ISpaceship ship1 = new Cruiser();
            ISpaceship ship2 = new Fighter();

            Assert.IsType<Cruiser>(ship1);
            Assert.IsType<Fighter>(ship2);
        }

        [Fact]
        public void Ships_CanPerformActionsWithoutExceptions()
        {
            ISpaceship cruiser = new Cruiser();
            ISpaceship fighter = new Fighter();

            // Просто вызываем методы, чтобы убедиться, что они работают и не падают
            cruiser.MoveForward();
            cruiser.Rotate(90);
            cruiser.Fire();

            fighter.MoveForward();
            fighter.Rotate(45);
            fighter.Fire();
        }
    }
}
