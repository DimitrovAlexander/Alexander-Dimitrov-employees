using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PairOfEmployeesSirma.Models;
using PairOfEmployeesSirma.Services;

namespace PairOfEmployeesSirma.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly EmployeeService _service;
        public IndexModel(ILogger<IndexModel> logger, EmployeeService service)
        {
            _logger = logger;
            _service = service;
        }
        [BindProperty]
        public IFormFile UploadedFile { get; set; }

        public List<EmployeePairResult> AllPairs { get; set; }
        public EmployeePairResult TopPair { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {

            if (UploadedFile != null)
            {
                using var stream = UploadedFile.OpenReadStream();
                var data = _service.LoadFromCsv(stream);
                var longestPair = _service.FindLongestPair(data);

                AllPairs = longestPair.AllPairs;
                TopPair = longestPair.LongestPair;
            }

            return Page();
        }
    }
}
