using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Infrastructure.Settings
{
    public class OcrSettings
    {
        public const string SectionName = "OcrSettings";
        public string ApiUrl { get; set; } = string.Empty;
    }
}
