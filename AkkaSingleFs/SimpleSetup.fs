module SimpleSetup

open System
open Akka.Actor

type ConsoleWriterActor() =
    inherit UntypedActor()
    
    override my.OnReceive message =
        printfn $"Retrieved: %A{message}"
        
type Runner(writer: IActorRef) =
    inherit UntypedActor()
    
    override my.OnReceive _ =
        let text = Console.ReadLine()
        if String.IsNullOrEmpty text then
            // Unlike C#, static member must be explicit with type.
            Runner.Context.System.Terminate() |> ignore
        else
            writer.Tell text
            my.Self.Tell "continue"

let Example (actor_system: ActorSystem) =
    let write_actor = actor_system.ActorOf(Props.Create<ConsoleWriterActor>())
    let runner = actor_system.ActorOf(Props.Create(fun _ -> Runner write_actor))
    
    printfn "Input texts. Just ENTER to end."
    runner.Tell "start"