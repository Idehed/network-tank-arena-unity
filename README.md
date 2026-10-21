# Network Tank Arena Anton Idehed

## Project summary

You play as a tank and you are to survive as long as you can. You play vs another player that can damage you, but you do have some healthkits laying around that you can pick up and it will give you 25 health.


## How to run/test the project

You start the game with one pressing Start Host and the other one (or you again) pressing Start Client. You use WASD to move around and then you use the mouse to look around. You will shoot with the left mouse button. 

## What I Implemented/ why I choose it

I implemented healthkits that will give you some health back if you are damaged. When the player óverlaps with the healthkit it will give the one overlapped some hp back and it will dissapear for both thge clinet and the host.  I also of course implemented health for the player/tank so that you can take damage and die. I thought adding health/ healthkits would add some more liveliness to the game. 
Kinda feel like it is not a game othervise.

## Networking/synchronizasstion problem

I had some problems with the GIU that showed health for both the client and the host when I added the healthscript to the networkingtank. I wanted the health to be shown indepenently for each client/ host.
I did change the NetworkVariableReadPermission to owner and then I had to check it had an owner (If !Owner return;).

## Important client/server authority and replication decision 

Health is stored as a server NetworkVariable rather than being updated through RPCs. This means clients can read the value but never change it, and a player who joins midgame receives the current health immediately instead of needing past damage events replayed. Effects as explosions are sent as ClientRpcs instead, since they carry no state that a late joining player need.