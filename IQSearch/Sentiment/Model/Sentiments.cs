using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQSearch.Sentiment.Model
{
    [Serializable]
    public class Sentiments
    {
        public List<SubSentiment> HighlightToWeightMap { get; set; }

        public int PositiveSentiment { get; set; }

        public int NegativeSentiment { get; set; }

        public int FullSentiment { get; set; }
    }
}
