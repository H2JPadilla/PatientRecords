using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using EL;
using UL;
using System.Globalization;
using System.Data.SqlClient;

namespace AL.Areas.Admin.Controllers
{
    public class PatientController : Controller
    {
        // Create instance of BLL
        private readonly PatientBLL bll = new PatientBLL();

        // Standard View: Used for the initial page load with filtering and pagination
        public ActionResult ViewPatient(
            string patient = "",
            string drug = "",
            decimal? dosage = null,
            DateTime? date = null,
            int page = 1,
            int pageSize = 10)
        {
            List<PatientEntity> patients = new List<PatientEntity>();
            string errorMessage;
            string errorField;

            try
            {
                if (patient != null) patient = patient.Trim();
                if (drug != null) drug = drug.Trim();

                patients = bll.GetPatients();

                // Filtering logic is applied here
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
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                TempData["Message"] = errorMessage;
            }

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

        // AJAX Action: Returns JSON for live search
        [HttpGet]
        public JsonResult SearchPatients(
            string patient = "",
            string drug = "",
            decimal? dosage = null,
            DateTime? date = null,
            int page = 1,
            int pageSize = 10)
        {
            string errorMessage = null;

            try
            {
                if (patient != null) patient = patient.Trim();
                if (drug != null) drug = drug.Trim();

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

                var formattedPatients = pagedPatients.Select(p => new
                {
                    p.ID,
                    p.Patient,
                    p.DrugName,
                    p.Dosage,
                    ModifiedDate = p.ModifiedDate.ToString("MM-dd-yyyy")
                }).ToList();

                return Json(new
                {
                    success = true,
                    patients = formattedPatients,
                    totalPages = totalPages,
                    page = page,
                    pageSize = pageSize
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorField;
                Validations.Handle(ex, out errorMessage, out errorField);
                return Json(new { success = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
            }
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
            string errorMessage;
            string errorField;

            if (!ModelState.IsValid)
            {
                return View("AddPatient", model);
            }

            try
            {
                bll.AddPatient(model);
                TempData["Message"] = MessageUtil.AddSuccess;
                return RedirectToAction("ViewPatient");
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                ModelState.AddModelError(errorField, errorMessage);
                return View("AddPatient", model);
            }
        }

        // GET: Update Patient
        [HttpGet]
        public ActionResult UpdatePatient(int id)
        {
            string errorMessage;
            string errorField;

            try
            {
                var patient = bll.GetPatientById(id);
                if (patient == null)
                {
                    TempData["Message"] = MessageUtil.RecordNotFound;
                    return RedirectToAction("ViewPatient");
                }
                return View(patient);
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                TempData["Message"] = errorMessage;
                return RedirectToAction("ViewPatient");
            }
        }

        // POST: Update Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePatient(PatientEntity model)
        {
            string errorMessage;
            string errorField;

            if (!ModelState.IsValid)
            {
                return View("UpdatePatient", model);
            }

            try
            {
                bll.UpdatePatient(model);
                TempData["Message"] = MessageUtil.UpdateSuccess;
                return RedirectToAction("ViewPatient");
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                ModelState.AddModelError(errorField, errorMessage);
                return View("UpdatePatient", model);
            }
        }

        // POST: Delete Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePatient(int id)
        {
            string errorMessage;
            string errorField;

            try
            {
                bll.DeletePatient(id);
                TempData["Message"] = MessageUtil.DeleteSuccess;
                return RedirectToAction("ViewPatient");
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                TempData["Message"] = errorMessage;
                return RedirectToAction("ViewPatient");
            }
        }

        // Controller: PatientController.cs
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

            // 1. Check for a full row existence (exact duplicate)
            if (bll.Exists(patient, drugName, parsedDosage, DateTime.Now.Date, id))
            {
                return Json(new { success = false, message = MessageUtil.ExistingRecord, errorType = "alreadyExist" });
            }

            // 2. Check if the specific drug has already been prescribed to the patient today
            if (bll.Exists(patient, drugName, null, DateTime.Now.Date, id, true))
            {
                return Json(new { success = false, message = MessageUtil.ExistingDrug, errorType = "uniqueDrug" });
            }

            // If both checks pass, validation succeeds
            return Json(new { success = true });
        }
    }
}