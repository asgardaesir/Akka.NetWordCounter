using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Akka.Actor;

namespace WordCounter
{
    public class CountSupervisor : ReceiveActor
    {
        private readonly Dictionary<string, int> _wordCounts = new Dictionary<string, int>();
        private IActorRef _lineReaderActor;
        private const int NumberOfWorkers = 10;
        private readonly Stopwatch _timer;

        public CountSupervisor()
        {
            Initalize();
            _timer = new Stopwatch();
            _timer.Start();
        }

        private void Initalize()
        {
            Receive<ProcessFileMessage>(message =>
            {
                _lineReaderActor = Context.ActorOf(Props.Create(() => new LineReaderActor(message.File, NumberOfWorkers)), "LineReader");
                _lineReaderActor.Tell(message, Self);
            });

            Receive<ReducedLineResultsMessage>(message => CalcuateWordCount(message.WordSums));

            Receive<Completed>(completed =>
            {
                _timer.Stop();

                using (var writer = new StreamWriter(string.Format(@"C:\Users\Brad\Documents\results_{0}.txt", DateTime.Now.Ticks)))
                {
                    writer.WriteLine("{0} Milliseconds to process file.", _timer.ElapsedMilliseconds);
                    writer.WriteLine("=====================================================");

                    foreach (var wordCount in _wordCounts.OrderByDescending(pair => pair.Value))
                    {
                        writer.WriteLine("{0} -{1}", wordCount.Key, wordCount.Value);
                    }

                    Console.WriteLine("=================================");
                    Console.WriteLine("Completed file processing took {0} Milliseconds", _timer.ElapsedMilliseconds);
                    Console.WriteLine("=================================");
                    _lineReaderActor.Tell(PoisonPill.Instance);
                    Context.Stop(Self);
                }
            });

            Receive<LineReaderReadyMessage>(message =>
            {
                _lineReaderActor.Tell(new ProcesLineMessage());
            });

        }

        private void CalcuateWordCount(Dictionary<string, int> wordCounts)
        {
            foreach (var wordSum in wordCounts)
            {
                if (_wordCounts.ContainsKey(wordSum.Key))
                {
                    _wordCounts[wordSum.Key] += wordSum.Value;
                }
                else
                {
                    _wordCounts.Add(wordSum.Key, wordSum.Value);
                }
            }
        }

    }

    public class Completed
    {
        
    }
}
