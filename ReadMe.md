# NotBackgroundObjectRando

A Randomizer connection for every single breakable background object

This connection adds 10068 locations. Optionally you may choose to add an item for each location with the "Lock Behind Items" setting. 
If enabled, the breakable object will not exist until the corresponding item is obtained, effectively acting as a "key" item.
- This tends to cause your rando to have several thousand progression spheres, so use at your own risk.

## Rooms
Some locations exist in obscure rooms such as the void heart climb, the dream nail platforming section, and some godhome arenas. See **Warnings** below for more on that. 
The only pantheon-specific room you need to worry about is Great Nailsage Sly. Most bosses can be unlocked throughout Hallownest and then accessed via Hall of Gods, but you will need to complete the third pantheon.

## Logic
Due to the tremendous number of checks in this connection, complete logic has not been attempted. 
Most locations only require that you can access their room from any transition, though infection is accounted for. 
However, there is a json available for manually implemented clauses. Pull requests are welcome and encouraged.

## Warnings
- If you enable the setting to lock locations behind items, at least one other location must exist from base rando or another connection.
- Softlock prevention has not been implemented. If you will not be able to return to a room, do your best to clear it out completely.
- Some other mods may remove objects to make space for new content (for example, new benches or stags). No compatibility checks have been implemented.Includes:BgObj-Fungus1_36_green_grass_1[1]_22,14_2,85 with "stone sactuarry" bench. something and a zote bench in city bridge,left of storerooms. quirrel peak bench/stag.
(i didnt come up with any better place for this yes)
- Yes, on some devices you may experience lag when you start the rando, sit at a bench, break items, etc. This is normal.

## GrassRandomizer
- This mod appears to have no issues running with GrassRando (aside from the lag from thousands of checks).
- Certain types of objects were intentionally skipped because they appear in other rando connections (such as stalactites or jelly egg bombs). However, GrassRando was selective about which objects counted as "grass", but this mod includes all forms of plants, so there will be some overlapping locations.
- When both connections are enabled, qualifying grass objects will simply grant two checks simultaneously.

## Misc
- Some objects can only be broken a single time (such as the walls in King's Pass or the jars beneath Collector). These are not check locations.
- For technical reasons, a few breakable objects were skipped/overlooked due to their unique internal structure. I am aware of two such signs in Crossroads but there might be more.
- Some missable objects were ignored (such as the infection barriers near Glowing Womb and Broken Vessel). It is possible that other similar objects weren't found during development.
- Some geometry changes slightly in west Ancient Basin when Monarch Wings is obtained, including various breakable objects. These are not check locations.
- Honestly this isn't really a serious mod. It would take way way way too much work to add names and logic and implementation for these checks properly.
