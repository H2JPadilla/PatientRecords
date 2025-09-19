using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using EL;
using UL;
using System.Globalization;

    
namespace AL.Areas.Admin.Controllers
{
    public class PatientController : Controller
    {
        // Create instance of BLL
        private readonly PatientBLL bll = new PatientBLL();

        // View with pagination
        public ActionResult ViewPatient(
            string patient = "",
            string drug = "",
            decimal? dosage = null,
            DateTime? date = null,
            int page = 1,
            int pageSize = 10)
        {
            // Trim leading and trailing spaces from search parameters
            if (patient != null) patient = Validations.CleanSpaces(patient);
            if (drug != null) drug = Validations.CleanSpaces(drug);

            var patients = bll.GetPatients();

            // Filtering logic 
            if (!string.IsNullOrEmpty(patient))
                patients = patients
                    .Where(p => p.Patient != null && p.Patient.IndexOf(patient, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            if (!string.IsNullOrEmpty(drug))
                patients = patients
                    .Where(p => p.DrugName != null && p.DrugName.IndexOf(drug, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            if (dosage.HasValue)
                patients = patients.Where(p => p.Dosage == dosage.Value).ToList();
            if (date.HasValue)
                patients = patients.Where(p => p.ModifiedDate.Date == date.Value.Date).ToList();

            int totalRecords = patients.Count();
            var pagedPatients = patients
                .OrderByDescending(p => p.ModifiedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Patient = patient;
            ViewBag.Drug = drug;
            ViewBag.Dosage = dosage;
            ViewBag.Date = date?.ToString("MM-dd-yyyy");
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.PageSize = pageSize;

            return View(pagedPatients);
        }

        //Returns JSON for live search
        [HttpGet]
        public JsonResult SearchPatients(
            string patient = "",
            string drug = "",
            decimal? dosage = null,
            DateTime? date = null,
            int page = 1,
            int pageSize = 10)
        {
            // Trim leading and trailing spaces from search parameters
            if (patient != null) patient = Validations.CleanSpaces(patient);
            if (drug != null) drug = Validations.CleanSpaces(drug);

            var patients = bll.GetPatients();

            if (!string.IsNullOrEmpty(patient))
                patients = patients
                    .Where(p => p.Patient != null && p.Patient.IndexOf(patient, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            if (!string.IsNullOrEmpty(drug))
                patients = patients
                    .Where(p => p.DrugName != null && p.DrugName.IndexOf(drug, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            if (dosage.HasValue)
                patients = patients.Where(p => p.Dosage == dosage.Value).ToList();
            if (date.HasValue)
                patients = patients.Where(p => p.ModifiedDate.Date == date.Value.Date).ToList();

            int totalRecords = patients.Count();
            var pagedPatients = patients
                .OrderByDescending(p => p.ModifiedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            //format the ModifiedDate to a string
            var formattedPatients = pagedPatients.Select(p => new
            {
                p.ID,
                p.Patient,
                p.DrugName,
                p.Dosage,
                ModifiedDate = p.ModifiedDate.ToString("MM-dd-yyyy") // Format as a standard date string
            }).ToList();

            return Json(new
            {
                patients = formattedPatients,
                totalPages = totalPages,
                page = page,
                pageSize = pageSize
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: Add Patient
        [HttpGet]
        public ActionResult AddPatient()
        {
            return View(new PatientEntity());
        }

        // POST: Add Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPatient(PatientEntity model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddPatient", model);
            }

            bll.AddPatient(model);
            TempData["Message"] = MessageUtil.AddSuccess;
            return RedirectToAction("ViewPatient");
        }

        // GET: Update Patient
        [HttpGet]
        public ActionResult UpdatePatient(int id)
        {
            var patient = bll.GetPatientById(id);
            if (patient == null)
            {
                TempData["Message"] = MessageUtil.RecordNotFound;
                return RedirectToAction("ViewPatient");
            }
            return View(patient);
        }

        // POST: Update Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePatient(PatientEntity model)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdatePatient", model);
            }

            bll.UpdatePatient(model);
            TempData["Message"] = MessageUtil.UpdateSuccess;
            return RedirectToAction("ViewPatient");
        }

        // POST: Delete Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePatient(int id)
        {
            bll.DeletePatient(id);
            TempData["Message"] = MessageUtil.DeleteSuccess;
            return RedirectToAction("ViewPatient");
        }

        // Validate that will be called via AJAX before form submission
        [HttpPost]
        public JsonResult ValidatePatient(string patient, string drugName, string dosage, int? id)
        {
            CultureInfo culture = new CultureInfo("en-US");
            if (!decimal.TryParse(dosage, NumberStyles.Any, culture, out decimal parsedDosage))
            {
                return Json(new { success = false, message = "Dosage must be a valid positive number.", errorType = "invalidDosage" });
            }

            if (parsedDosage <= 0)
            {
                return Json(new { success = false, message = "Dosage must be a positive number.", errorType = "invalidDosage" });
            }

            // 1. Check for a full row existence (exact duplicate) == Jabes, Alaxan, 250, 9/19/2025 == Jabes, Alaxan, 250, 9/19/2025 (Return error: Existing record)
            if (bll.Exists(patient, drugName, parsedDosage, DateTime.Now.Date, id))
            {
                return Json(new { success = false, message = MessageUtil.ExistingRecord, errorType = "alreadyExist" });
            }

            // 2. Check if the specific drug has already been prescribed to the patient today == Jabes, Alaxan, 300, 9.19.2025 ==  Jabes, Alaxan, 200, 9.19.2025 == Patient = Patient, Drugname = Drugname, Dosage != Dosage, Date = DateNow (Return error: Existing drug for today) 
            if (bll.Exists(patient, drugName, null, DateTime.Now.Date, id, true))
            {
                return Json(new { success = false, message = MessageUtil.ExistingDrug, errorType = "uniqueDrug" });
            }

            // If both checks pass, validation succeeds
            return Json(new { success = true });
        }
    }
}