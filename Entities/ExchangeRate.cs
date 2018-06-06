using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quiniela.Entities
{
    public class ExchangeRate
    {
        public CurrencyValue USD_MXN { get; set; }
    }

    public class CurrencyValue
    {
        public double val { get; set; }
    }
}