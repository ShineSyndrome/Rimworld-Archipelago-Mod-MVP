using RimWorld;
using RimworldArchipelago.Client.Data;
using RimworldArchipelago.Client.Multiworld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimworldArchipelago.Client.Rimworld
{
    public class DefManager
    {
        public const string archiResearchTab = "AD_Archipelago";
        public const string locationTag = "ArchiLocation";

        public void LoadResearchDefs(IEnumerable<Location> researchLocations)
        {
            DisableOriginalResearch();
            LoadResearch();
        }

        private void LoadResearch()
        {
            var archiResearchLocationDefs = DefDatabase<ResearchProjectDef>.AllDefs
                .Where(x => x.tags.Any(t => t.defName == locationTag))
                .ToList();

            var archiResearchTab = DefDatabase<ResearchTabDef>.GetNamed("AD_Archipelago");

            foreach (var researchLocation in archiResearchLocationDefs)
            {
                researchLocation.tab = archiResearchTab;
            }


        }

        private void DisableOriginalResearch()
        {
            //todo: scope this to just vanilla
            var originalResearch = DefDatabase<ResearchProjectDef>.AllDefs
                .Where(r => !r.tags.Any(t => t.defName == locationTag))
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
    }
}
