using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecretCollect.Localization.Web.Models
{
    public class CultureVM
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsSupported { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid? FallbackCultureId { get; set; }

        public IEnumerable<(Guid Id, string Name)> OtherCultures { get; set; }
        public SaveResult SaveResult { get; set; } = SaveResult.None;
    }
}
