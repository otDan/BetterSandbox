using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RWF;
using RWF.GameModes;
using TMPro;
using UnboundLib;
using UnboundLib.Extensions;
using UnboundLib.GameModes;
using UnboundLib.Utils;
using UnityEngine.UI;
using UnityEngine;

namespace BetterSandbox.Mode
{
    public class SandboxHandler : RWFGameModeHandler<Sandbox>
    {
        internal const string GameModeName = "Better Sandbox";
        internal const string GameModeID = "Better_Sandbox";

        public SandboxHandler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: false,
            pointsToWinRound: 0,
            roundsToWinGame: 0,
            // null values mean RWF's instance values
            playersRequiredToStartGame: 1,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"A better version of the vanilla sandbox. Test your builds, break the game, have fun!"
            )
        { }

        public sealed override GameSettings Settings
        {
            get => new()
            {
                { "pointsToWinRound", 2 },
                { "roundsToWinGame", 5 },
                { "playersRequiredToStartGame", 1 },
            };
            protected set => base.Settings = value;
        }
    }
    
    public class TeamSandboxHandler : RWFGameModeHandler<Sandbox>
    {
        internal const string GameModeName = "Team Better Sandbox";
        internal const string GameModeID = "Team_Better_Sandbox";
        public TeamSandboxHandler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: true,
            pointsToWinRound: 0,
            roundsToWinGame: 0,
            // null values mean RWF's instance values
            playersRequiredToStartGame: 1,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"A better version of the vanilla sandbox with teams. Test your builds, break the game, have fun!"
            ) 
        { }
    }

    [HarmonyPatch(typeof(CharacterSelectionInstance), "ReadyUp")]
    class CharacterSelectionInstance_Patch_ReadyUp
    {
        static bool Prefix(CharacterSelectionInstance[] ___selectors, ref bool ___isReady) {
            ___isReady = !___isReady;

            int numReady = 0;
            int numPlayers = 0;

            foreach (var character in ___selectors)
            {
                if (character.isReady) {
                    numReady++;
                }
                if (character.currentPlayer) {
                    numPlayers++;
                }
            }

            int neededPlayers = GameModeManager.CurrentHandlerID == SandboxHandler.GameModeID ? 1 : RWFMod.instance.MinPlayers;
            if (numReady == numPlayers && numReady >= neededPlayers) {
                MainMenuHandler.instance.Close();

                // assign teamIDs according to colorIDs
                int nextTeamID = 0;
                var colorToTeam = new Dictionary<int, int>();
                foreach (Player player in PlayerManager.instance.players)
                {
                    if (colorToTeam.TryGetValue(player.colorID(), out int teamID))
                    {
                        player.AssignTeamID(teamID);
                    }
                    else
                    {
                        player.AssignTeamID(nextTeamID);
                        colorToTeam[player.colorID()] = nextTeamID;
                        nextTeamID++;
                    }
                }

                GameModeManager.CurrentHandler.StartGame();
                return false;
            }

            ___isReady = !___isReady;
            return true;
        }
    }
}
