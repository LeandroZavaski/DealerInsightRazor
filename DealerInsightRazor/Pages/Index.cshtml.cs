using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealerInsightRazor.DataAccess;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.PowerBI.Api.V2.Models;

namespace DealerInsightRazor.Pages
{
    public class IndexModel : PageModel
    {
        public List<Report> AvailableReports { get; private set; }
        private readonly ReportRepository _reportRepo;

        public IndexModel(ReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }
        public async Task OnGetAsync()
        {
            //var reports = await _reportRepo.GetAvailableReportsAsync();
            //AvailableReports = reports.ToList();
        }
    }
}
