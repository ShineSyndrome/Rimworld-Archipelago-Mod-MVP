using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using RimworldArchipelago.Client.Multiworld;
using RimworldArchipelago.Client.Rimworld;
using RimworldArchipelago.RimWorld;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace RimworldArchipelago.Client
{

    public class Main : HugsLib.ModBase
    {
        internal static Main Instance { get; private set; }
        public static IServiceProvider ServiceProvider { get; private set; }

        internal MultiWorldSessionManager MultiWorldSessionManager { get; } = new MultiWorldSessionManager();
        internal ComponentStateManager ComponentStateManager { get; } = new ComponentStateManager();

        internal bool Connected { get { return MultiWorldSessionManager.Session?.Socket?.Connected ?? false; } }
        // we might need this for stopping bad loads
        //public bool InvalidGame = false;

        public ArchipelagoSession Session { get => MultiWorldSessionManager.Session; }


        public string Address { get; private set; } = "127.0.0.1:38281";
        public string PlayerSlot { get; private set; } = "Player";

        public struct RimWorldDef { public string DefName; public string DefType; public int Quantity; }
        public IDictionary<string, long> DefNameToArchipelagoId { get;  } = new ConcurrentDictionary<string, long>();
        public IDictionary<long, RimWorldDef> ArchipeligoItemIdToRimWorldDef { get; } = new ConcurrentDictionary<long, RimWorldDef>();

        public ArchipelagoLoader ArchipelagoLoader { get; private set; }

        public static bool IsResearchLocation(long id) => id >= 11_000 && id < 12_000;

        public Main()
        {
            Instance = this;
        }

        public string GetPlayerStamp => ComponentStateManager.GetMultiworldMarker();

        public void UpdatePlayerStamp()
        {
            ComponentStateManager.UpdateMultiworldMarker(PlayerSlot);
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
            //grabbing locations
            //grabbing items
            //setting up research according to locations and items
            //register hooks

            Console.WriteLine("test");    
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
