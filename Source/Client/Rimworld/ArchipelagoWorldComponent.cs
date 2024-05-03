using Archipelago.MultiClient.Net.Helpers;
using HugsLib.Utils;
using Newtonsoft.Json;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace RimworldArchipelago.Client.Rimworld
{
    public class ArchipelagoWorldComponent : WorldComponent
    {
        //Used to mark saves as belonging to a MW Rando
        //and stop people from breaking their vanilla game saves
        public string multiWorldPlayerStamp;
        public string mwSeed;

        public List<long> ReceivedItems = new List<long>();

        public ArchipelagoWorldComponent(World world) : base(world)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ReceivedItems, nameof(ReceivedItems), LookMode.Value);
            Scribe_Values.Look(ref multiWorldPlayerStamp, nameof(multiWorldPlayerStamp), "");
            Scribe_Values.Look(ref mwSeed, nameof(mwSeed), "");
        }
    }
}
