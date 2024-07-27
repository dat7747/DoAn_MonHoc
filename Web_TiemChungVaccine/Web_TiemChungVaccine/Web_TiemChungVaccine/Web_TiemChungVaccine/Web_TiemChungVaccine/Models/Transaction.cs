using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public class Transaction
    {
        public List<string> Items { get; set; }

        public Transaction(List<string> items)
        {
            Items = items;
        }
    }
}