using BLL;
using EL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UL;

namespace AL.Areas.Admin.Controllers
{
    public class PatientController : Controller
    {
        //Instance of BLL.
        private readonly PatientBLL bll = new PatientBLL();

        //View Patient.
        public ActionResult ViewPatient(
            string patient = "",
            string drug = "",
            string dosage = "", // Changed from decimal? to string for search to avoid exact value (LIKE).
            DateTime? date = null,
            int page = 1,
            int pageSize = 7)
        {
            List<PatientEntity> patients = new List<PatientEntity>();
            string errorMessage;
            string errorField;

            try
            {
                //Trimmer(first, middle (1 space), last).
                if (patient != null) patient = Validations.CleanSpaces(patient);
                if (drug != null) drug = Validations.CleanSpaces(drug);
                if (dosage != null) dosage = Validations.CleanSpaces(dosage);

                patients = bll.GetPatients();

                // Filtering logic.
                if (!string.IsNullOrEmpty(patient))
                    patients = patients
                        .Where(p => p.Patient != null && p.Patient.IndexOf(patient, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                if (!string.IsNullOrEmpty(drug))
                    patients = patients
                        .Where(p => p.DrugName != null && p.DrugName.IndexOf(drug, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                if (!string.IsNullOrEmpty(dosage))
                    patients = patients.Where(p => p.Dosage.ToString().Contains(dosage)).ToList();
                // Updated filter logic.
                if (date.HasValue)
                    patients = patients.Where(p => p.ModifiedDate.Date == date.Value.Date).ToList();
            }
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                TempData["Message"] = errorMessage;
                TempData["AlertType"] = "danger";
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

        //Search Function.
        [HttpGet]
        public JsonResult SearchPatients(
            //Optional assignment of empty value.
            string patient = "",
            string drug = "",
            string dosage = "", 
            DateTime? date = null,
            int page = 1,
            int pageSize = 7)
        {
            string errorMessage = null;
            try
            {
                //Trimmer(first, middle (1 space), last).
                if (patient != null) patient = Validations.CleanSpaces(patient);
                if (drug != null) drug = Validations.CleanSpaces(drug);
                if (dosage != null) dosage = dosage.Trim();

                var patients = bll.GetPatients();

                if (!string.IsNullOrEmpty(patient))
                    patients = patients
                        .Where(p => p.Patient != null && p.Patient.IndexOf(patient, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                if (!string.IsNullOrEmpty(drug))
                    patients = patients
                        .Where(p => p.DrugName != null && p.DrugName.IndexOf(drug, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                if (!string.IsNullOrEmpty(dosage))
                    patients = patients.Where(p => p.Dosage.ToString().Contains(dosage)).ToList();

                // Updated filter logic.
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

        //Add Patient (GET).
        [HttpGet]
        public ActionResult AddPatient()
        {
            return View(new PatientEntity());
        }

        //Add Patient (POST).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPatient(PatientEntity model)
        {
            string errorMessage;
            string errorField;

            //If invalid, just return.
            if (!ModelState.IsValid)
            {
                return View("AddPatient", model);
            }

            //BLL (Server side validation - BLL ?- DAL).
            try
            {
                bll.AddPatient(model);
                //Valid Data == Success.
                TempData["Message"] = MessageUtil.AddSuccess;
                TempData["AlertType"] = "success";
                return RedirectToAction("ViewPatient");
            }
            //Catch exceptions, for errors.
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                ModelState.AddModelError(errorField, errorMessage);
                TempData["Message"] = errorMessage;
                TempData["AlertType"] = "danger";
                return View("AddPatient", model);
            }
        }

        //Update (GET)
        [HttpGet]
        public ActionResult UpdatePatient(int id) //Fetching ID
        {
            string errorMessage;
            string errorField;

            try
            {
                var patient = bll.GetPatientById(id);
                if (patient == null)
                {
                    TempData["Message"] = MessageUtil.RecordNotFound;
                    TempData["AlertType"] = "danger"; 
                    return RedirectToAction("ViewPatient");
                }
                return View(patient);
            }
            //Catch exceptions, for errors.
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                TempData["Message"] = errorMessage;
                TempData["AlertType"] = "danger"; 
                return RedirectToAction("ViewPatient");
            }
        }

        //Update (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePatient(PatientEntity model)
        {
            string errorMessage;
            string errorField;

            //If invalid, just return.
            if (!ModelState.IsValid)
            {
                return View("UpdatePatient", model);
            }

            //BLL (Server side validation - BLL ?- DAL).
            try
            {
                bll.UpdatePatient(model);
                TempData["Message"] = MessageUtil.UpdateSuccess;
                TempData["AlertType"] = "success";
                return RedirectToAction("ViewPatient");
            }
            //Catch operation if no changes were made.
            catch (InvalidOperationException ex) when (ex.Message == MessageUtil.NoChange)
            {
                TempData["Message"] = ex.Message;
                TempData["AlertType"] = "info";
                return RedirectToAction("ViewPatient");
            }

            //Catch exceptions, for errors.
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                ModelState.AddModelError(errorField, errorMessage);
                TempData["Message"] = errorMessage;
                TempData["AlertType"] = "danger"; 
                return View("UpdatePatient", model);
            }
        }


        //Delete (POST).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePatient(int id)
        {
            string errorMessage;
            string errorField;

            try // Fetching ID
            {
                bll.DeletePatient(id);
                TempData["Message"] = MessageUtil.DeleteSuccess;
                TempData["AlertType"] = "success";
                return RedirectToAction("ViewPatient");
            }

            //Catch exceptions, for errors. (Rare occurence of error in delete, but for assurance.)
            catch (Exception ex)
            {
                Validations.Handle(ex, out errorMessage, out errorField);
                TempData["Message"] = errorMessage;
                TempData["AlertType"] = "danger"; 
                return RedirectToAction("ViewPatient");
            }
        }

        // Controller: PatientController.cs - AJAX Validation Method
        [HttpPost]
        public JsonResult ValidatePatient(string patient, string drugName, string dosage, int? id)
        {
            // Convention in parsing based on specific country format (e.g., en-US).
            CultureInfo culture = new CultureInfo("en-US");

            // Character and format validations.
            if (!decimal.TryParse(dosage, NumberStyles.Any, culture, out decimal parsedDosage))
            {
                return Json(new { success = false, message = "Dosage must be a valid positive number.", errorType = "invalidDosage" });
            }

            // Ensure dosage is positive.
            if (parsedDosage <= 0)
            {
                return Json(new { success = false, message = "Dosage must be a positive number.", errorType = "invalidDosage" });
            }

            // Check for a full row existence (exact duplicate).
            if (bll.Exists(patient, drugName, parsedDosage, DateTime.Now.Date, id))
            {
                return Json(new { success = false, message = MessageUtil.ExistingRecord, errorType = "alreadyExist" });
            }

            // Check if the specific drug has already been prescribed to the patient today.
            if (bll.Exists(patient, drugName, null, DateTime.Now.Date, id, true))
            {
                return Json(new { success = false, message = MessageUtil.ExistingDrug, errorType = "uniqueDrug" });
            }

            // If both checks pass, validation succeeds.
            return Json(new { success = true });
        }
    }
}