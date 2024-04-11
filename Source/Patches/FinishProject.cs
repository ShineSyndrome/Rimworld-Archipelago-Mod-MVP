﻿using HarmonyLib;
using RimWorld;
using Verse;
using RimworldArchipelago.Client;
using RimWorld.Planet;

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
