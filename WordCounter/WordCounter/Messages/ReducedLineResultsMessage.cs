using System.Collections.Generic;

namespace WordCounter
{
    public class ReducedLineResultsMessage
    {
        public Dictionary<string, int> WordSums { get; private set; } 
        public ReducedLineResultsMessage(Dictionary<string, int> wordSums)
        {
            WordSums = wordSums;
        }
    }
}