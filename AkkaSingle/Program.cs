using Akka.Actor;

namespace AkkaSingle
{
    class Program
    {
        static readonly ActorSystem ActorSystem = ActorSystem.Create("SingleActor");

        static void Main() {
            //SimpleSetup.Example(ActorSystem);
            ExceptionHandling.Example(ActorSystem);

            ActorSystem.WhenTerminated.Wait();
        }
    }
}