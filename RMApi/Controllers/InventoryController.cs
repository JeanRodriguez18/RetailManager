using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryModel> GetInventory()
        {
            InventoryData data = new InventoryData(_config);

            return data.GetInventory();
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public void Post(InventoryModel model)
        {
            InventoryData data = new InventoryData(_config);

            data.SaveInventoryRecord(model);
        }
    }
}
