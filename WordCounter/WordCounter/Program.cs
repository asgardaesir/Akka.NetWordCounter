using Akka.Actor;

namespace WordCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("WordCounter");
            var wordCounter = actorSystem.ActorOf(Props.Create<CountSupervisor>(), "CountSupervisor");
            wordCounter.Tell(new ProcessFileMessage(@"C:\Users\Brad\Documents\1984.txt"));

            actorSystem.WhenTerminated.Wait();
        }
    }
}
