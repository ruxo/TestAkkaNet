open System
open Akka.Actor

let actor_system = ActorSystem.Create("SingleActor")

printfn "Choose an example:"
printfn "1. Simple Setup"
printfn "2. Exception Handling"
printfn "3. ReceiveActor with state"
printf "> "
let choice = Console.ReadKey().KeyChar
printfn ""

match choice with
| '1' -> SimpleSetup.Example actor_system
| '2' -> ExceptionHandling.Example actor_system
| '3' -> ReceiveActorWithState.Example actor_system
| _ -> ()

actor_system.WhenTerminated.Wait()
printfn "End."