using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using System.Reflection;

namespace RandomsGeneAssistant
{
    public static class PatchTradeUIRevisited
    {
        public static void HandlePatch(Harmony har)
        {
            if (LoadedModManager.RunningModsListForReading.Any(m => m.PackageId == "hobtook.tradeui")) {
                har.Patch(TargetMethod(), new HarmonyMethod(typeof(PatchTradeUIRevisited).GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public)));
            }
        }

        public static MethodBase TargetMethod()
        {
            MethodInfo mi = AccessTools.FirstMethod(AccessTools.TypeByName("Harmony_DialogTrade_FillMainRect"), m => m.Name == "MyDrawTradableRow");
            return mi;
        }


        [HarmonyPrefix]
        public static void Prefix(Rect mainRect, Tradeable trad, int index, bool isOurs)
        {
            if (trad.ThingDef != ThingDefOf.Genepack) { return; }
            Map map = Find.CurrentMap;
            if (map == null)
            {
                map = Find.AnyPlayerHomeMap;
            }
            if (map == null || !map.IsPlayerHome) { return; }

            //  Get the genepack
            Genepack genepack = (Genepack)trad.AnyThing;
            if (genepack == null) { return; }

            //  Get genebanks
            List<Thing> banks = new List<Thing>(map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.GenepackHolder)));
            if ((banks == null) || (banks.Count == 0)) { return; }

            GenepackTradeStatus.Status status = GenepackTradeStatus.GetStatus(genepack, banks);

            Rect rect = mainRect;
            //  Draw our own background
            Rect rect1 = new Rect(rect.width - 75f, 0.0f, 75f, rect.height);
            Color c = SettingsRef.red;
            if (status == GenepackTradeStatus.Status.Mixed)
            {
                c = SettingsRef.yellow;
            }
            else if (status == GenepackTradeStatus.Status.Reproducible || status == GenepackTradeStatus.Status.Ignored)
            {
                c = SettingsRef.green;
            }
            Widgets.DrawBoxSolid(rect, c);
            GUI.color = Color.white;
        }
    }
}
