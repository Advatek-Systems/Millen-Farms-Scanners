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
        private PalletService service = new PalletService();
        private List<string> freshOrFrozen = new List<string>()
        {
            "Fresh",
            "Frozen"
        };

        public ActionResult Index()
        {
            ViewBag.Msg = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(string palletNumber)
        {
            ViewBag.Msg = "";
            if (string.IsNullOrEmpty(palletNumber) || string.IsNullOrWhiteSpace(palletNumber))
            {
                ViewBag.Msg = "Pallet Number cannot be empty.";
                ModelState.Clear();
                return View();
            }
            if(palletNumber.Length != 12)
            {
                ViewBag.Msg = "Pallet Number must be 12 characters exactly.";
                ModelState.Clear();
                return View();
            }
            if(!service.ValidatePalletNumber(palletNumber))
            {
                ViewBag.Msg = "Invalid pallet number.";
                ModelState.Clear();
                return View();
            }
            string lotNumber = service.UpdatePalletStatus(palletNumber);
            ModelState.Clear();
            return RedirectToAction("ProductionInfo", new { lotNumber });
        }

        public ActionResult ProductionInfo(string lotNumber)
        {
            ViewBag.FreshOrFrozen = freshOrFrozen;
            ViewBag.Products = service.GetProductList();
            ViewBag.BoxSizes = service.GetBoxSizeList();
            ViewBag.LotNumber = lotNumber;

            return View();
        }

        [HttpPost]
        public ActionResult ProductionInfo(string lotNumber, string freshOrFrozen, int productID, int boxSizeID, int? quantity)
        {
            ViewBag.FreshOrFrozen = freshOrFrozen;
            ViewBag.Products = service.GetProductList();
            ViewBag.BoxSizes = service.GetBoxSizeList();
            ViewBag.LotNumber = lotNumber;

            if (string.IsNullOrEmpty(lotNumber) || string.IsNullOrWhiteSpace(lotNumber))
            {
                ViewBag.Msg = "Lot Number is required! Please go back and rescan the pallet label!";
                return View();
            }
            if (!quantity.HasValue)
            {
                ViewBag.Msg = "Quantity is required!";
                return View();
            }
            if (quantity.Value <= 0 || quantity.Value > 32)
            {
                ViewBag.Msg = "Quantity cannot be 0 or less than 0 and cannot be more than 32!";
                return View();
            }

            service.InsertProductionRecord(lotNumber, productID, boxSizeID, quantity.Value, freshOrFrozen);

            return RedirectToAction("Index");
        }

        public ActionResult DeleteCaseLabels()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteCaseLabels(string serialNo)
        {
            if (string.IsNullOrEmpty(serialNo) || string.IsNullOrWhiteSpace(serialNo))
            {
                ViewBag.Msg = "Serial Number is required!";
                ViewBag.Colour = "danger";
                return View();
            }

            service.DeleteCase(serialNo);
            ViewBag.Msg = $"Successfully deleted case #{serialNo}";
            ViewBag.Colour = "success";

            ModelState.Clear();

            return View();
        }
    }
}