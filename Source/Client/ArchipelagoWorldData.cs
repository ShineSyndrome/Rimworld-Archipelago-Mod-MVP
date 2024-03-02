using Archipelago.MultiClient.Net.Helpers;
using HugsLib.Utils;
using Newtonsoft.Json;
using RimWorld.Planet;
using RimworldArchipelago.Client.Services;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimworldArchipelago.Client
{
    public static class ArchipelagoWorldComp
    {
        private static ModLogger Log => Main.Instance.Log;
        internal static HashSet<long> ItemsAwaitingReceipt = new HashSet<long>();

        private static ArchipelagoWorldData comp => Find.World?.GetComponent<ArchipelagoWorldData>();

        public static void ReceiveItem(long archipelagoItemId)
        {
            bool readyToRecieve = Find.AnyPlayerHomeMap != null || comp != null;

            if (!readyToRecieve)
            {
                ItemsAwaitingReceipt.Add(archipelagoItemId);
                Log.Message($"Could not yet receive Archipelago item {archipelagoItemId}");
            }
            else
            {
                comp.ReceiveItem(archipelagoItemId);
            }
        }

        public static void Reset()
        {
            ItemsAwaitingReceipt.Clear();
            comp.ReceivedItems.Clear();
        }
    }

    public class ArchipelagoWorldData : WorldComponent
    {
        private static ModLogger Log => Main.Instance.Log;

        public HashSet<long> ReceivedItems = new HashSet<long>();

        public ArchipelagoWorldData(World world) : base(world)
        {

        }

        public void ReceiveItem(long archipelagoItemId)
        {
            if (ArchipelagoWorldComp.ItemsAwaitingReceipt.Contains(archipelagoItemId))
            {
                ArchipelagoWorldComp.ItemsAwaitingReceipt.Remove(archipelagoItemId);
            }

            ReceivedItems.Add(archipelagoItemId);

            if (MultiWorldService.Instance.ArchipeligoItemIdToRimWorldDef.ContainsKey(archipelagoItemId))
            {
                var defMapping = MultiWorldService.Instance.ArchipeligoItemIdToRimWorldDef[archipelagoItemId];
                var defName = defMapping.DefName;
                var defType = defMapping.DefType;

                // TODO something other than ResearchProjectDef
                if (defType == "ResearchProjectDef")
                {
                    var def = DefDatabase<ResearchProjectDef>.GetNamed(defName, true);
                    Find.ResearchManager.FinishProject(def);
                }
                else
                {
                    Log.Error($"Unrecognized RimWorld DefType {defType} associated with Archipelago item id {archipelagoItemId}");
                }
            }
            else
            {
                Log.Error($"Could not find RimWorld Def associated with Archipelago item id {archipelagoItemId}");
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ReceivedItems, "Archipelago_ReceivedItems");
            Scribe_Collections.Look(ref ReceivedItems, "Archipelago_ItemsAwaitingReceipt");
        }
    }
}
