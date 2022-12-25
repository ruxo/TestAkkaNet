module ExceptionHandling

open System
open Akka.Actor

type RandomBomb() =
    inherit UntypedActor()
    
    let r = Random()
    
    override my.OnReceive message =
        match message with
        | :? int as n -> printfn $"1000 / %d{n} = %d{1000/n}"
        | _ -> ()
        my.Self.Tell(r.Next 10)
        
    override my.PreStart() =
        printfn "Starting..."
        
    override my.PostStop() =
        printfn "Stopping..."
        
    override my.PostRestart reason =
        printfn $"Restarted! Reason = %s{reason.Message}"
        my.Self.Tell "restarted!"

type ParentActor() =
    inherit UntypedActor()
    
    let r = Random()
    let mutable child = 0
    
    override my.OnReceive message =
        if "start".Equals(message) then
            child <- child + 1
            ParentActor.Context.ActorOf(Props.Create<RandomBomb>(), child.ToString())
                       .Tell("run!")
                       
    override my.SupervisorStrategy() =
        OneForOneStrategy(3, 1000,
                          fun ex ->
                              let is_head = r.Next(2) = 0
                              printfn "Coin is %s" (if is_head then "head, restart!" else "tail, stop! Press ENTER to end.")
                              if is_head then Directive.Restart else Directive.Stop
                              )
        
let Example (actor_system: ActorSystem) =
    let parent = actor_system.ActorOf(Props.Create<ParentActor>(), "parent")
    
    parent.Tell "start"
    
    printf "Wait for bomb.."
    Console.ReadLine() |> ignore
    actor_system.Terminate() |> ignore