using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RandomsGeneAssistant
{
    internal static class GenepackTradeStatus
    {
        // Ordered so Min() selects the least-covered non-ignored gene.
        internal enum Status
        {
            // Not found in any owned genepack.
            Missing,
            // Found only in genepacks that also contain genes outside the target pack.
            Mixed,
            // Found in an owned genepack containing only genes from the target pack.
            Reproducible,
            // Ignored genes do not lower the result; all ignored means gray.
            Ignored
        }

        internal static Status GetStatus(Genepack genepack, IEnumerable<Thing> banks)
        {
            Dictionary<GeneDef, Status> status = new Dictionary<GeneDef, Status>();
            HashSet<GeneDef> ignoredGenes = SettingsRef.ignoredGenes;
            foreach (GeneDef gene in genepack.GeneSet.GenesListForReading)
            {
                status[gene] = ignoredGenes.Contains(gene) ? Status.Ignored : Status.Missing;
            }

            if (status.Count == 0)
            {
                return Status.Missing;
            }

            Status minimum = status.Values.Min();
            if (minimum == Status.Ignored)
            {
                return minimum;
            }

            foreach (Thing bank in banks)
            {
                CompGenepackContainer container = bank.TryGetComp<CompGenepackContainer>();
                if (container == null)
                {
                    continue;
                }

                foreach (Genepack owned in container.ContainedGenepacks)
                {
                    List<GeneDef> ownedGenes = owned.GeneSet.GenesListForReading;
                    if (ownedGenes.TrueForAll(status.ContainsKey))
                    {
                        foreach (GeneDef gene in ownedGenes)
                        {
                            if (status[gene] != Status.Ignored)
                            {
                                status[gene] = Status.Reproducible;
                            }
                        }
                    }
                    else
                    {
                        foreach (GeneDef gene in ownedGenes)
                        {
                            if (status.ContainsKey(gene) && status[gene] == Status.Missing)
                            {
                                status[gene] = Status.Mixed;
                            }
                        }
                    }

                    minimum = status.Values.Min();
                    if (minimum == Status.Reproducible)
                    {
                        return minimum;
                    }
                }
            }

            return minimum;
        }
    }
}
