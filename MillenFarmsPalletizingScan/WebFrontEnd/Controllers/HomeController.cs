using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private ProductionService service = new ProductionService();

        public ActionResult Index()
        {
            return View(service.GetAllPallets());
        }

        public ActionResult Create()
        {
            long palletID = service.CreatePallet();
            return RedirectToAction("Scan", new { id = palletID });
        }

        public ActionResult Scan(long? id)
        {
            if (id.HasValue)
            {
                Pallet pallet = new Pallet
                {
                    PalletID = id.Value
                };
                ViewBag.ID = id.Value;
                return View(service.GetCases(id.Value));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Scan(long id, string caseNumber)
        {
            ViewBag.ID = id;
            ModelState.Clear();
            if (string.IsNullOrEmpty(caseNumber) || string.IsNullOrWhiteSpace(caseNumber))
            {
                ViewBag.Msg = "Serial Number cannot be left blank";
                return View(service.GetCases(id));
            }
            if(caseNumber.Length != 12)
            {
                ViewBag.Msg = "Serial number is not a valid length";
                return View(service.GetCases(id));
            }
            if (!service.ValidateCaseNumber(caseNumber))
            {
                ViewBag.Msg = "Invalid serial number";
                return View(service.GetCases(id));
            }
            if (service.UpdateCaseStatus(id, caseNumber))
            {
                ViewBag.SuccMsg = "Success!";
                return View(service.GetCases(id));
            }
            else
            {
                ViewBag.Msg = "There was a problem scanning that case, please try again.";
                return View(service.GetCases(id));
            }
        }

        public ActionResult Complete(long id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult Complete(long id, string btnValue)
        {
            if (service.MarkComplete(id))
                return RedirectToAction("Index", service.GetAllPallets());
            else
                return RedirectToAction("Scan", new { id });
        }

        public ActionResult SendToFreezer(long id)
        {
            ViewBag.ID = id;
            ViewBag.Freezers = service.GetFreezers();

            return View();
        }

        [HttpPost]
        public ActionResult SendToFreezer(long id, int freezerID)
        {
            ViewBag.ID = id;
            ViewBag.Freezers = service.GetFreezers();

            service.PutInFreezer(id, freezerID);

            if (service.MarkComplete(id))
                return RedirectToAction("Index", service.GetAllPallets());
            else
                return RedirectToAction("Scan", new { id });
        }

        public ActionResult Delete(long id)
        {
            if (service.Delete(id))
                return View("DeleteSuccess");

            return View("DeleteError");
        }
    }
}