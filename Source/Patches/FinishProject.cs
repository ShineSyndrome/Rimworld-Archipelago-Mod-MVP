using HarmonyLib;
using RimWorld;
using Verse;
using RimworldArchipelago.Client;
using RimworldArchipelago.Client.Services;

namespace RimworldArchipelago.Patches
{
    [HarmonyPatch(typeof(ResearchManager),nameof(ResearchManager.FinishProject))]
    public static class FinishProject
    {
        //public static bool Prefix(ref ResearchProjectDef proj, ref bool doCompletionDialog, ref Pawn researcher, ref bool doCompletionLetter)
        //{
        //    if (Main.Instance.DefNameToArchipelagoId.ContainsKey(proj.defName))
        //    {
        //        Main.Instance.SendLocationCheck(proj.defName);
        //    }

        //    return true;
        //}

        public static void Postfix(ref ResearchProjectDef proj)
        {
            if (MultiWorldService.Instance.DefNameToArchipelagoId.ContainsKey(proj.defName))
            {
                Main.Instance.SendLocationCheck(proj.defName);
            }
        }
    }
}
