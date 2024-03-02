using HarmonyLib;
using RimWorld;
using System.Collections;
using Verse;

namespace RimworldArchipelago.Patches
{
    [HarmonyPatch(typeof(GameVictoryUtility), nameof(GameVictoryUtility.MakeEndCredits))]
    public static class MakeEndCredits
    {
        public static void Postfix()
        {
            
            FileLog.Log("Endgame reached.");
        }
    }
}
