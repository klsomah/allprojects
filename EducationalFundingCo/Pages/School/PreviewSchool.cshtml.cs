using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Data;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using EducationalFundingCo.Utilities;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using EducationalFundingCo.Areas.Identity.Data;

namespace EducationalFundingCo.Pages.School
{
    [Authorize(Roles = "Administrator")]
    public class PreviewSchoolModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly IConfiguration _config;

        public PreviewSchoolModel(EducationalFundingCoContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

      


        [BindProperty]
        public Areas.Identity.Data.School Schools { get; set; }

        [BindProperty]
        public List<USState> USStateValues { get; set; }

        [BindProperty]
        public Areas.Identity.Data.USState USStates { get; set; }

        [TempData]
        public int PreviewId { get; set; }

        [TempData]
        public string PreviewMessage { get; set; }
      
         
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["USAState"] = new SelectList(_context.USState.Where(x=>x.Id ==Schools.USStateId), "Id", "Name");

            //ViewData["Choices"] = new SelectList(selectedChoices, "Id", "Name", new List<string> { });

            PreviewId = 0;
            PreviewMessage = string.Empty;

            if (id == null)
            {
                return NotFound();
            }

            Schools = await _context.School
               .Include(c => c.USState).FirstOrDefaultAsync(m => m.Id == id);

            PreviewId = Schools.Id;

            if (Schools == null)
            {
                return NotFound();
            }

            return await LoadData(Schools.Id);
        }
 


        private async Task<IActionResult> LoadData(int id)
        {
            ViewData["Choice"] = new SelectList(_context.LearningSolution, "Id", "Name", new List<string> { });
            List<Object> selectedChoices = new List<Object>();
            var getChoices = await Task.Run(() => _context.SchoolLearningSolution.Where(x => x.SchoolId == id).ToList());
            foreach (var item in getChoices)
            {
                var totalChoices = await Task.Run(() => _context.LearningSolution.Where(x => x.Id == item.LearningSolutionId).FirstOrDefault());
                selectedChoices.Add(totalChoices);
            }
           
            ViewData["Choices"] = new SelectList(selectedChoices, "Id", "Name", new List<string> { });
            Schools = await _context.School
               .Include(c => c.USState).FirstOrDefaultAsync(m => m.Id == id);


            
            //USStateValues = await _context.School.Include(x=>x.USState == id).ToList();
            //ViewData["USStateValues"] = new SelectList(_context.USState, "Id", "Name", new List<string> { });
            //List<Object> selectedstates = new List<Object>();

            


            //USStateValues = await Task.Run(() => _context.USState.ToListAsync());



            if (Schools == null)
            {
                return NotFound();
            }

            return Page();

        }


        private bool SchoolExists(int id)
        {
            return _context.School.Any(e => e.Id == id);
        }
 
    }
}
