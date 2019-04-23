using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealerInsightRazor.Models
{
    public class ReportEmbedConfig
    {
        /// <summary>
        /// The Power BI report ID
        /// </summary>
        public Guid ReportID { get; set; }

        /// <summary>
        /// Name of report
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Access token associated with report
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Embed Url for report
        /// </summary>
        public string EmbedUrl { get; set; }
    }
}
