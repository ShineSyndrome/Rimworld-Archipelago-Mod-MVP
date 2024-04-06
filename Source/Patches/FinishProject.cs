using HarmonyLib;
using RimWorld;
using Verse;
using RimworldArchipelago.Client;

namespace RimworldArchipelago.Patches
{
    [HarmonyPatch(typeof(ResearchManager),nameof(ResearchManager.FinishProject))]
    public static class FinishProject
    {
        public static void Postfix(ref ResearchProjectDef proj)
        {
            if (Main.Instance.DefNameToArchipelagoId.ContainsKey(proj.defName))
            {
                Main.Instance.SendLocationCheck(proj.defName);
            }
        }
    }
}
