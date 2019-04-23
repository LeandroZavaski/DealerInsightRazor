using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealerInsightRazor.DataAccess;
using DealerInsightRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DealerInsightRazor.Pages
{
    public class ReportModel : PageModel
    {
        public ReportEmbedConfig ReportDetail { get; private set; }

        private readonly ReportRepository _reportRepo;

        /// <param name="reportRepo"></param>
        public ReportModel(ReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }

        /// <returns></returns>
        public async Task OnGetAsync(Guid reportID, string name, string role)
        {
            if (name == null && role == null)
                ReportDetail = await _reportRepo.GetReportForIdAsync(reportID);
            else
                ReportDetail = await _reportRepo.GetReportForIdAsync(reportID, name, role);
        }
    }
}