using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PalletShipmentService
    {
        private PalletShipmentRepo repo;

        public PalletShipmentService()
        {
            repo = new PalletShipmentRepo();
        }

        /// <summary>
        /// creates a new shipment
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public bool CreateShipment(Shipment shipment)
        {
            if (ValidateShipment(shipment))
                return repo.CreateShipment(shipment);
            return false;
        }

        /// <summary>
        /// creates a new pallet
        /// </summary>
        /// <param name="pallet"></param>
        /// <returns></returns>
        public bool CreatePallet(Pallet pallet)
        {
            if (ValidatePallet(pallet))
                return repo.CreatePallet(pallet);
            return false;
        }

        /// <summary>
        /// marks a shipment as complete and sends the data to the according tables
        /// </summary>
        /// <param name="shipmentID"></param>
        /// <returns></returns>
        public bool CompleteShipment(int shipmentID)
        {
            return repo.CompleteShipment(shipmentID);
        }

        /// <summary>
        /// returns a list of all shipments with details
        /// </summary>
        /// <returns></returns>
        public List<Shipment> GetAllShipments()
        {
            return repo.GetAllShipments();
        }

        /// <summary>
        /// gets a list of all pallets that belong to the selected shipment
        /// </summary>
        /// <param name="shipmentID"></param>
        /// <returns></returns>
        public List<Pallet> GetAllPallets(int shipmentID)
        {
            return repo.GetAllPallets(shipmentID);
        }

        /// <summary>
        /// deletes a shipment if there are no pallets scanned into it
        /// </summary>
        /// <param name="shipmentID"></param>
        /// <returns></returns>
        public bool DeleteShipment(int shipmentID)
        {
            return repo.DeleteShipment(shipmentID);
        }

        /// <summary>
        /// deletes a pallet from a shipment
        /// </summary>
        /// <param name="palletID"></param>
        public void DeletePallet(int palletID)
        {
            repo.DeletePallet(palletID);
        }

        /// <summary>
        /// validates a shipment for errors
        /// </summary>
        /// <param name="shipment">object which holds the data</param>
        /// <returns></returns>
        private bool ValidateShipment(Shipment shipment)
        {
            // validate entity
            ValidationContext context = new ValidationContext(shipment);
            List<ValidationResult> results = new List<ValidationResult>();

            Validator.TryValidateObject(shipment,
                context, results, true);

            foreach (ValidationResult e in results)
            {
                shipment.AddError(new ValidationError(e.ErrorMessage));
            }

            return shipment.Errors.Count == 0;
        }

        /// <summary>
        /// validates a pallet for errors
        /// </summary>
        /// <param name="pallet">object which holds the data</param>
        /// <returns></returns>
        private bool ValidatePallet(Pallet pallet)
        {
            // validate entity
            ValidationContext context = new ValidationContext(pallet);
            List<ValidationResult> results = new List<ValidationResult>();

            Validator.TryValidateObject(pallet,
                context, results, true);

            foreach (ValidationResult e in results)
            {
                pallet.AddError(new ValidationError(e.ErrorMessage));
            }

            return pallet.Errors.Count == 0;
        }
    }
}
