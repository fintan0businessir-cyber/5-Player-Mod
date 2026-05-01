using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Rewired;
using UnityEngine;

namespace Coop
{
    public class CreatePlayers
    {
        bool coroutinestarted = false;
        bool everyoneDies = false;
        public void Init()
        {
            On.Level.CreatePlayers += Createplayers;
            On.Map.CreatePlayers += CreateMapPlayers;
            On.PlayerStatsManager.OnStatsDeath += EveryoneDies;
        }
        
        public static IEnumerator checkinput()
        {
            
            while (true)
            {
                foreach (Joystick.Axis button in Rewired.ReInput.controllers.Joysticks[0].Axes)
                {
                    if (Mathf.Abs( button.value) > 0.3f )
                    {
                        Plugin.Log("ButtonName: " + button.name);
                    }
                }
                foreach (Joystick.Button button in Rewired.ReInput.controllers.Joysticks[0].Buttons)
                {
                    if (button.value)
                    {
                        Plugin.Log("ButtonName: " + button.name);
                    }
                }
                foreach (Keyboard.Button button in Rewired.ReInput.controllers.Keyboard.Buttons)
                {
                    if (button.value)
                    {
                        Plugin.Log("ButtonName: " + button.name);
                    }
                }
                yield return null;
            }
        }

        //class hi
        //{
        //    public static IEnumerator checkinput()
        //    {

        //        while (true)
        //        {
        //            foreach (Rewired.Joystick.Axis button in Rewired.ReInput.controllers.Joysticks[0].Axes)
        //            {
        //                if (Mathf.Abs(button.value) > 0.3f)
        //                {
        //                    UnityExplorer.ExplorerCore.Log("ButtonName: " + button.name);
        //                }
        //            }
        //            foreach (Rewired.Joystick.Button button in Rewired.ReInput.controllers.Joysticks[0].Buttons)
        //            {
        //                if (button.value)
        //                {
        //                    UnityExplorer.ExplorerCore.Log("ButtonName: " + button.name);
        //                }
        //            }
        //            foreach (Rewired.Keyboard.Button button in Rewired.ReInput.controllers.Keyboard.Buttons)
        //            {
        //                if (button.value)
        //                {
        //                    UnityExplorer.ExplorerCore.Log("ButtonName: " + button.name);
        //                }
        //            }
        //            yield return null;
        //        }
        //    }


        //}
        private void Createplayers(On.Level.orig_CreatePlayers orig, Level self)
        {
            //self.StartCoroutine(checkinput());
            self.PlayersCreated = true;
            foreach (AbstractPlayerController abstractPlayerController in UnityEngine.Object.FindObjectsOfType<AbstractPlayerController>())
            {
                UnityEngine.Object.Destroy(abstractPlayerController.gameObject);
            }
            self.players = new AbstractPlayerController[2];
            AbstractPlayerController[] customplayers = new AbstractPlayerController[3];

            if (PlayerManager.Multiplayer && self.allowMultiplayer)
            {

                Vector3 v = (self.playerMode != PlayerMode.Plane) ? self.spawns.playerOne : self.player1PlaneSpawnPos;
                self.players[0] = AbstractPlayerController.Create(PlayerId.PlayerOne, v, self.playerMode);
                Vector3 v2 = (self.playerMode != PlayerMode.Plane) ? self.spawns.playerTwo : self.player2PlaneSpawnPos;
                self.players[1] = AbstractPlayerController.Create(PlayerId.PlayerTwo, v2, self.playerMode);

                Vector3 addition = (self.playerMode == PlayerMode.Plane) ? new Vector3(0, 100) : new Vector3(100, 0);

                AbstractPlayerController abstractplayercontroller1 = self.players[0];
                AbstractPlayerController abstractplayercontroller2 = self.players[1];
                self.StartCoroutine(stupidcoroutine( self, abstractplayercontroller1, abstractplayercontroller2, addition, v2));
                
            }
            else
            {
                Vector3 v3 = (self.playerMode != PlayerMode.Plane) ? self.spawns.playerOneSingle : self.player1PlaneSpawnPos;
                self.players[0] = AbstractPlayerController.Create(PlayerId.PlayerOne, v3, self.playerMode);
            }
        }

        //base.player.input.actions.controllers.GetController<Rewired.Joystick>(Convert.ToInt32(this.properties.speed) - 490).GetAxis(0);
        public IEnumerator stupidcoroutine(Level self, AbstractPlayerController abstractplayercontroller1, AbstractPlayerController abstractplayercontroller2, Vector3 addition, Vector3 v2)
        {
            yield return new WaitForSeconds((self.playerMode == PlayerMode.Plane) ? 5 : 0);
            AbstractPlayerController abstractplayercontroller3 = AbstractPlayerController.Create(PlayerId.PlayerTwo, v2 + addition, self.playerMode);
            abstractplayercontroller3.LevelJoin(v2 + addition);
            yield return new WaitForSeconds((self.playerMode == PlayerMode.Plane) ? 0.1f : 0);
            AbstractPlayerController abstractplayercontroller4 = AbstractPlayerController.Create(PlayerId.PlayerTwo, v2 + (2 * addition), self.playerMode);
            abstractplayercontroller4.LevelJoin(v2 + (2 * addition));
            yield return new WaitForSeconds((self.playerMode == PlayerMode.Plane) ? 0.1f : 0);
            AbstractPlayerController abstractplayercontroller5 = AbstractPlayerController.Create(PlayerId.PlayerTwo, v2 + (3 * addition), self.playerMode);
            abstractplayercontroller5.LevelJoin(v2 + (3 * addition));
            Rewired.Player player = self.players[0].input.actions;
            Rewired.Player player2 = self.players[1].input.actions;
            player.controllers.ClearAllControllers();
            player2.controllers.ClearAllControllers();

            AbstractPlayerController[] abscontarray = { abstractplayercontroller1, abstractplayercontroller2, abstractplayercontroller3 , abstractplayercontroller4, abstractplayercontroller5};

            foreach (SpriteRenderer renderer in abstractplayercontroller3.GetComponentsInChildren<SpriteRenderer>())
            { renderer.color = new Color(1, 1, 0); }
            foreach (SpriteRenderer renderer in abstractplayercontroller4.GetComponentsInChildren<SpriteRenderer>())
            { renderer.color = new Color(1, 0, 1); }
            foreach (SpriteRenderer renderer in abstractplayercontroller5.GetComponentsInChildren<SpriteRenderer>())
            { renderer.color = new Color(0, 1, 1); }

            

            Rewired.Controller[] joystick = Rewired.ReInput.controllers.GetControllers(Rewired.ControllerType.Joystick);
            Plugin.Log("Controllers found: " + joystick.Length);
            Plugin.Log("Controller names: " + string.Join(", ", joystick.Select(c => c.name).ToArray()));
            Plugin.Log("Controller types: " + string.Join(", ", joystick.Select(c => c.id.ToString()).ToArray()));
            Rewired.Controller[] keyboard = Rewired.ReInput.controllers.GetControllers(Rewired.ControllerType.Keyboard);
            Plugin.Log("keyboards found: " + keyboard.Length);
            Plugin.Log("keyboard names: " + string.Join(", ", keyboard.Select(c => c.name).ToArray()));
            Plugin.Log("keyboard types: " + string.Join(", ", keyboard.Select(c => c.id.ToString()).ToArray()));


            for (int i = 0; i < joystick.Length;)
            {
                player.controllers.AddController(joystick[i], false);
                player2.controllers.AddController(joystick[i], false);

                i++;
            }

            player.controllers.AddController(ControllerType.Keyboard, 0, false);
            player2.controllers.AddController(ControllerType.Keyboard, 0, false);

            if (self.playerMode == PlayerMode.Plane)
            {
                for (int j = 0; j < 5;)
                {
                    PlanePlayerMotor motor = abscontarray[j].GetComponent<PlanePlayerMotor>();
                    motor.id = j;

                    j++;
                }
            }
            else
            {
                for (int j = 0; j < 5;)
                {
                    LevelPlayerMotor motor = abscontarray[j].GetComponent<LevelPlayerMotor>();
                    motor.properties.moveSpeed = 490f + j;

                    j++;
                }
            }
            self.StartCoroutine(WaitAndAddHealth(abstractplayercontroller3, abstractplayercontroller4, abstractplayercontroller5));


        }
        private void CreateMapPlayers(On.Map.orig_CreatePlayers orig, Map self)
        {


            if (!PlayerData.Data.CurrentMapData.sessionStarted)
            {
                PlayerData.Data.CurrentMapData.sessionStarted = true;
                PlayerData.Data.CurrentMapData.playerOnePosition = self.firstNode.transform.position + (Vector3)self.firstNode.returnPositions.playerOne;
                PlayerData.Data.CurrentMapData.playerTwoPosition = self.firstNode.transform.position + (Vector3)self.firstNode.returnPositions.playerTwo;
                if (!PlayerManager.Multiplayer)
                {
                    PlayerData.Data.CurrentMapData.playerOnePosition = self.firstNode.transform.position + (Vector3)self.firstNode.returnPositions.singlePlayer;
                }
            }
            else if (PlayerData.Data.CurrentMapData.enteringFrom != PlayerData.MapData.EntryMethod.None)
            {
                self.entryPoints[(int)PlayerData.Data.CurrentMapData.enteringFrom].SetPlayerReturnPos();
                PlayerData.Data.CurrentMapData.enteringFrom = PlayerData.MapData.EntryMethod.None;
            }
            PlayerData.SaveCurrentFile();
            MapPlayerPose pose = MapPlayerPose.Default;
            if (Level.Won && Level.PreviousLevel != Levels.Saltbaker)
            {
                pose = MapPlayerPose.Won;
            }
            self.players = new MapPlayerController[2];
            self.players[0] = MapPlayerController.Create(PlayerId.PlayerOne, new MapPlayerController.InitObject(PlayerData.Data.CurrentMapData.playerOnePosition, pose));
            self.players[0].motor.id = 4;
            if (PlayerManager.Multiplayer)
            {

                //Rewired.Player player = self.players[0].input.actions;
                //Rewired.Player player2 = self.players[1].input.actions;
                //player.controllers.ClearAllControllers();
                //player2.controllers.ClearAllControllers();

                //Rewired.Controller[] joystick = Rewired.ReInput.controllers.GetControllers(Rewired.ControllerType.Joystick);
                //Plugin.Log("Controllers found: " + joystick.Length);
                //Plugin.Log("Controller names: " + string.Join(", ", joystick.Select(c => c.name).ToArray()));


                //for (int i = 0; i < joystick.Length;)
                //{
                //    player.controllers.AddController(joystick[i], false);
                //    player2.controllers.AddController(joystick[i], false);

                //    i++;
                //}

                self.players[1] = MapPlayerController.Create(PlayerId.PlayerTwo, new MapPlayerController.InitObject(PlayerData.Data.CurrentMapData.playerTwoPosition, pose));
                PlayerManager.aCUSTOMplayerjoinevent();
                PlayerManager.aCUSTOMplayerjoinevent();
                PlayerManager.aCUSTOMplayerjoinevent();
                int k = 0;
                foreach (MapPlayerMotor mapplayer in UnityEngine.Object.FindObjectsOfType<MapPlayerMotor>())
                {
                    mapplayer.id = k;
                    k++;
                }
            }
        }

        public void EveryoneDies(On.PlayerStatsManager.orig_OnStatsDeath orig, PlayerStatsManager self)
        {
            if (everyoneDies)
            {
                orig(self);
            }
            else
            {
                everyoneDies = true;
                foreach (PlayerStatsManager stats in UnityEngine.Object.FindObjectsOfType<PlayerStatsManager>())
                {
                    stats.Health = 0;
                    stats.OnStatsDeath();
                }
                everyoneDies = false;
            }
        }


        public IEnumerator WaitAndAddHealth(AbstractPlayerController abstractplayercontroller3, AbstractPlayerController abstractplayercontroller4, AbstractPlayerController abstractplayercontroller5)
        {
            yield return new WaitForSeconds(4);
            foreach (PlayerStatsManager stats in UnityEngine.Object.FindObjectsOfType<PlayerStatsManager>())
            {
                stats.Health = 3;
            }

            abstractplayercontroller3.GetComponentInChildren<SpriteRenderer>().color = new Color(0, 1, 1);
            abstractplayercontroller4.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 0, 1);
            abstractplayercontroller5.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 0);
        }






















    }
}
//this.player.input.actions.controllers.GetController<Joystick>(Convert.ToInt32(this.properties.moveSpeed) - 490).GetButtonDown();


