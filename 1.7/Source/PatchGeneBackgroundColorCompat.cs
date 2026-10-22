using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RandomsGeneAssistant
{
    [HarmonyPatch(typeof(GeneUIUtility), nameof(GeneUIUtility.DrawGeneBasics))]
    public static class PatchGeneBackgroundColorCompat
    {
        [HarmonyPrefix]
        public static void Prefix(GeneDef gene, Rect geneRect, GeneType geneType, ref bool doBackground, bool clickable, bool overridden)
        {
            PatchGeneBackgroundColor.DrawGeneBasics(gene, geneRect, geneType, ref doBackground, clickable, overridden);
        }
    }
}
