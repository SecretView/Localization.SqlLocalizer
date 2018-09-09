using System;
using System.Collections.Generic;

namespace SecretCollect.Localization.Web.Models
{
    public class KeyVM
    {
        public Guid Id { get; set; }
        public string Base { get; set; }
        public string Main { get; set; }
        public string Comment { get; set; }
        public SaveResult SaveResult { get; set; } = SaveResult.None;

        public IEnumerable<LocVM> Translations { get; set; }
    }
}
