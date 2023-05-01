using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RandomCoffee.Core.Abstractions;
using RandomCoffee.EntityFramework.Models;

namespace RandomCoffee.EntityFramework.Services
{
    /// <summary>
    /// Service for the Random Coffee history storage
    /// </summary>
    public class RandomCoffeeHistorian : IRandomCoffeeHistorian
    {
        private readonly Db _context;
        
        public RandomCoffeeHistorian(Db context)
        {
            _context = context;
        }
        
        /// <inheritdoc />
        public List<List<string[]>> GetRandomCoffeeHistory(int count)
        {
            if (count < 1)
            {
                return new List<List<string[]>>();
            }

            var records = _context.RandomCoffeeRecords
                .OrderByDescending(x => x.TossDate)
                .Select(x => x.TossResult)
                .Take(count)
                .ToList();

            var result = records
                .Select(JsonConvert.DeserializeObject<List<string[]>>)
                .ToList();

            return result;
        }

        /// <inheritdoc />
        public void SaveRandomCoffeeInfo(List<string[]> groups)
        {
            var record = new RandomCoffeeRecord
            {
                TossDate = DateTime.UtcNow,
                TossResult = JsonConvert.SerializeObject(groups)
            };

            _context.RandomCoffeeRecords.Add(record);
            _context.SaveChanges();
        }

        public DateTime? GetLastRandomCoffeeDate()
        {
            return _context.RandomCoffeeRecords
                .OrderByDescending(x => x.TossDate)
                .Select(x => x.TossDate)
                .FirstOrDefault();
        }
    }
}