using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQSearch.Sentiment.Model;

namespace IQSearch
{
    public class QuarterHit
    {
        /// <summary>
        /// The start point for the quarter hour (1-4)
        /// </summary>
        public short StartPoint { get; set; }

        /// <summary>
        /// Number of occurrences in the quarter hour
        /// </summary>
        public int TotalNoOfOccurrence { get; set; }

        /// <summary>
        /// A list of all occurrences of the search term in the quarter hour
        /// </summary>
        public List<TermOccurrence> TermOccurrences { get; set; }

        /// <summary>
        /// Sentiment weight values of each occurance and sentiment counts for the quarter hour
        /// </summary>
        public Sentiments Sentiments { get; set; }

        /// <summary>
        /// The show title corresponding to the first occurance in the quarter hour
        /// </summary>
        public string FirstTitle120 { get; set; }

        /// <summary>
        /// The iQ Class corresponding to the first occurance in the quarter hour
        /// </summary>
        public string FirstIQClass { get; set; }

        public QuarterHit(short startPoint)
        {
            StartPoint = startPoint;
            TotalNoOfOccurrence = 0;
            TermOccurrences = new List<TermOccurrence>();
        }
    }
}
