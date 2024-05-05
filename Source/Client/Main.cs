using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using RimworldArchipelago.Client.Multiworld;
using RimworldArchipelago.Client.Rimworld;
using RimworldArchipelago.RimWorld;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace RimworldArchipelago.Client
{

    public class Main : HugsLib.ModBase
    {
        internal static Main Instance { get; private set; }

        internal MultiWorldSessionManager MultiWorldSessionManager { get; } = new MultiWorldSessionManager();
        internal ComponentStateManager ComponentStateManager { get; } = new ComponentStateManager();
        internal DefManager DefManager { get; } =  new DefManager();

        public string Address { get; private set; } = "127.0.0.1:38281";
        public string PlayerSlot { get; private set; } = "Player";

        internal bool Connected { get { return MultiWorldSessionManager.Session?.Socket?.Connected ?? false; } }
        // todo: allow users to connect multiple times.
        // eg- preserve original defs.

        public static bool IsResearchLocation(long id) => id >= 11_000 && id < 12_000;
        public ArchipelagoSession Session { get => MultiWorldSessionManager.Session; }
        public string GetPlayerStamp => ComponentStateManager.GetMultiworldMarker();
        public string GetMWSeedStamp => ComponentStateManager.GetMWSeed();
        public IEnumerable<HydratedLocation> AllLocations => MultiWorldSessionManager.AllHydratedLocations;

        /************************************************/
        public IDictionary<string, long> DefNameToArchipelagoId { get; } = new ConcurrentDictionary<string, long>();
        public IDictionary<long, RimWorldDef> ArchipeligoItemIdToRimWorldDef { get; } = new ConcurrentDictionary<long, RimWorldDef>();
        public struct RimWorldDef { public string DefName; public string DefType; public int Quantity; }

        public ArchipelagoLoader ArchipelagoLoader { get; private set; }
        /******************************/
        public Main()
        {
            Instance = this;
        }

        public void UpdatePlayerStamp()
        {
            ComponentStateManager.UpdateMultiworldMarker(PlayerSlot, MultiWorldSessionManager.Seed);
        }

        public void LogMessage(string message, bool error = false)
        {
            if (error)
            {
                Logger.Error(message);
            }
            else
            { 
                Logger.Message(message);
            }
        }

        public bool Connect(string address, string playerSlot, string password = null)
        {
            Logger.Message("Connecting to Archipelago...");

            Address = address;
            PlayerSlot = playerSlot;

            LoginResult result = MultiWorldSessionManager.Connect(address, playerSlot, password);

            if (result.Successful)
            {
                MultiWorldSessionManager.InitialiseFromConnection(playerSlot);
                Logger.Message("Successfully Connected.");
                return true;
            }
            else
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {address} as {playerSlot}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }
                Logger.Error(errorMessage);

                return false;
            }
        }

        public void LoadMultiworld()
        {
            var researchLocations = MultiWorldSessionManager.AllHydratedLocations
                .Where(l => l.IsResearchLocation);
            //setting up research according to locations and items
            this.DefManager.LoadResearchDefs(researchLocations);
            //register hooks
        }

        public void SendLocationCheck(string defName)
        {
            Logger.Message($"Sending completed location {defName} to Archipelago");
            MultiWorldSessionManager.Session.Locations.CompleteLocationChecks(DefNameToArchipelagoId[defName]);
        }

        public void TriggerGoalComplete()
        {
            Logger.Message("Goal complete!");
            MultiWorldSessionManager.SendGoalCompletePacket();
        }
    }
}
