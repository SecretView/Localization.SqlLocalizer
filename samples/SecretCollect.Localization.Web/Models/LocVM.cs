using SecretCollect.Localization.SqlLocalizer.Data;
using System;

namespace SecretCollect.Localization.Web.Models
{
    public class LocVM
    {
        public Guid CultureId { get; set; }
        public string CultureName { get; set; }
        public string Translation { get; set; }
        public RecordStatus Status { get; set; }
        public DateTime LastUsed { get; internal set; }
    }
}
