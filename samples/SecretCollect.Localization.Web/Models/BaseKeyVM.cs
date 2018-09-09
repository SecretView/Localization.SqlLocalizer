using SecretCollect.Localization.SqlLocalizer.Data;
using System.Collections.Generic;

namespace SecretCollect.Localization.Web.Models
{
    public class BaseKeyVM
    {
        public string Base { get; set; }
        public IEnumerable<LocalizationKey> Keys { get; set; }
    }
}
