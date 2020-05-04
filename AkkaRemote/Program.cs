using System;
using System.IO;
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
            system.ActorOf(Props.Create(() => new EchoService()), "echo");

            Console.WriteLine("ENTER to start connect!");
            Console.ReadLine();

            var otherEcho = system.ActorSelection($"akka.tcp://remote-example@localhost:{theirPort}/user/echo");

            Console.WriteLine("Start!");

            var input = "Hi!";

            do {
                otherEcho.Tell(input);
                input = Console.ReadLine();
            } while (!string.IsNullOrEmpty(input));

            Console.WriteLine("End..");
        }
    }
}