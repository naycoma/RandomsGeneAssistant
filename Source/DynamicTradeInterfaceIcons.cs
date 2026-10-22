using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RandomsGeneAssistant
{
    public static class DynamicTradeInterfaceIcons
    {
        private const string IconPath = "Things/Item/Special/Genepack/Genepack_e";

        public static IEnumerable<(Texture, string, Color?)> GetIcons(Tradeable tradeable)
        {
            if (!TryGetStatus(tradeable, out GenepackTradeStatus.Status status))
            {
                return Enumerable.Empty<(Texture, string, Color?)>();
            }

            Texture2D icon = ContentFinder<Texture2D>.Get(IconPath, false);
            if (icon == null)
            {
                return Enumerable.Empty<(Texture, string, Color?)>();
            }

            Color color = status == GenepackTradeStatus.Status.Ignored ? Color.gray :
                status == GenepackTradeStatus.Status.Reproducible ? Color.green :
                status == GenepackTradeStatus.Status.Mixed ? Color.yellow : Color.red;
            return new[] { ((Texture)icon, TooltipKey(status).Translate().ToString(), (Color?)color) };
        }

        public static string GetSearchString(Tradeable tradeable)
        {
            return TryGetStatus(tradeable, out GenepackTradeStatus.Status status) ? TooltipKey(status).Translate() : string.Empty;
        }

        private static bool TryGetStatus(Tradeable tradeable, out GenepackTradeStatus.Status status)
        {
            status = GenepackTradeStatus.Status.Missing;
            if (!(tradeable.AnyThing is Genepack genepack) || genepack.GeneSet == null || genepack.GeneSet.GenesListForReading.Count == 0)
            {
                return false;
            }

            Map map = Find.CurrentMap ?? Find.AnyPlayerHomeMap;
            if (map == null || !map.IsPlayerHome)
            {
                return false;
            }

            status = GenepackTradeStatus.GetStatus(genepack, map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.GenepackHolder)));
            return true;
        }

        private static string TooltipKey(GenepackTradeStatus.Status status)
        {
            return status == GenepackTradeStatus.Status.Ignored ? "RandomsGeneAssistant.TradeStatusIgnored" :
                status == GenepackTradeStatus.Status.Reproducible ? "RandomsGeneAssistant.TradeStatusReproducible" :
                status == GenepackTradeStatus.Status.Mixed ? "RandomsGeneAssistant.TradeStatusMixed" : "RandomsGeneAssistant.TradeStatusMissing";
        }
    }
}
