using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ProductionService
    {
        private ProductionRepo repo;

        public ProductionService()
        {
            repo = new ProductionRepo();
        }

        /// <summary>
        /// Gets a list of all pallets from the database
        /// </summary>
        /// <returns>A list with all the pallets info</returns>
        public List<Pallet> GetAllPallets()
        {
            return repo.GetAllPallets();
        }

        /// <summary>
        /// Creates a new pallet and returns the ID to be used right away
        /// </summary>
        /// <returns></returns>
        public long CreatePallet()
        {
            return repo.CreatePallet();
        }

        /// <summary>
        /// Gets a list of all cases belonging to a pallet
        /// </summary>
        /// <param name="palletID">ID of the pallet the cases belong to</param>
        /// <returns></returns>
        public List<Case> GetCases(long palletID)
        {
            return repo.GetCases(palletID);
        }

        /// <summary>
        /// Checks the database to see if the case number exists in the table or not
        /// </summary>
        /// <param name="caseNumber"></param>
        /// <returns></returns>
        public bool ValidateCaseNumber(string caseNumber)
        {
            return repo.ValidateCaseNumber(caseNumber);
        }

        /// <summary>
        /// Updates the case's status
        /// </summary>
        /// <param name="palletID">ID of the pallet the case belonged too</param>
        /// <param name="caseNumber">Case number being updated</param>
        /// <returns></returns>
        public bool UpdateCaseStatus(long palletID, string caseNumber)
        {
            return repo.UpdateCaseStatus(palletID, caseNumber);
        }

        /// <summary>
        /// Marks a pallet as complete and sends the info to the correct tables
        /// </summary>
        /// <param name="palletID">ID of the pallet to be completed</param>
        /// <returns></returns>
        public bool MarkComplete(long palletID)
        {
            return repo.MarkComplete(palletID);
        }

        /// <summary>
        /// Deletes a pallet if no cases are scanned onto it yet
        /// </summary>
        /// <param name="id">ID of the pallet to be deleted</param>
        /// <returns></returns>
        public bool Delete(long id)
        {
            return repo.Delete(id);
        }

        public List<Freezer> GetFreezers()
        {
            return repo.GetFreezers();
        }

        public bool PutInFreezer(long palletID, int freezerID)
        {
            return repo.PutInFreezer(palletID, freezerID);
        }
    }
}
