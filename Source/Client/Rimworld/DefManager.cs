using Newtonsoft.Json;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimworldArchipelago.Client.Rimworld
{
    internal class DefManager
    {
        private void LoadResearchDefs()
        {
            DisableNormalResearch();

            //// get archipelago research tab
            //var tab = DefDatabase<ResearchTabDef>.GetNamed("AD_Archipelago");
            //var researchesBefore = DefDatabase<ResearchProjectDef>.DefCount;
            //var scenarioDef = DefDatabase<ScenarioDef>.AllDefs;
            //var techTree = JsonConvert.DeserializeObject<Dictionary<long, LocationResearchMetaData>>(SlotData["techTree"].ToString());

            //foreach (var kvp in techTree)
            //{
            //    var locationData = ResearchLocations[kvp.Key];
            //    var def = new ResearchProjectDef()
            //    {
            //        baseCost = kvp.Value.cost,
            //        defName = $"AP_{kvp.Key}",
            //        description = locationData.ExtendedLabel + $" (AP_{kvp.Key})",
            //        label = locationData.ExtendedLabel,
            //        tab = tab,
            //        researchViewX = kvp.Value.x,
            //        researchViewY = kvp.Value.y,

            //        //requiredResearchFacilities = Thing
            //    };
            //    AddedResearchDefs.Add(kvp.Key, def);
            //    Main.Instance.DefNameToArchipelagoId[def.defName] = kvp.Key;
            //}
            //foreach (var kvp in AddedResearchDefs)
            //{
            //    kvp.Value.prerequisites = techTree[kvp.Key].prerequisites.Select(x => AddedResearchDefs[x]).ToList();
            //}

            //DefDatabase<ResearchProjectDef>.Add(AddedResearchDefs.Values);
            //var researchesAfter = DefDatabase<ResearchProjectDef>.DefCount;
            //ResearchProjectDef.GenerateNonOverlappingCoordinates();
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
    }
}
