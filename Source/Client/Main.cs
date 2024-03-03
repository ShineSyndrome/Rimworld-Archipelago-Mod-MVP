using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using HarmonyLib;
using HugsLib;
using HugsLib.Utils;
using Newtonsoft.Json;
using RimworldArchipelago.Client.Multiworld;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.PlayerLoop;
using Verse;

namespace RimworldArchipelago.Client
{

    public class Main : HugsLib.ModBase
    {
        internal static Main Instance { get; private set; }

        public ArchipelagoSession Session { get => MultiWorldSessionManager.Session; }
        public ArchipelagoLoader ArchipelagoLoader { get; private set; }

        public string Address { get; private set; } = "127.0.0.1:38281";
        public string PlayerSlot { get; private set; } = "Player";

        public struct RimWorldDef { public string DefName; public string DefType; public int Quantity; }
        public IDictionary<string, long> DefNameToArchipelagoId { get;  } = new ConcurrentDictionary<string, long>();
        public IDictionary<long, RimWorldDef> ArchipeligoItemIdToRimWorldDef { get; } = new ConcurrentDictionary<long, RimWorldDef>();

        public static bool IsResearchLocation(long id) => id >= 11_000 && id < 12_000;
        public static bool IsCraftLocation(long id) => id >= 12_000 && id < 13_000;
        public static bool IsPurchaseLocation(long id) => id >= 13_000 && id < 14_000;

        public Main()
        {
            Instance = this;
        }

        public bool Connect(string address, string playerSlot, string password = null)
        {
            Logger.Message("Connecting to Archipelago...");
            // store address & player slot even if invalid if there is no session yet
            if (Session == null)
            {
                Address = address;
                PlayerSlot = playerSlot;
            }

            LoginResult result = MultiWorldSessionManager.Connect(address, playerSlot, password);

            if (result.Successful)
            {
                Logger.Message("Successfully Connected.");
                ArchipelagoLoader = new ArchipelagoLoader();
                
                //Dirty but works.
                //Concession for tricking a synchronous override to do async workloads.
                _ = ArchipelagoLoader.Load();

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

        public void SendLocationCheck(string defName)
        {
            Logger.Message($"Sending completed location {defName} to Archipelago");
            Session.Locations.CompleteLocationChecks(DefNameToArchipelagoId[defName]);
        }

        public void TriggerGoalComplete()
        {
            Logger.Message("Goal complete!");
            MultiWorldSessionManager.SendGoalCompletePacket();
        }
    }
}
