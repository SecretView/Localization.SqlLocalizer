using SecretCollect.Localization.SqlLocalizer.Data;
using System;
using System.Collections.Generic;

namespace SecretCollect.Localization.Web.Models
{
    public class MissingLocalizationsVM
    {
        public string CultureName { get; set; }
        public Guid CultureId { get; set; }
        public IEnumerable<LocalizationKey> LocalizationKeys { get; set; }
    }
}
