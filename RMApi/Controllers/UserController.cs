﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RMApi.Data;
using RMApi.Models;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserData _userData;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context,
                              UserManager<IdentityUser> userManager,
                              IUserData userData,
                              ILogger<UserController> logger)
        {
            _context = context;
            _userManager = userManager;
            _userData = userData;
            _logger = logger;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _userData.GetUserById(userId).First();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
            
            var users = _context.Users.ToList();
            var userRoles = (from ur in _context.UserRoles
                             join r in _context.Roles on ur.RoleId equals r.Id
                             select new { ur.UserId, ur.RoleId, r.Name });

            foreach (var user in users)
            {
                ApplicationUserModel userModel = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                };

                userModel.Roles = userRoles.Where(x => x.UserId == userModel.Id).ToDictionary(key => key.RoleId, value => value.Name);

                output.Add(userModel);

            }
            return output;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
            return roles;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pairing)
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = _userData.GetUserById(loggedInUserId).First();

            var user = await _userManager.FindByIdAsync(pairing.UserId);
            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}", loggedInUserId, user.Id, pairing.RoleName);

            await _userManager.AddToRoleAsync(user, pairing.RoleName);
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pairing)
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = _userData.GetUserById(loggedInUserId).First();

            var user = await _userManager.FindByIdAsync(pairing.UserId);
            _logger.LogInformation("Admin {Admin} remove user {User} from role {Role}", loggedInUserId, user.Id, pairing.RoleName);

            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);

        }
    }
}
