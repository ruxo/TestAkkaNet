module ReceiveActorWithState

open System
open Akka.Actor

type Messages =
    | Hello of string
    | Swap

type TestActor() as this =
    inherit ReceiveActor()

    let mutable stash: IStash = null

    do this.Normal()

    interface IWithUnboundedStash with
        member this.Stash
            with get () = stash
            and set v = stash <- v

    // First pain point, to use protected member from lambda, it needs a bridge..
    member _.Become x = base.Become(x: Action)

    member this.Normal() =
        this.Receive<Messages> (function
            | Hello s -> printfn $"Hi %s{s}!"
            | Swap -> this.Become(Action(this.Ghost)))

    member this.Ghost() =
        this.Receive<Messages> (function
            // Second pain point, call self implementation of interface is tricky!
            | Hello _ -> (this :> IWithUnboundedStash).Stash.Stash()
            | Swap ->
                this.Become(Action(this.Normal))
                stash.UnstashAll())

let Example (actor_system: ActorSystem) =
    let a = actor_system.ActorOf(Props.Create<TestActor>(), "test-actor")
    printfn "Normal mode"
    a.Tell(Hello "Hello")
    a.Tell(Hello "World")

    printfn "Press ENTER to swap"
    Console.ReadLine() |> ignore
    a.Tell Swap

    printfn "Ghost mode"
    a.Tell(Hello "Keep silent")
    a.Tell(Hello "message after swap")

    printfn "Press ENTER to swap again"
    Console.ReadLine() |> ignore
    a.Tell Swap

    printfn "Press ENTER to end"
    Console.ReadLine() |> ignore
    actor_system.Terminate() |> ignore