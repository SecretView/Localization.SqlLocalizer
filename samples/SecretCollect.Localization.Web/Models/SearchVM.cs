using System.Collections.Generic;

namespace SecretCollect.Localization.Web.Models
{
    public class SearchVM
    {
        public string Query { get; set; }
        public IEnumerable<SearchResultItemVM> Items { get; set; }
    }
}
