using Akka.Actor;

namespace AkkaSingle
{
    sealed class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message) {
            Console.WriteLine("Retrieved: {0}", message);
        }
    }

    sealed class Runner : UntypedActor
    {
        readonly IActorRef writer;
        public Runner(IActorRef writer) {
            this.writer = writer;
        }

        protected override void OnReceive(object message) {
            var text = Console.ReadLine();
            if (string.IsNullOrEmpty(text)) {
                Context.System.Terminate();
                return;
            }

            writer.Tell(text);

            Self.Tell("continue");
        }
    }

    // Basically, this is a short version of Akka Bootcamp unit-1
    // ref: https://github.com/petabridge/akka-bootcamp/blob/master/src/Unit-1/lesson1/README.md
    static class SimpleSetup
    {
        public static void Example(ActorSystem actorSystem) {
            var writeActor = actorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var runner = actorSystem.ActorOf(Props.Create(() => new Runner(writeActor)));

            Console.WriteLine("Input texts. Just ENTER to end.");
            runner.Tell("start");
        }
    }
}