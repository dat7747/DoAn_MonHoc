using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class AssociationRule
    {
        public HashSet<string> X { get; set; }
        public HashSet<string> Y { get; set; }
        public double Support { get; set; }
        public double Confidence { get; set; }

        public AssociationRule(HashSet<string> x, HashSet<string> y, double support, double confidence)
        {
            X = x;
            Y = y;
            Support = support;
            Confidence = confidence;
        }
    }
}