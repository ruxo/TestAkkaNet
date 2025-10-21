using Akka.Actor;

namespace AkkaSingle
{
    class Program
    {
        static readonly ActorSystem ActorSystem = ActorSystem.Create("SingleActor");

        static void Main() {
            Console.WriteLine("Choose an example:");
            Console.WriteLine("1. Simple Setup");
            Console.WriteLine("2. Exception Handling");
            Console.WriteLine("3. ReceiveActor with state");
            Console.Write("> ");
            var choice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (choice) {
                case '1':
                    SimpleSetup.Example(ActorSystem);
                    break;
                case '2':
                    ExceptionHandling.Example(ActorSystem);
                    break;
                case '3':
                    ReceiveActorWithState.Example(ActorSystem);
                    break;
            }
            ActorSystem.WhenTerminated.Wait();
            Console.WriteLine("End.");
        }
    }
}