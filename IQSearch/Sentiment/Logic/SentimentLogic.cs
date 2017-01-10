using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQSearch.Sentiment.Model;
using System.Xml;
using System.Text.RegularExpressions;
using IQLog;

namespace IQSearch.Sentiment.Logic
{
    public class SentimentLogic
    {
        private List<IQSentimentWord> sentimentWords;

        public Dictionary<string, Sentiments> GetSentiment(Dictionary<string, List<string>> p_MapIQCCKeyToListOfHighlight, float p_LowThreshold, float p_HighThreshold, Guid p_ClientGuid)
        {
            IQLogger.BuildDebug("GetSentiment()")
                .SetValue("p_LowThreshold", p_LowThreshold)
                .SetValue("p_HighThreshold", p_HighThreshold)
                .SetValue("p_ClientGuid", p_ClientGuid)
                .Submit();

            Dictionary<string, Sentiments> sentimentsResultList = new Dictionary<string, Sentiments>();
            try
            {
                if (sentimentWords == null)
                {
                    IQSentimentWordModel sentimentWordsModel = new IQSentimentWordModel();
                    sentimentWords = sentimentWordsModel.GetSentimentWordsByClientGuid(p_ClientGuid);
                }
                else
                {
                    IQLogger.Debug("Already have sentiment list.");
                }

                foreach (var iqCCKeyToListOfHighlight in p_MapIQCCKeyToListOfHighlight)
                {
                    List<SubSentiment> subSentiments = new List<SubSentiment>();
                    foreach (string _HighlightText in iqCCKeyToListOfHighlight.Value)
                    {
                        // parse the time offset from the text
                        int? offset = parseOffset(_HighlightText);

                        // remove ns: and <span> tag from fragment
                        string text = Regex.Replace(_HighlightText.ToLower(), "(\\d*)(s:)", string.Empty).Replace("<span class=\"highlight\">", string.Empty).Replace("</span>", string.Empty);

                        // replace all special characters expect ' (signle quote)  to space'
                        text = Regex.Replace(text, "[^0-9a-zA-Z']+", " ");

                        // replace multiple spaces to a signle space.
                        text = Regex.Replace(text, @"\s{2,}", " ");

                        // get the list of words by splitting with <space> , and remove empty words if any. 
                        List<string> wordsInText = text.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                        // find all the words in the text that match sentiment words from the iQ sentiment dictionary
                        List<IQSentimentWord> matchedSentimentWords = sentimentWords.Join(wordsInText, q => q.Word.ToLower(), a => a.ToLower(), (q, a) => q).ToList();

                        // calculate the average weight by summing the values of the matched words and dividing by the total word count from the text
                        double _Weight = matchedSentimentWords.Count > 0 ? (double)Math.Round(Convert.ToDouble(matchedSentimentWords.Sum(s => s.Value)) / wordsInText.Count(), 5) : 0;

                        subSentiments.Add(new SubSentiment() { HighlightingText = _HighlightText, Weight = _Weight });
                    }

                    Sentiments _Sentiment = new Sentiments();
                    _Sentiment.HighlightToWeightMap = subSentiments;
                    _Sentiment.PositiveSentiment = subSentiments.Where(a => a.Weight >= p_HighThreshold).Count();
                    _Sentiment.NegativeSentiment = subSentiments.Where(a => a.Weight <= p_LowThreshold).Count();

                    sentimentsResultList.Add(iqCCKeyToListOfHighlight.Key, _Sentiment);
                }
                return sentimentsResultList;
            }
            catch (Exception ex)
            {
                IQLogger.Event(323, ex);
                return sentimentsResultList;
            }
        }

        public IDictionary<short, Sentiments> GetQuarterHourSentiment(Sentiments hourSentiment, float lowThreshold, float highThreshold)
        {
            var quarters = new Dictionary<short, Sentiments>(4);
            if (hourSentiment.HighlightToWeightMap != null)
            {
                foreach (var sub in hourSentiment.HighlightToWeightMap)
                {
                    int? offset = parseOffset(sub.HighlightingText);
                    if (offset.HasValue)
                    {
                        short startPoint = Convert.ToInt16(offset / 900 + 1);
                        Sentiments quarter;
                        if (!quarters.TryGetValue(startPoint, out quarter))
                        {
                            quarter = new Sentiments();
                            quarter.HighlightToWeightMap = new List<SubSentiment>();
                            quarters.Add(startPoint, quarter);
                        }
                        quarter.HighlightToWeightMap.Add(sub);
                        if (sub.Weight >= highThreshold)
                        {
                            quarter.PositiveSentiment++;
                        }
                        if (sub.Weight <= lowThreshold)
                        {
                            quarter.NegativeSentiment++;
                        }
                    }
                }
            }
            return quarters;
        }

        private int? parseOffset(string text)
        {
            try
            {
                MatchCollection matches = Regex.Matches(text.Substring(0, text.IndexOf("<span class=\"highlight\"")), "(\\d+)s:");
                if (matches.Count > 0)
                {
                    Match last = null;

                    foreach (Match m in matches)
                    {
                        last = m;
                    }
                    if (last != null && last.Groups != null && last.Groups.Count > 1)
                    {
                        return int.Parse(last.Groups[1].Value);
                    }
                }
                int start = text.IndexOf("</span>") + 7;
                Match first = Regex.Match(text.Substring(start, text.Length - start), "(\\d+)s:");
                if (first != null && first.Groups != null && first.Groups.Count > 1)
                {
                    return int.Parse(first.Groups[1].Value) - 1;
                }
            }
            catch (Exception ex)
            {
                IQLogger.BuildDebug("No time offset found in sentiment text.").SetValue("text", text).SetException(ex).Submit();
            }
            return null;
        }
    }
}
