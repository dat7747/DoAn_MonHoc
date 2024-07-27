using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class Apriori
    {
        public double MinSupport { get; set; }
        public double MinConfidence { get; set; }

        public Apriori(double minSupport, double minConfidence)
        {
            MinSupport = minSupport;
            MinConfidence = minConfidence;
        }

        public Result Run(List<Transaction> transactions)
        {
            Result result = new Result();
            List<HashSet<string>> frequentItemsets = new List<HashSet<string>>();
            Dictionary<HashSet<string>, int> itemsetCounts = new Dictionary<HashSet<string>, int>(HashSet<string>.CreateSetComparer());

            // Step 1: Generate initial candidate itemsets (C1)
            foreach (var transaction in transactions)
            {
                foreach (var item in transaction.Items)
                {
                    var itemset = new HashSet<string> { item };
                    if (!itemsetCounts.ContainsKey(itemset))
                    {
                        itemsetCounts[itemset] = 0;
                    }
                    itemsetCounts[itemset]++;
                }
            }

            // Log C1 itemsets and their counts
            Debug.WriteLine("C1 Itemsets and Counts:");
            foreach (var kvp in itemsetCounts)
            {
                Debug.WriteLine($"{string.Join(", ", kvp.Key)}: {kvp.Value}");
            }

            // Step 2: Filter itemsets by support threshold
            itemsetCounts = itemsetCounts.Where(kvp => (double)kvp.Value / transactions.Count >= MinSupport)
                                         .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, HashSet<string>.CreateSetComparer());

            frequentItemsets.AddRange(itemsetCounts.Keys);

            // Log L1 frequent itemsets
            Debug.WriteLine("L1 Frequent Itemsets:");
            foreach (var itemset in frequentItemsets)
            {
                Debug.WriteLine(string.Join(", ", itemset));
            }

            // Step 3: Generate larger itemsets from frequent itemsets
            int k = 2;
            while (itemsetCounts.Count > 0)
            {
                var candidateItemsets = GenerateCandidates(itemsetCounts.Keys.ToList(), k);
                itemsetCounts.Clear();

                foreach (var transaction in transactions)
                {
                    foreach (var candidate in candidateItemsets)
                    {
                        if (candidate.IsSubsetOf(transaction.Items))
                        {
                            if (!itemsetCounts.ContainsKey(candidate))
                            {
                                itemsetCounts[candidate] = 0;
                            }
                            itemsetCounts[candidate]++;
                        }
                    }
                }

                itemsetCounts = itemsetCounts.Where(kvp => (double)kvp.Value / transactions.Count >= MinSupport)
                                             .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, HashSet<string>.CreateSetComparer());

                frequentItemsets.AddRange(itemsetCounts.Keys);

                // Log frequent itemsets at each step
                Debug.WriteLine($"L{k} Frequent Itemsets:");
                foreach (var itemset in itemsetCounts.Keys)
                {
                    Debug.WriteLine(string.Join(", ", itemset));
                }

                k++;
            }

            result.FrequentItemsets = frequentItemsets;

            // Step 4: Generate association rules
            foreach (var itemset in frequentItemsets.Where(f => f.Count > 1))
            {
                foreach (var subset in GetSubsets(itemset, itemset.Count - 1))
                {
                    var remaining = new HashSet<string>(itemset);
                    remaining.ExceptWith(subset);

                    int itemsetCount = 0;
                    int subsetCount = 0;

                    // Kiểm tra xem itemset và subset có tồn tại trong từ điển không
                    if (itemsetCounts.ContainsKey(itemset))
                    {
                        itemsetCount = itemsetCounts[itemset];
                    }
                    if (itemsetCounts.ContainsKey(subset))
                    {
                        subsetCount = itemsetCounts[subset];
                    }

                    double support = (double)itemsetCount / transactions.Count;
                    double confidence = subsetCount != 0 ? (double)itemsetCount / subsetCount : 0;

                    if (confidence >= MinConfidence)
                    {
                        var rule = new AssociationRule(subset, remaining, support, confidence);
                        result.Rules.Add(rule);

                        // Log each generated rule
                        Debug.WriteLine($"Generated Rule: {string.Join(", ", rule.X)} => {string.Join(", ", rule.Y)} (Support: {rule.Support}, Confidence: {rule.Confidence})");
                    }
                }
            }

            return result;
        }

        private List<HashSet<string>> GenerateCandidates(List<HashSet<string>> previousItemsets, int k)
        {
            var candidates = new List<HashSet<string>>();

            foreach (var itemset1 in previousItemsets)
            {
                foreach (var itemset2 in previousItemsets)
                {
                    var union = new HashSet<string>(itemset1);
                    union.UnionWith(itemset2);

                    if (union.Count == k)
                    {
                        candidates.Add(union);
                    }
                }
            }

            return candidates;
        }

        private IEnumerable<HashSet<string>> GetSubsets(HashSet<string> itemset, int length)
        {
            var subsets = new List<HashSet<string>>();

            foreach (var combination in Combinations(itemset.ToList(), length))
            {
                subsets.Add(new HashSet<string>(combination));
            }

            return subsets;
        }

        private IEnumerable<IEnumerable<T>> Combinations<T>(IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
                elements.SelectMany((e, i) =>
                    Combinations(elements.Skip(i + 1), k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }

}