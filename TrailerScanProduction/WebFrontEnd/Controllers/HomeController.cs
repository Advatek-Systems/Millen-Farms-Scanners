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
        private readonly PalletShipmentService service = new PalletShipmentService();
        // GET: Home
        public ActionResult Index()
        {
            return View(service.GetAllShipments());
        }

        /// <summary>
        /// The create function for creating new shipments
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Customers = service.GetAllCustomers();
            return View(new Shipment());
        }

        /// <summary>
        /// Handles the POST of the create shipment function
        /// </summary>
        /// <param name="shipment">The shipment being created currently</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int customerID, Shipment shipment)
        {
            shipment.StartedAt = DateTime.Now;
            shipment.customerID = customerID;
            if (service.CreateShipment(shipment) == true)
                return RedirectToAction("Scan", "Home", new { id = shipment.ShipmentID });
            return View("Create", shipment);
        }

        /// <summary>
        /// Delete function to delete shipments incase of user input error
        /// </summary>
        /// <param name="id">ID of the shipment to be deleted</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            if (service.DeleteShipment(id))
                return View("DeleteSuccess");
            return View("DeleteError");
        }

        /// <summary>
        /// Scan function for when user is scanning pallets into a shipment
        /// </summary>
        /// <param name="id">ID of the shipment being scanned into</param>
        /// <returns></returns>
        public ActionResult Scan(int? id)
        {
            ViewBag.ID = id.Value;
            ViewBag.Palletized = null;
            return View(service.GetAllPallets(id.Value));
        }

        /// <summary>
        /// Handles the POST of the scan function to add pallets to a shipment
        /// </summary>
        /// <param name="id">ID of the shipment being scanned into</param>
        /// <param name="palletNo">Pallet Number being scanned into the shipment</param>
        /// <returns></returns>10.1.5.127:45455
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

            foreach(Pallet p in pallets)
            {
                if(p.PalletNo == pallet.PalletNo)
                {
                    ViewBag.Msg = "You already scanned this pallet/case";
                    ModelState.Clear();
                    return View(service.GetAllPallets(id));
                }
            }

            if (service.IsPallet(palletNo) == 1)
                pallet.IsPalletized = "Y";
            else
                pallet.IsPalletized = "N";

            if(pallet.IsPalletized.ToLower() == "n" && service.IsCasePalletized(pallet) == 0)
            {
                ViewBag.Msg = "Invalid Case";
                ModelState.Clear();
                return View(service.GetAllPallets(id));
            }

            if (!service.CreatePallet(pallet))
                ViewBag.Msg = pallet.Errors[0].Description;

            ModelState.Clear();
            return View(service.GetAllPallets(id));
        }

        /// <summary>
        /// Marks the shipment as complete
        /// </summary>
        /// <param name="id">ID of the shipment to mark as complete</param>
        /// <returns></returns>
        public ActionResult Complete(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        /// <summary>
        /// Handles the POST for the complete shipment function
        /// </summary>
        /// <param name="id">ID of the shipment to complete</param>
        /// <param name="btnValue">extra value to make it different from the other function</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Complete(int id, string btnValue)
        {
            if (service.CompleteShipment(id))
                return View("CompleteSuccess");
            return View("CompleteFailed");
        }

        /// <summary>
        /// Function to delete a pallet incase of user input error
        /// </summary>
        /// <param name="id">ID of the pallet to delete from the shipment</param>
        /// <param name="shipmentID">ID of the shipment so the user stays on the same page</param>
        /// <returns></returns>
        public ActionResult DeletePallet(int? id, int? shipmentID)
        {
            ViewBag.ID = shipmentID.Value;
            service.DeletePallet(id.Value);
            return RedirectToAction("Scan", "Home", new { id = shipmentID.Value }); //View("Scan", service.GetAllPallets(shipmentID.Value));
        }

        /// <summary>
        /// makes the item that was just scanned a case instead of a pallet
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shipmentID"></param>
        /// <returns></returns>
        public ActionResult MakeCase(int? id, int? shipmentID)
        {
            ViewBag.ID = shipmentID.Value;
            service.MakeCase(id.Value);
            return RedirectToAction("Scan", "Home", new { id = shipmentID.Value });
        }

        /// <summary>
        /// makes the item that was just scanned a pallet instead of a case
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shipmentID"></param>
        /// <returns></returns>
        public ActionResult MakePallet(int? id, int? shipmentID)
        {
            ViewBag.ID = shipmentID.Value;
            service.MakePallet(id.Value);
            return RedirectToAction("Scan", "Home", new { id = shipmentID.Value });
        }
    }
}