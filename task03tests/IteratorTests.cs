using Xunit;
using task03;
using System;
using System.Collections.Generic;
using System.Linq;

namespace task03tests
{
    public class IteratorTests
    {
        [Fact]
        public void CustomCollection_GetEnumerator_ReturnsAllItems()
        {
            var collection = new CustomCollection<int>();
            collection.Add(1);
            collection.Add(2);
            var result = new List<int>();
            foreach (var item in collection)
            {
                result.Add(item);
            }
            Assert.Equal(new[] { 1, 2 }, result);
        }

        [Fact]
        public void GetReverseEnumerator_ReturnsItemsInReverseOrder()
        {
            var collection = new CustomCollection<int>();
            collection.Add(1);
            collection.Add(2);
            var result = collection.GetReverseEnumerator().ToList();
            Assert.Equal(new[] { 2, 1 }, result);
        }

        [Fact]
        public void GenerateSequence_ReturnsCorrectSequence()
        {
            var sequence = CustomCollection<int>.GenerateSequence(5, 3).ToList();
            Assert.Equal(new[] { 5, 6, 7 }, sequence);
        }

        [Fact]
        public void FilterAndSort_ReturnsFilteredAndSortedItems()
        {
            var collection = new CustomCollection<int>();
            collection.Add(3);
            collection.Add(1);
            collection.Add(2);
            var result = collection.FilterAndSort(x => x > 1, x => x).ToList();
            Assert.Equal(new[] { 2, 3 }, result);
        }
        [Fact]
        public void Add_WithNullItem_ThrowsException()
        {
            var collection = new CustomCollection<string>();
            //null! для подавления предупреждений(говорит компилятору "не ругайся,я специально передаю null для теста")
            Assert.Throws<ArgumentNullException>(() => collection.Add(null!));
        }

        [Fact]
        public void Remove_WithNullItem_ThrowsException()
        {
            var collection = new CustomCollection<string>();
            Assert.Throws<ArgumentNullException>(() => collection.Remove(null!));
        }

        [Fact]
        public void GenerateSequence_WithNegativeCount_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => CustomCollection<int>.GenerateSequence(5, -1).ToList());
        }

        [Fact]
        public void FilterAndSort_WithNullPredicate_ThrowsException()
        {
            var collection = new CustomCollection<int>();
            collection.Add(1);
            Assert.Throws<ArgumentNullException>(() => collection.FilterAndSort(null!, x => x).ToList());
        }

        [Fact]
        public void FilterAndSort_WithNullKeySelector_ThrowsException()
        {
            var collection = new CustomCollection<int>();
            collection.Add(1);
            Assert.Throws<ArgumentNullException>(() => collection.FilterAndSort(x => true, null!).ToList());
        }
    }
}
