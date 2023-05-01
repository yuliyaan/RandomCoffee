using System;
using System.Collections.Generic;
using System.Linq;
using RandomCoffee.Core.Abstractions;

namespace RandomCoffee.Tests.Fakes
{
    /// <summary>
    /// Fake implementation of IRandomCoffeeHistorian for RandomCoffeeService testing
    /// </summary>
    public class FakeHistorian : IRandomCoffeeHistorian
    {
        private readonly List<List<string[]>> _storage = new List<List<string[]>>();
        
        public List<List<string[]>> GetRandomCoffeeHistory(int count)
        {
            return Enumerable.Reverse(_storage).Take(count).ToList();
        }

        public void SaveRandomCoffeeInfo(List<string[]> groups)
        {
            _storage.Add(groups);
        }

        public DateTime? GetLastRandomCoffeeDate()
        {
            throw new NotImplementedException();
        }
    }
}