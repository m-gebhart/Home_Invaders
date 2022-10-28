# Home_Invaders
Home_Invaders is a networking assignment from my fourth semester. Inspired by Space Invaders, multiple players can take over the role of an alien each to defeat the enemy tank (NPC) on the ground. The client system was made with Unity with the server being written in C++.

The project and documentation can be found on <a href="https://onedrive.live.com/?id=F93423146BB21A09%2177819&cid=F93423146BB21A09">OneDrive</a>.

## Media
![home_invaders_1](https://user-images.githubusercontent.com/45672199/198705416-861e0a60-e4b4-4816-9952-e15c27b3d221.gif)

Above, you can see the basic gameplay. The moving alien is the controlling local player while the other aliens represent the other online players. Here, they are inactive due to no input given to them (I was alone while playtesting it). The enemy cannon’s movement and attack frequencies are fully controlled by the C++ Server.

![home_invaders_2](https://user-images.githubusercontent.com/45672199/198705482-1afe8b04-da48-4ef9-a966-e11b26eaeae8.gif)


Similar to agar io, the server hosts a continous session that players can join and leave at any time.

As of now, a round only starts each time the server restarts. Due to time constraints, I didn’t fully implement a logout function or any winning conditions yet. Initially, it was planned that the server is suppossed to restart the round once the enemy is killed and to reward the winner with victory points.

![home_invaders_3](https://user-images.githubusercontent.com/45672199/198705529-5cdd15f1-da9c-414b-b4ed-b0b4a4a3e478.gif)
