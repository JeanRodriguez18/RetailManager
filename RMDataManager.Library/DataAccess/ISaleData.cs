using RMDataManager.Library.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public interface ISaleData
    {
        List<SaleReportModel> GetSalesReport();
        void SaveSale(SaleModel saleInfo, string cashierId);
    }
}