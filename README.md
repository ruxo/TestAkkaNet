## AkkaSingle project ##

This project demonstrates a simple setup of local Akka system, and how to handle an exception of an actor.

## AkkaRemote project ##

This demonstrates how to setup two Akka systems and let them talk to each other.  This program needs two
arguments. The first argument is its own port for starting Akka with TCP listener.  The second argument
is a port that the program should talk to.  It will connect to the target Akka system and ask for Echo
service, which should be on `/user/echo` as Akka path.

### Steps ###

1. Run two instances of this project with following commands.

    ```shell
    dotnet run 6000 6001
    dotnet run 6001 6000
    ```

2. On instance #1, select menu (1) to run an Echo service on itself.
3. On instance #2, select menu (3), input text and observe an echoed text from remote service.

## AkkaCluster project ##

Run a cluster of 3 instances, create 3 echo services, one service per instance. Send text and see each echo service response in round-robin fashion.