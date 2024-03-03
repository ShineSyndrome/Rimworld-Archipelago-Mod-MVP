﻿using HarmonyLib;
using HugsLib.Utils;
using RimWorld;
using RimworldArchipelago.Client;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimworldArchipelago.Patches
{
    [HarmonyPatch(typeof(MainMenuDrawer), nameof(MainMenuDrawer.DoMainMenuControls))]
    public static class DoMainMenuControls
    {
        public static bool drawing;

        static bool Prefix(ref Rect rect) 
        {
            drawing = true;
            rect.height += 45f;

            return true;
        }

        static void Postfix()
        {
            drawing = false;
        } 
    }

    [HarmonyPatch(typeof(OptionListingUtility), nameof(OptionListingUtility.DrawOptionListing))]
    public static class DrawOptionListing
    {
        static void Prefix(Rect rect, List<ListableOption> optList)
        {
            if (!DoMainMenuControls.drawing)
            {
                return;
            }

            if (Current.ProgramState != ProgramState.Entry)
            {
                return;
            }
            
            int newColony = optList.FindIndex(opt => opt.label == "NewColony".Translate());

            if (newColony == -1)
            {
                return;
            }

            if (Main.Instance.Session == null)
            {
                optList.Insert(newColony + 1, new ListableOption("Archipelago", () =>
                {
                    Find.WindowStack.Add(new ArchipelagoOptionsMenu());
                }));
            }
            else
            {
                optList.Insert(newColony + 1, new ListableOption("Connected!", () =>
                {
                    Messages.Message("RimWorld can't unload Archipelago's changes.\nRestart RimWorld to connect to a different Archipelago server or player slot.",
                        MessageTypeDefOf.SilentInput, false);
                }));
            }   
        }
    }


    public class ArchipelagoOptionsMenu : Window
    {
        public string address = Main.Instance.Address;
        public string slotName = Main.Instance.PlayerSlot;
        public string acceptBtnLabel;
        public string closeBtnLabel;

        public ArchipelagoOptionsMenu()
        {
            closeOnClickedOutside = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnAccept = true;
            acceptBtnLabel = "OK".Translate();
            closeBtnLabel = "Cancel".Translate();
        }

        public override void DoWindowContents(Rect inRect)
        {
            const float margin = 10f;
            var yPos = 35f;
            GUI.Label(new Rect(margin, yPos + 5, 80, 20), "Address:");
            address = Widgets.TextField(new Rect(120, yPos, inRect.width - 120 - margin, 35f), address);
            yPos += 35f + margin;
            GUI.Label(new Rect(margin, yPos + 5, 80, 20), "Slot Name:");
            slotName = Widgets.TextField(new Rect(120, yPos, inRect.width - 120 - margin, 35f), slotName);

            var btnsRect = new Rect(0f, inRect.height - 35f - 5f, closeBtnLabel != null ? 210 : 120, 35f).CenteredOnXIn(inRect);

            if (Widgets.ButtonText(btnsRect.LeftPartPixels(closeBtnLabel != null ? 100 : 120), acceptBtnLabel, true, false))
            {
                if (applySettings())
                {
                    Close();
                }
            }

            if (closeBtnLabel != null)
                if (Widgets.ButtonText(btnsRect.RightPartPixels(100), closeBtnLabel, true, false))
                    Close();
        }
        public override void OnAcceptKeyPressed()
        {
            base.OnAcceptKeyPressed();
            applySettings();
        }

        private bool applySettings()
        {
            Log.Message($"address: {address}");
            Log.Message($"slotName: {slotName}");
            return Main.Instance.Connect(address, slotName);
        }
    }
}
