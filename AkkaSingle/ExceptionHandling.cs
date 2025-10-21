using Akka.Actor;

namespace AkkaSingle
{
    sealed class ParentActor : UntypedActor
    {
        readonly Random r = new Random();
        int child;
        protected override void OnReceive(object message) {

            if ("start".Equals(message))
                Context.ActorOf(Props.Create(() => new RandomBomb()), (++child).ToString())
                       .Tell("run!");
        }

        protected override SupervisorStrategy SupervisorStrategy() =>
            new OneForOneStrategy(maxNrOfRetries: 3, withinTimeMilliseconds: 1000,
              ex => {
                  var isHead = r.Next(2) == 0;
                  Console.WriteLine("Coin is {0}", isHead? "head, restart!" : "tail, stop! Press ENTER to end.");
                  return isHead? Directive.Restart : Directive.Stop;
              });
    }

    sealed class RandomBomb : UntypedActor
    {
        readonly Random r = new Random();
        protected override void OnReceive(object message) {
            if (message is int n)
                Console.WriteLine("1000 / {0} = {1}", n, 1000 / n);

            Self.Tell(r.Next(10));
        }

        protected override void PreStart() {
            Console.WriteLine("Starting...");
        }

        protected override void PostStop() {
            Console.WriteLine("Stopping...");
        }

        protected override void PostRestart(Exception reason) {
            Console.WriteLine($"Restarted! Reason = {reason.Message}");
            Self.Tell("restarted!");
        }
    }

    /// <summary>
    /// A simplified example of error handling
    /// ref: https://github.com/petabridge/akka-bootcamp/blob/master/src/Unit-1/lesson4/README.md
    /// </summary>
    static class ExceptionHandling
    {
        public static void Example(ActorSystem actorSystem) {
            var parent = actorSystem.ActorOf(Props.Create(() => new ParentActor()), "parent");

            parent.Tell("start");

            Console.Write("Wait for bomb..");
            Console.ReadLine();
            actorSystem.Terminate();
        }
    }
}