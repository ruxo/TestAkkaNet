using System;
using Akka.Actor;
using AkkaSingle.Messages;

namespace AkkaSingle
{
    namespace Messages
    {
        public sealed record Hello(string Message);
        public sealed record Swap;
    }
    
    public sealed class TestActor : ReceiveActor, IWithUnboundedStash
    {
        public TestActor() {
            Normal();
        }

        public IStash Stash { get; set; } = null!;

        void Normal() {
            Receive<Hello>(h => Console.WriteLine($"Hi {h.Message}!"));
            Receive<Swap>(_ => Become(Ghost));
        }

        void Ghost() {
            Receive<Hello>(_ => Stash.Stash());
            Receive<Swap>(_ => {
                Become(Normal);
                Stash.UnstashAll();
            });
        }
    }

    public static class ReceiveActorWithState
    {
        public static void Example(ActorSystem actorSystem) {
            var a = actorSystem.ActorOf(Props.Create<TestActor>(), "test-actor");
            Console.WriteLine("Normal mode");
            a.Tell(new Hello("Hello"));
            a.Tell(new Hello("World"));
            
            Console.WriteLine("Press ENTER to swap");
            Console.ReadLine();
            a.Tell(new Swap());
            
            Console.WriteLine("Ghost mode");
            a.Tell(new Hello("Keep silent"));
            a.Tell(new Hello("message after swap"));
            
            Console.WriteLine("Press ENTER to swap");
            Console.ReadLine();
            a.Tell(new Swap());
            
            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();
            actorSystem.Terminate();
        }
    }
}