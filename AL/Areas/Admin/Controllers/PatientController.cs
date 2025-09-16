// C:\Users\H2 SOFTWARE\Desktop\PatientRecords\AL\Areas\Admin\Controllers\PatientController.cs
using BLL;
using EL;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using UL;

namespace AL.Areas.Admin.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientBLL bll = new PatientBLL();

        // LIST VIEW
        public ActionResult ViewPatient(string patient, string drug, int? dosage, DateTime? modifieddate, int page = 1, int pageSize = 10)
        {
            var drugs = bll.GetPatient().AsQueryable();

            if (!string.IsNullOrWhiteSpace(patient))
                drugs = drugs.Where(d => d.Patient.Contains(patient));

            if (!string.IsNullOrWhiteSpace(drug))
                drugs = drugs.Where(d => d.DrugName.Contains(drug));

            if (dosage.HasValue)
                drugs = drugs.Where(d => d.Dosage == dosage.Value);

            if (modifieddate.HasValue)
                drugs = drugs.Where(d => d.ModifiedDate.Date == modifieddate.Value.Date);

            var pagedDrugs = drugs
                .OrderBy(d => d.ID)
                .ToPagedList(page, pageSize);

            ViewBag.PatientName = patient;
            ViewBag.DrugName = drug;
            ViewBag.Dose = dosage;
            ViewBag.DatePrescribed = modifieddate;

            return View(pagedDrugs);
        }

        // ADD DRUG (GET)
        [HttpGet]
        public ActionResult AddPatient()
        {
            ViewBag.formId = "addForm";
            ViewBag.modalTitle = "Confirm Save";
            ViewBag.modalMessage = "Do you want to save this drug?";
            ViewBag.confirmText = "Save";

            return View(new PatientEntity());
        }

        // ADD DRUG (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPatient(PatientEntity drug)
        {
            string message;
            string errorField;

            bool result = bll.AddPatient(drug, out message, out errorField);

            if (result)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("ViewPatient");
            }
            else
            {
                TempData["ErrorMessage"] = message;
                TempData["ErrorField"] = errorField;

                if (message.Contains(MessageUtil.Exist))
                {
                    // reset model if duplicate
                    ModelState.Clear();
                    return View(new PatientEntity());
                }
                return View(drug); // re-display with input
            }
        }

        // UPDATE DRUG (GET)
        [HttpGet]
        public ActionResult UpdatePatient(int id)
        {
            PatientEntity model = bll.GetPatientById(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("ViewPatient");
            }

            ViewBag.formId = "updateForm";
            ViewBag.modalTitle = "Confirm Update";
            ViewBag.modalMessage = "Do you want to update this drug?";
            ViewBag.confirmText = "Update";

            return View(model);
        }

        // UPDATE DRUG (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePatient(PatientEntity model)
        {
            string message;
            string errorField;

            bool result = bll.UpdatePatient(model, out message, out errorField);

            if (result)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("ViewPatient");
            }
            else
            {
                TempData["ErrorMessage"] = message;
                TempData["ErrorField"] = errorField;

                return View(model); // re-display with errors
            }
        }

        // DELETE DRUG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePatient(int id)
        {
            string message;
            string errorField;
            bool result = bll.DeletePatient(id, out message, out errorField);

            if (result)
            {
                return Json(new { success = true, message });
            }
            else
            {
                Response.StatusCode = 400;
                return Json(new { success = false, message });
            }
        }
    }
}
