using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PalletService
    {
        private PalletRepo repo;

        public PalletService()
        {
            repo = new PalletRepo();
        }

        public bool ValidatePalletNumber(string palletNumber)
        {
            return repo.ValidatePalletNumber(palletNumber);
        }

        public string UpdatePalletStatus(string palletNumber)
        {
            return repo.UpdatePalletStatus(palletNumber);
        }

        public List<Product> GetProductList()
        {
            return repo.GetProductList();
        }

        public List<BoxSize> GetBoxSizeList()
        {
            return repo.GetBoxSizeList();
        }

        public void InsertProductionRecord(string lotNo, int prodID, int boxSizeID, int qty, string freshOrFrozen)
        {
            repo.InsertProductionRecord(lotNo, prodID, boxSizeID, qty, freshOrFrozen);
        }

        public bool DeleteCase(string serialNo)
        {
            return repo.DeleteCase(serialNo);
        }
    }
}
