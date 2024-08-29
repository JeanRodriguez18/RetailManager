using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> GetInventory()
        {
            InventoryData data = new InventoryData();

            return data.GetInventory();
        }
        
        [Authorize(Roles = "Manager,Admin")]
        public void Post(InventoryModel model)
        {
            InventoryData data = new InventoryData();

            data.SaveInventoryRecord(model);
        }
    }
}
