using Cotur.DataMining.Association;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class Result
    {
        public List<HashSet<string>> FrequentItemsets { get; set; }
        public List<AssociationRule> Rules { get; set; }

        public Result()
        {
            FrequentItemsets = new List<HashSet<string>>();
            Rules = new List<AssociationRule>();
        }
    }
}