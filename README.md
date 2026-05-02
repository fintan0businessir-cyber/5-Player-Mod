I know this install is ridiculously complicated, but there is a WIP on gamebanana (https://gamebanana.com/wips/99604) for a much better version so stay tuned for that.

Initial install:
  1. Put Assembly-CSharp.dll in Cuphead/Cuphead_data/Managed/
  2. Put Everything else in Cuphead/
  3. Open Cuphead. If using steam remote play, add everyone and change which player they are depending on what control scheme they want.
  5. Open a save file and press any button twice on one of the extra controllers.
  6. Press F7 to open UnityExplorer.
  7. Open the inspector.
  8. At the bottom, change the scene to a ground boss.
  9. Press load scene.
  10. Retry

Changing control scheme is quite complicated, so most people would probably be best off just using the default ones. Here's how you do it anyway:
  1. Open Assembly-Csharp.dll in Dnspy - https://github.com/dnspy/dnspy.
  2. Find the AbstractPlayerController class.
  3. The control schemes are stored in an array of dictionaries called aCUSTOMmappings, which each index corresponding to a different player.
  4. Change as needed, the numbers are the numbers Rewired uses to correspond to inputs
  5. Open aCUSTOMinput() which is also in AbstractPlayerController
  6. RT and LT are usually both axes, so you have to add custom logic for everything you want to bind to one:
     1. Copy the code that has already been done for player 1 and dash, player 2 and lock, player 2 and shoot, etc.
     2. Change the condition to whichever the index of the player who wants the trigger on their control scheme is.
     3. Change the button to whichever action you want assigned to the Trigger.
     4. Repeat for every control scheme that uses LT or RT.
     5. Remove all the conditions that were there initially.

To reduce player count:
  1. Download the source code and open the CreatePlayers class.
  2. On line 171 and 181, set j < [The amount of players you want].
  3. Delete line 129 - 131 to remove one player. 126 - 131 to remove two.
  4. Remove the players you want to remove from abscontarray.
  5. go to line 243 and remove a PlayerManager.aCUSTOMplayerjoinevent(); for every player you want to remove.

