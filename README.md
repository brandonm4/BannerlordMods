![Mount & Blade II: Bannerlord Tournaments XPanded](https://staticdelivery.nexusmods.com/mods/3174/images/headers/27_1586668241.jpg)
#Mount & Blade II: Bannerlord Tournaments XPanded Module

##Current Features
* XP Gain in Arena and Tournaments	
	* Configurable % gain
* Prize Pool for Tournaments
	* Select a prize from a pool
	* Re-roll prize pool
	* Prize Pool Generation Options
		* Vanilla
		* Customized List
		* Items only in the Town inventory
		* Combination of above
		* Filter items by value
		* Filter items by type
* Additional Rewards for Tournament
	* Flat Amout on Winning
		* Gold per match
		* Gold per tournament
		* Renown
		* Influcence
	* Dynamic Amount of renown based on Difficulty
* All Options are configurable and can be turned on or off 
* OPTIONAL Integration with ModLib for GUI Configuration		

Provides Tournament Prize options as well as enhances the xp in tournaments and arena fights.  It originally provided xp gain in tournaments but that is now a feature in the core game as of 1.0.6.

Technical Design Decisions/Rules
I will not override/replace any stock game default model or behavior.  I will allow myself the option to add new behaviors that will work alongside the existing ones, but they will not be replaced by a customized version.
If I am going to alter vanilla models/behaviors it will be done purely by harmony injection.
The theory with that is that if I use injection it should remain cross-compatible with any other mod.  If you use model replacement, it's a last mod wins.

Any feature should be able to be turned off or on by the end-user.
Any numerical value should be editable by the end-user.

I only provide options and guidelines on how a player can play.  It's left up to them to decide what to enable, and how they want to alter their game.  I hate cooks to think food must be eaten their way and no other.


