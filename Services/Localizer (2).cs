using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace quiniela.Services
{
    public static class Localizer
    {
        public static string Get(string key)
        {
            var culture = CultureInfo.CurrentCulture;

            if (HttpContext.Current.Session["cultureInfo"] == null) {
                HttpContext.Current.Session["cultureInfo"] = culture;
            }

            if (HttpContext.Current.Session["cultureInfo"].ToString() != culture.ToString())
            {
                culture = HttpContext.Current.Session["cultureInfo"] as CultureInfo;
            }

            return Resources.quiniela.ResourceManager.GetString(key, culture);
        }

        public static void SetCulture(string culture)
        {
            if (!string.IsNullOrEmpty(culture)) {
                HttpContext.Current.Session["cultureInfo"] = new CultureInfo(culture);
            }
        }

        public static CultureInfo GetCulture()
        {
            var culture = CultureInfo.CurrentCulture;
            if (HttpContext.Current.Session["cultureInfo"] != null) {
                culture = HttpContext.Current.Session["cultureInfo"] as CultureInfo;
            }

            return culture;
        }
    }
}