using RWF;
using RWF.GameModes;
using RWF.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModdingUtils.AIMinion;
using UnboundLib.GameModes;
using UnboundLib;
using UnityEngine;
using Photon.Pun;

namespace BetterSandbox.Mode
{
    public class Sandbox : RWFGameMode
    {
        private readonly List<int> toRespawn = new();

        public override IEnumerator DoStartGame()
        {
            CardBarHandler.instance.Rebuild();
            UIHandler.instance.InvokeMethod("SetNumberOfRounds", 1);
            ArtHandler.instance.NextArt();
            yield return GameModeManager.TriggerHook("GameStart");
            GameManager.instance.battleOngoing = false;
            UIHandler.instance.ShowJoinGameText("SANDBOX START", PlayerSkinBank.GetPlayerSkinColors(1).winText);
            yield return new WaitForSecondsRealtime(0.25f);
            UIHandler.instance.HideJoinGameText();
            PlayerSpotlight.CancelFade(disable_shadow: true);
            PlayerManager.instance.SetPlayersSimulated(simulated: false);
            PlayerManager.instance.InvokeMethod("SetPlayersVisible", false);
            MapManager.instance.LoadNextLevel();
            TimeHandler.instance.DoSpeedUp(); 
            // var pickOrder = PlayerManager.instance.players;
            // int cards = (int)GameModeManager.CurrentHandler.Settings["roundsToWinGame"];
            // yield return new WaitForSecondsRealtime(1f);
            // for (int _ = 0; _ < cards; _++)
            // {
            //     yield return GameModeManager.TriggerHook("PickStart");
            //     foreach (Player player in pickOrder)
            //     {
            
            //     }
            //
            //     yield return WaitForSyncUp();
            //     CardChoiceVisuals.instance.Hide();
            //     yield return GameModeManager.TriggerHook("PickEnd");
            // }
            yield return new WaitForSecondsRealtime(1f);
            PlayerSpotlight.FadeIn();
            MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
            TimeHandler.instance.DoSpeedUp();
            TimeHandler.instance.StartGame();
            GameManager.instance.battleOngoing = true;
            UIHandler.instance.ShowRoundCounterSmall(teamPoints, teamRounds);
            PlayerManager.instance.InvokeMethod("SetPlayersVisible", true);
            StartCoroutine(DoRoundStart());
        }

        private IEnumerator TriggerPick(Player player)
        {
            // BetterSandbox.Instance.Log($"minions: {ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(player.data).minions.Count}");
            yield return WaitForSyncUp();
            yield return GameModeManager.TriggerHook("PlayerPickStart");
            CardChoiceVisuals.instance.Show(player.playerID, animateIn: true);
            yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
            yield return GameModeManager.TriggerHook("PlayerPickEnd");
            yield return new WaitForSecondsRealtime(0.1f);
        }

        public override void PlayerDied(Player killedPlayer, int teamsAlive)
        {
            // PlayerAssigner
            AIMinionHandler.sandbox
            if (killedPlayer.GetComponentInChildren<PlayerAI>() != null)
                BetterSandbox.Instance.Log($"Killed player {killedPlayer.playerID} is minion");
            if (toRespawn.Contains(killedPlayer.playerID)) return;
            toRespawn.Add(killedPlayer.playerID);
            StartCoroutine(IRespawnPlayer(killedPlayer));
        }

        public IEnumerator IRespawnPlayer(Player player)
        {
            // yield return new WaitForSecondsRealtime(delay);
            if (!toRespawn.Contains(player.playerID)) yield break;
            var _ = PlayerSpotlight.Cam;
            _ = PlayerSpotlight.Group;

            if (player.data.view.IsMine || PhotonNetwork.OfflineMode) PlayerSpotlight.FadeIn(0.05f);
            player.transform.position = GetSpawn(player.teamID);
            player.data.playerVel.SetFieldValue("simulated", false);
            // yield return TriggerPick(player);
            yield return new WaitForSecondsRealtime(0.5f);
            player.data.healthHandler.Revive();
            if (player.data.view.IsMine || PhotonNetwork.OfflineMode) PlayerSpotlight.FadeOut();
            player.data.playerVel.SetFieldValue("simulated", true);
            player.GetComponent<GeneralInput>().enabled = true; 
            toRespawn.Remove(player.playerID);
        }

        private static Vector3 GetSpawn(int teamID)
        {
            var spawns = MapManager.instance.GetSpawnPoints().Select(s => s.localStartPos).ToArray();
            spawns.Shuffle();
            return spawns[0];
        }
    }
}
