using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData
    {

        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: Make this SOLID/DRY/BETTER
            // Start filling in the models we will save to the database
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();
            decimal taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };

                //Get the information about this product
                var productInfo = products.GetProductById(detail.ProductId);
                
                if (productInfo == null)
                {
                    throw new Exception($"The product id of {item.ProductId} could not be found in the database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);
                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                details.Add(detail);
            }

            // Create the sale model
            SaleDBModel sale = new SaleDBModel()
            {
                SubTotal = details.Sum(x=> x.PurchasePrice),
                Tax = details.Sum(x=> x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;
            
            

            using(SqlDataAccess sql = new SqlDataAccess())
            {
                try
                {
                    sql.StartTransaction("RMData");

                    //Save the sale model
                    sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                    // Get the ID from the sale model
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { CashierId = sale.CashierId, SaleDate = sale.SaleDate }).FirstOrDefault();

                    // Finish filling in the sale detail models
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;
                        // Save the sale details models
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }


                    sql.CommitTransaction();
                }
                catch (Exception ex)
                {
                    sql.RollBackTransaction();
                    throw;
                }
            }

            
           
        }

    }
}
