using System;
using System.Collections.Generic;
using Akka.Actor;

namespace WordCounter
{
    public class SumActor : ReceiveActor
    {
        public SumActor()
        {
            Initalize();
        }

        private void Initalize()
        {
            Receive<ReadLineMessage>(message =>
            {
                var wordArray = message.LineFromFile.StripPunctuation().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var wordCount = new Dictionary<string, int>();

                foreach (var key in wordArray)
                {
                    if (wordCount.ContainsKey(key))
                    {
                        wordCount[key]++;
                    }
                    else
                    {
                        wordCount.Add(key, 1);
                    }
                }

                Context.Sender.Tell(new ReducedLineResultsMessage(wordCount));
            });
        }

        protected override void PostStop()
        {
            Context.Sender.Tell(new WorkerCompletedMessage());
        }
    }
}