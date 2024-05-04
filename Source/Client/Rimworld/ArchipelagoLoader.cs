﻿using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
using HugsLib.Utils;
using Newtonsoft.Json;
using RimWorld;
using RimworldArchipelago.Client;
using RimworldArchipelago.Client.Rimworld;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace RimworldArchipelago.RimWorld
{
    /// <summary>
    /// Loads the initial data for archipelago locations and items
    /// </summary>
    public class ArchipelagoLoader
    {
        public class Location
        {
            public string Name;
            public long ItemId;
            public string ItemName;
            public int Player;
            public string ExtendedLabel;
        }

        public class LocationResearchMetaData
        {
            public float x;
            public float y;
            public float cost;
            public long[] prerequisites;
        }

        public IDictionary<int, PlayerInfo> Players { get; private set; }

        public readonly IDictionary<long, Location> ResearchLocations = new ConcurrentDictionary<long, Location>();

        public int CurrentPlayerId;
        private IDictionary<string, object> SlotData { get; set; }

        public readonly IDictionary<long, ResearchProjectDef> AddedResearchDefs = new Dictionary<long, ResearchProjectDef>();
        public readonly IDictionary<long, RecipeDef> AddedRecipeDefs = new Dictionary<long, RecipeDef>();

        //private ArchipelagoSession Session => Main.Instance.Session;

        public ArchipelagoLoader()
        {

        }

        //public async Task Load()
        //{
        //    try
        //    {
        //        //LoadRimworldDefMaps();
        //        //await LoadLocationDictionary();
        //        LoadResearchDefs();

        //        AddSessionHooks();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.Message + "\n" + ex.StackTrace);
        //    }
        //}

        /// <summary>
        /// Build the mappings from numerical Archipelago ids to rimworld string def names. 
        /// This will help us when receiving items from Archipelago
        /// </summary>
        //private void LoadRimworldDefMaps()
        //{
        //    var defNameMap = JsonConvert.DeserializeObject<Dictionary<long, Dictionary<string, string>>>(SlotData["item_id_to_rimworld_def"].ToString());
        //    foreach (var kvp in defNameMap)
        //    {
        //        Main.Instance.ArchipeligoItemIdToRimWorldDef[kvp.Key] = new Main.RimWorldDef()
        //        {
        //            DefName = kvp.Value["defName"],
        //            DefType = kvp.Value["defType"]
        //        };
        //    }
        //}

        /// <summary>
        /// Fill the locations dictionary, which maps the Archipelago's numeric location ids to data we can use internally
        /// </summary>
        //private async Task LoadLocationDictionary()
        //{
        //    var alllocations = session.locations.alllocations.toarray();
        //    var items = (await session.locations.scoutlocationsasync(false, alllocations)).locations;
        //    // todo: parrallelism overkill, make it procedural.
        //    parallel.foreach (
        //        items,
        //        new paralleloptions
        //        {
        //            maxdegreeofparallelism = convert.toint32(math.ceiling((environment.processorcount * 0.75) * 2.0))
        //        },
        //        item =>
        //        {
        //            try
        //            {
        //                var locationid = item.location;
        //                var itemname = session.items.getitemname(item.item);
        //                var locationname = session.locations.getlocationnamefromid(locationid);
        //                var location = new location()
        //                {
        //                    itemid = item.item,
        //                    itemname = itemname,
        //                    name = locationname,
        //                    player = item.player,
        //                    extendedlabel = $"{players[item.player].name}'s {itemname}"
        //                };
        //                if (main.isresearchlocation(locationid))
        //                {
        //                    researchlocations[locationid] = location;
        //                }
        //                else
        //                {
        //                    log.error($"unknown location id: {locationid}");
        //                }
        //            }
        //            catch (exception ex) { log.error(ex.message + "\n" + ex.stacktrace); }
        //        });
        //}

        //todo: tech print techs need count ripping out
        //Remove hidden requirements on all research
        //discovered letter shit
        //if this doesn't work we go back to creating our own ResearchDefs
        // Ship research uses a parent name tag, because of course it fucking does

        //mechinator requirements are interesting? leave them in?

        //Remember that we need both hidden real technology with stripped requirements, 
        //and fake technology that will link to the real technology

        //remember we need to send out our own checks!


        /// <summary>
        /// seems clunky, but here we combine the research segment of Locations with the research-only metadata,
        /// output them as RimWorld ResearchProjectDefs, and add them to the Archipelago research tab
        /// </summary>
        private void LoadResearchDefs()
        {
            DisableNormalResearch();

            // get archipelago research tab
            var tab = DefDatabase<ResearchTabDef>.GetNamed("AD_Archipelago");
            var researchesBefore = DefDatabase<ResearchProjectDef>.DefCount;
            var scenarioDef = DefDatabase<ScenarioDef>.AllDefs;
            var techTree = JsonConvert.DeserializeObject<Dictionary<long, LocationResearchMetaData>>(SlotData["techTree"].ToString());

            foreach (var kvp in techTree)
            {
                var locationData = ResearchLocations[kvp.Key];
                var def = new ResearchProjectDef()
                {
                    baseCost = kvp.Value.cost,
                    defName = $"AP_{kvp.Key}",
                    description = locationData.ExtendedLabel + $" (AP_{kvp.Key})",
                    label = locationData.ExtendedLabel,
                    tab = tab,
                    researchViewX = kvp.Value.x,
                    researchViewY = kvp.Value.y,

                    //requiredResearchFacilities = Thing
                };
                AddedResearchDefs.Add(kvp.Key, def);
                Main.Instance.DefNameToArchipelagoId[def.defName] = kvp.Key;
            }
            foreach (var kvp in AddedResearchDefs)
            {
                kvp.Value.prerequisites = techTree[kvp.Key].prerequisites.Select(x => AddedResearchDefs[x]).ToList();
            }

            DefDatabase<ResearchProjectDef>.Add(AddedResearchDefs.Values);
            var researchesAfter = DefDatabase<ResearchProjectDef>.DefCount;
            ResearchProjectDef.GenerateNonOverlappingCoordinates();
        }

        private void DisableNormalResearch()
        {
            // use our def map, not all ResearchProjectDefs, in case there are researches that we will not get from Archipelago e.g. from mods
            var researchDefNames = Main.Instance.ArchipeligoItemIdToRimWorldDef.Values.Where(def => def.DefType == "ResearchProjectDef").Select(def => def.DefName);
            foreach (var researchName in researchDefNames)
            {
                var def = DefDatabase<ResearchProjectDef>.GetNamed(researchName);
                if (def == null)
                {
                    Log.Error($"Could not find expected ResearchProjectDef by name {researchName}");
                }
                else
                {
                    // Removing all research prequisites stops
                    // later technoligies back-filling earlier requirements.
                    def.prerequisites = new List<ResearchProjectDef>();
                    def.hiddenPrerequisites = new List<ResearchProjectDef>();
                    def.techprintCount = 0;
                    def.techprintCommonality = 0;
                    // Hide technologies on main tab to stop vanilla research.
                    def.tab = null;
                }
            }
        }

        private void AddSessionHooks()
        {
            //Session.Items.ItemReceived += (receivedItemsHelper) =>
            //{
            //    var itemReceivedName = receivedItemsHelper.PeekItemName();
            //    Log.Message($"Received Item: {itemReceivedName}");
            //    var networkItem = receivedItemsHelper.DequeueItem();
            //    ComponentStateManager.ReceiveItem(networkItem.Item);
            //};

            //Session.MessageLog.OnMessageReceived += (message) =>
            //{
            //    foreach (var part in message.Parts)
            //    {
            //        Find.LetterStack.ReceiveLetter(part.Text, part.Text, LetterDefOf.NeutralEvent);
            //        Messages.Message(part.Text, MessageTypeDefOf.SilentInput, false);
            //        Log.Message(part.Text);
            //    }
            //};
        }
    }
}
