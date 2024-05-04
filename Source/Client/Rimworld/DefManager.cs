using Archipelago.MultiClient.Net.Enums;
using RimWorld;
using RimworldArchipelago.Client.Data;
using RimworldArchipelago.Client.Multiworld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimworldArchipelago.Client.Rimworld
{
    public class DefManager
    {
        public const string archiResearchTab = "AD_Archipelago";
        public const string locationTag = "ArchiLocation";
        public const string coreTag = "CoreLocation";

        public void LoadResearchDefs(IEnumerable<HydratedLocation> researchLocations)
        {
            DisableOriginalResearch();
            LoadResearch(researchLocations);
        }

        private void LoadResearch(IEnumerable<HydratedLocation> networkResearchLocations)
        {
            var archiResearchLocationDefs = DefDatabase<ResearchProjectDef>.AllDefs
                .Where(x => x.tags != null && x.tags.Any(t => t.defName == locationTag))
                .ToList();

            var archiResearchTab = DefDatabase<ResearchTabDef>.GetNamed("AD_Archipelago");

            foreach (var researchLocationDef in archiResearchLocationDefs)
            {
                var vanillaDefName = researchLocationDef.defName.Substring(0, researchLocationDef.defName.LastIndexOf("Location"));

                var networkLocation = networkResearchLocations
                    .SingleOrDefault(x => vanillaDefName == ResearchLocations.AllResearchLocations[x.NetworkItem.Location]);

                if (networkLocation != null)
                {
                    researchLocationDef.label = networkLocation.ItemLabel;
                    researchLocationDef.tab = archiResearchTab;

                    researchLocationDef.description = BuildResearchDescription(networkLocation);

                    continue;
                }
                //else not part of mw game

                var vanillaDef = DefDatabase<ResearchProjectDef>.GetNamed(vanillaDefName, false);
                    
                //Completed already, therefore probably from starting scenario and needs completing. 
                if (vanillaDef != null && vanillaDef.IsFinished)
                {
                    researchLocationDef.label = vanillaDef.label;
                    researchLocationDef.description = vanillaDef.description;
                    Find.ResearchManager.FinishProject(researchLocationDef);
                    continue;
                }

                //likely expansion that isn't here
                if (vanillaDef == null)
                {
                    researchLocationDef.tab = null;
                    continue;
                }

                //research location with corresponding vanilla location (not finished) that isn't part of mw
                //This means something has gone very wrong.
                throw new InvalidOperationException($"Error loading research locations, {researchLocationDef.defName} not in MW location list and not handled otherwise");
                
            }
        }

        private void DisableOriginalResearch()
        {
            var originalResearch = DefDatabase<ResearchProjectDef>.AllDefs
                .Where(r => r.tags == null || !r.tags.Any(t => t.defName == locationTag))
                .ToList();

            foreach (var research in originalResearch)
            {
                // Removing all research prequisites stops
                // later technoligies back-filling earlier requirements.
                research.prerequisites = new List<ResearchProjectDef>();
                research.hiddenPrerequisites = new List<ResearchProjectDef>();
                research.techprintCount = 0;
                research.techprintCommonality = 0;
                research.tab = null;
            }
        }

        private static string BuildResearchDescription(HydratedLocation researchLocation)
        {
            var importanceLabel = "a regular";

            if (researchLocation.NetworkItem.Flags == ItemFlags.Advancement)
            {
                importanceLabel = "an important";
            }

            if (researchLocation.SelfItem)
            {
                return $"Complete this research project to receive {researchLocation.ItemLabel}. This is deemed to be {importanceLabel} item for your playthrough.";
            }

            return $@"
Completing this research project will unlock {researchLocation.ItemLabel}. This is {importanceLabel} item for their game.

The Archotech network spans beyond mortal reckoning. By lending your intelligence to the Archonexus you may alter the reality of distant worlds, gifting them useful boons.

These remote civilisations have strange and alien customs, but they will surely reciprocate aid given.
";
        }
    }
}
