using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;

namespace AkkaCluster
{
    sealed class EchoService : UntypedActor
    {
        static int instanceCounter;
        readonly int id;

        public EchoService() {
            id = Interlocked.Increment(ref instanceCounter);
            Console.WriteLine("START Echo service #{0}!", id);
        }

        protected override void PreStart() {
            base.PreStart();
            Console.WriteLine("Echo #{0} ref = {1}", id, Context.Self.Path);
        }

        protected override void OnReceive(object message) {
            Console.WriteLine("Echo [{1}]: {0}", message, id);
        }

        protected override void PostStop() {
            base.PostStop();
            Console.WriteLine("STOP Echo service #{0}.", id);
        }
    }

    class Program
    {
        static async Task Main(string[] args) {
            var myPort = int.Parse(args[0]);

            var portConfig = ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port = {myPort}");
            var config = ConfigurationFactory.ParseString(File.ReadAllText("app.conf")).WithFallback(portConfig);

            using var system = ActorSystem.Create("MyCluster", config);

            var broadcaster = system.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "broadcaster");

            var quit = false;
            IActorRef echo = ActorRefs.Nobody;
            while (!quit) {
                switch (Menu()) {
                    case '1':
                        echo = system.ActorOf(Props.Create(typeof(EchoService)).WithRouter(FromConfig.Instance), "echo");
                        Console.WriteLine("Echo service is created.");
                        break;

                    case '2':
                        Console.Write("Input: ");
                        echo.Tell(Console.ReadLine());
                        break;

                    case '3':
                        Console.Write("Broadcast: ");
                        broadcaster.Tell(Console.ReadLine());
                        break;

                    case 'Q':
                        quit = true;
                        break;
                }
            }

            await CoordinatedShutdown.Get(system).Run(CoordinatedShutdown.ClusterLeavingReason.Instance);

            Console.WriteLine("End..");
        }

        static char Menu() {
            Console.WriteLine("Choose:");
            Console.WriteLine("1) Run Echo in every instance");
            Console.WriteLine("2) Send text to Echo");
            Console.WriteLine("3) Broadcast text to Echo");
            Console.WriteLine("Q) Quit");
            return char.ToUpper(Console.ReadKey().KeyChar);
        }
    }
}