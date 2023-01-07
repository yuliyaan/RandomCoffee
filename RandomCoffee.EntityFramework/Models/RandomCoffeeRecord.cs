using System;

namespace RandomCoffee.EntityFramework.Models
{
    /// <summary>
    /// One random coffee event info
    /// </summary>
    public class RandomCoffeeRecord
    {
        /// <summary>
        /// Random Coffee event (toss) unique id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Random Coffee toss date and time
        /// </summary>
        public DateTime TossDate { get; set; }
        
        /// <summary>
        /// Serialized toss results
        /// </summary>
        public string TossResult { get; set; }
    }
}