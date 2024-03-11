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
        private PalletShipmentService service = new PalletShipmentService();

        public ActionResult Index()
        {
            return View(service.GetAllShipments());
        }

        public ActionResult Create()
        {
            return View(new Shipment());
        }

        [HttpPost]
        public ActionResult Create(Shipment shipment, string pc)
        {
            shipment.StartedAt = DateTime.Now;
            if(pc.ToLower() == "ecwb")
                shipment.statecode = 1;
            else if(pc.ToLower() == "eci")
                shipment.statecode = 4;

            if (service.CreateShipment(shipment) == true)
                return RedirectToAction("Scan", "Home", new { id = shipment.ShipmentID });

            return View("Create", shipment);
        }

        public ActionResult Delete(int id)
        {
            if (service.DeleteShipment(id))
                return View("DeleteSuccess");

            return View("DeleteError");
        }

        public ActionResult Scan(int? id)
        {
            ViewBag.ID = id.Value;

            return View(service.GetAllPallets(id.Value));
        }

        [HttpPost]
        public ActionResult Scan(int id, string palletNo)
        {
            ViewBag.ID = id;

            Pallet pallet = new Pallet
            {
                PalletNo = palletNo,
                ShipmentID = id,
                ScannedAt = DateTime.Now
            };

            List<Pallet> pallets = service.GetAllPallets(id);

            foreach (Pallet p in pallets)
            {
                if (p.PalletNo == pallet.PalletNo)
                {
                    ViewBag.Msg = "You already scanned this pallet/case";
                    ModelState.Clear();
                    return View(service.GetAllPallets(id));
                }
            }

            if (!service.CreatePallet(pallet))
                ViewBag.Msg = pallet.Errors[0].Description;

            ModelState.Clear();
            return View(service.GetAllPallets(id));
        }

        public ActionResult Complete(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult Complete(int id, string btnValue)
        {
            if (service.CompleteShipment(id))
                return View("CompleteSuccess");

            return View("CompleteFailed");
        }

        public ActionResult DeletePallet(int? id, int? shipmentID)
        {
            ViewBag.ID = shipmentID.Value;
            service.DeletePallet(id.Value);
            return RedirectToAction("Scan", "Home", new { id = shipmentID.Value });
        }
    }
}