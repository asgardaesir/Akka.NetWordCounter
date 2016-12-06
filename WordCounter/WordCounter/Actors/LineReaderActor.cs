using System.IO;
using Akka.Actor;
using Akka.Routing;

namespace WordCounter
{
    public class LineReaderActor : ReceiveActor
    {
        private readonly StreamReader _reader;
        private IActorRef _sumActors;
        private readonly int _numberOfWorkers;
        private int _completedWorkers;

        public LineReaderActor(string file, int numberOfWorkers)
        {
            _numberOfWorkers = numberOfWorkers;
            _reader = new StreamReader(file);
            Initalize();
        }

        private void Initalize()
        {
            _sumActors = Context.ActorOf(Props.Create(() => new SumActor()).WithRouter(new RoundRobinPool(_numberOfWorkers)));
            Receive<ProcesLineMessage>(message => ReadLines());
            Receive<ReducedLineResultsMessage>(message => Context.Parent.Tell(message));

            Receive<WorkerCompletedMessage>(message =>
            {
                _completedWorkers++;
                if (_completedWorkers == _numberOfWorkers)
                {
                    Context.Parent.Tell(new Completed());
                }
            });

            Context.Parent.Tell(new LineReaderReadyMessage());
        }

        private void ReadLines()
        {
            while (!_reader.EndOfStream)
            {
                var fileLine = _reader.ReadLine();
                _sumActors.Tell(new ReadLineMessage(fileLine));
            }

            _sumActors.Tell(PoisonPill.Instance);
        }
    }
}
