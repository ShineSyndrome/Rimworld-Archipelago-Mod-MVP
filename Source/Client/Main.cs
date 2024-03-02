using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using HarmonyLib;
using HugsLib;
using HugsLib.Utils;
using Newtonsoft.Json;
using RimworldArchipelago.Client.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Verse;

namespace RimworldArchipelago.Client
{

    public class Main : HugsLib.ModBase
    {
        public MultiWorldService MultiWorldService { get; }

        internal static Main Instance { get; private set; }

        public ArchipelagoSession Session { get => MultiWorldService.Session; } 

        public ModLogger Log => base.Logger;
        public string Address { get; private set; } = "127.0.0.1:38281";
        public string PlayerSlot { get; private set; } = "Player";

        public ArchipelagoLoader ArchipelagoLoader { get; private set; }
        public struct RimWorldDef { public string DefName; public string DefType; public int Quantity; }

        public Main()
        {
            Instance = this;
            MultiWorldService = MultiWorldService.Instance;
        }

        public async Task<bool> Connect(string address, string playerSlot, string password = null)
        {
            Log.Message("Connecting to Archipelago...");

            Address = address;
            PlayerSlot = playerSlot;

            return await MultiWorldService.Connect(address, playerSlot, password);
        }

        public async void SendLocationCheck(string defName)
        {
            Log.Message($"Sending completed location {defName} to Archipelago");
            await MultiWorldService.SendLocationCheck(defName);
        }
    }
}
