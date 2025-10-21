using Akka.Actor;
using Akka.Configuration;

namespace AkkaRemote
{
    sealed class EchoService : UntypedActor
    {
        protected override void OnReceive(object message) {
            Console.WriteLine("Echo: {0}", message);
        }
    }

    class Program
    {
        static void Main(string[] args) {
            var myPort = int.Parse(args[0]);
            var theirPort = int.Parse(args[1]);

            var portConfig = ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port = {myPort}");
            var config = ConfigurationFactory.ParseString(File.ReadAllText("app.conf")).WithFallback(portConfig);

            using var system = ActorSystem.Create("remote-example", config);

            IActorRef? echo = null;
            var quit = false;
            while (!quit) {
                switch (Menu()) {
                    case '1':
                        echo = system.ActorOf(Props.Create(() => new EchoService()), "echo");
                        Console.WriteLine("Echo service is created locally.");
                        break;

                    case '2':
                        var remoteAddress = Address.Parse($"akka.tcp://remote-example@localhost:{theirPort}");
                        echo = system.ActorOf(Props.Create(() => new EchoService())
                                                   .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))),
                                              "echo");
                        Console.WriteLine("Echo service is created on a remote machine.");
                        break;

                    case '3':
                        Console.Write("Input: ");
                        echo?.Tell(Console.ReadLine());
                        break;

                    case 'Q':
                        quit = true;
                        break;
                }
            }
            Console.WriteLine("End..");
        }

        static char Menu() {
            Console.WriteLine("Choose:");
            Console.WriteLine("1) Run Echo locally");
            Console.WriteLine("2) Run Echo on remote");
            Console.WriteLine("3) Send text to Echo");
            Console.WriteLine("Q) Quit");
            return Console.ReadKey().KeyChar;
        }
    }
}