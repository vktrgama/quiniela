using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quiniela.Entities
{
    public class ExchangeRate
    {
        public string to { get; set; }
        public double rate { get; set; }
        public string from { get; set; }
    }
}