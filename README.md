## AkkaSingle project ##

This project demonstrates a simple setup of local Akka system, and how to handle an exception of an actor.

## AkkaRemote project ##

This demonstrates how to setup two Akka systems and let them talk to each other.  This program needs two
arguments. The first argument is its own port for starting Akka with TCP listener.  The second argument
is a port that the program should talk to.  It will connect to the target Akka system and ask for Echo
service, which should be on `/user/echo` as Akka path.
