using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace RandomsGeneAssistant
{
    [HarmonyPatch]
    public static class PatchTraderColorChange
    {
        [HarmonyPatch(typeof(RimWorld.TradeUI), nameof(RimWorld.TradeUI.DrawTradeableRow))]
        [HarmonyPrefix]
        public static void DrawTradeableRow_Prefix(Rect rect, Tradeable trad, int index)
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
