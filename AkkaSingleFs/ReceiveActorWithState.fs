module ReceiveActorWithState

open System
open Akka.Actor

type Hello = Hello of string
type Swap = Swap

type TestActor() as this =
    inherit ReceiveActor()

    // First pain point, cannot use auto-property. F# auto-property is in-compatible.
    let mutable stash: IStash = null

    do this.Normal()

    interface IWithUnboundedStash with
        member this.Stash
            with get () = stash
            and set v = stash <- v

    // Second, to use protected member from lambda, it needs a bridge..
    member _.Become f = base.Become(Action(f))

    member this.Normal() =
        this.Receive<Hello> (function
            | Hello s -> printfn $"Hi %s{s}!")

        this.Receive<Swap>(fun _ -> this.Become(this.Ghost))

    member this.Ghost() =
        this.Receive<Hello> (function
            // Third pain point, call self implementation of interface is tricky!
            | Hello _ -> (this :> IWithUnboundedStash).Stash.Stash())

        this.Receive<Swap>(fun _ ->
            this.Become(this.Normal)
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