using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using EL;
using UL;

namespace AL.Areas.Admin.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientBLL bll = new PatientBLL();

        // VIEW: Patients with search + pagination
        public ActionResult ViewPatient(
            string patient = "",
            string drug = "",
            int? dosage = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 5)
        {
            var patients = bll.GetPatients();

            // Filtering
            if (!string.IsNullOrEmpty(patient))
                patients = patients
                    .Where(p => p.Patient != null &&
                                p.Patient.IndexOf(patient, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            if (!string.IsNullOrEmpty(drug))
                patients = patients
                    .Where(p => p.DrugName != null &&
                                p.DrugName.IndexOf(drug, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            if (dosage.HasValue)
                patients = patients
                    .Where(p => p.Dosage == dosage.Value)
                    .ToList();

            if (fromDate.HasValue)
                patients = patients.Where(p => p.ModifiedDate.Date >= fromDate.Value.Date).ToList();

            if (toDate.HasValue)
                patients = patients.Where(p => p.ModifiedDate.Date <= toDate.Value.Date).ToList();

            // Pagination
            int totalRecords = patients.Count();
            var pagedPatients = patients
                .OrderByDescending(p => p.ModifiedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass filters back to view
            ViewBag.Patient = patient;
            ViewBag.Drug = drug;
            ViewBag.Dosage = dosage;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return View(pagedPatients);
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

            // Validation 1: Check for exact duplicate record
            if (bll.Exists(model.Patient, model.DrugName, model.Dosage, DateTime.Now))
            {
                ModelState.AddModelError("", "Data already exist.");
                return View("AddPatient", model);
            }

            // Validation 2: Check for unique medicine per day for the same patient
            if (bll.Exists(model.Patient, null, 0, DateTime.Now, null, true))
            {
                ModelState.AddModelError("", "One medicine per day is allowed.");
                return View("AddPatient", model);
            }

            bll.AddPatient(model);
            TempData["Message"] = "Patient added successfully!";
            return RedirectToAction("ViewPatient");
        }

        // GET: Update Patient
        [HttpGet]
        public ActionResult UpdatePatient(int id)
        {
            var patient = bll.GetPatientById(id);
            if (patient == null)
            {
                TempData["Message"] = "Record not found!";
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

            // Validation 1: Check for exact duplicate record (excluding current ID)
            if (bll.Exists(model.Patient, model.DrugName, model.Dosage, DateTime.Now, model.ID))
            {
                ModelState.AddModelError("", "Data already exist.");
                return View("UpdatePatient", model);
            }

            // Validation 2: Check for unique medicine per day (excluding current ID)
            if (bll.Exists(model.Patient, null, 0, DateTime.Now, model.ID, true))
            {
                ModelState.AddModelError("", "One medicine per day is allowed.");
                return View("UpdatePatient", model);
            }

            bll.UpdatePatient(model);
            TempData["Message"] = "Patient updated successfully!";
            return RedirectToAction("ViewPatient");
        }

        // POST: Delete Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePatient(int id)
        {
            bll.DeletePatient(id);
            TempData["Message"] = "Patient deleted successfully!";
            return RedirectToAction("ViewPatient");
        }
    }
}