using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PrayerRequest.Models;
using Microsoft.IdentityModel;
using Microsoft.AspNetCore.Identity;
using PrayerRequest.Areas.Identity.Data;
using PrayerRequest.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;

namespace PrayerRequest.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private IDBRepository _repository;
        private UserManager<PrayerRequestUser> _UserManager;
        public string Username { get; set; }
        public UserController(UserManager<PrayerRequestUser> UserManager, IDBRepository repo)
        {
            _UserManager = UserManager;
            _repository = repo;
             
        }
        public IActionResult Index()
        {
            PrayerRequestUser currnetUser = _UserManager.GetUserAsync(User).GetAwaiter().GetResult();
            User_Model model = new User_Model();
            model.DisplayName = currnetUser.DisplayName;
            model.Email = currnetUser.Email;
            model.phonenumber = currnetUser.PhoneNumber;
            model.UserID = currnetUser.Id;
            model.UserName = currnetUser.UserName;
            return View(model);
        }
        [HttpGet]
        public IActionResult Prayers()
        {
            List<Prayer> model = _repository.GetAllUserPrayer(_UserManager.GetUserId(User));
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Prayers(int id)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }
            _repository.DeletePrayerRequest(id, _UserManager.GetUserId(User));
            return RedirectToAction("Prayers");
        }
        [HttpGet]
        public IActionResult Groups()
        {
            List<Group> model = _repository.GetAllUserOwnedGroups(_UserManager.GetUserId(User));
            if (_UserManager.IsInRoleAsync(_UserManager.GetUserAsync(User).GetAwaiter().GetResult(), "Admin").GetAwaiter().GetResult())
            {
                model = _repository.GetAllGroups("", 0);
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult SubscribedGroups()
        {
            List<Group> model = _repository.GetAllUserSubGroups(_UserManager.GetUserId(User));
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SubscribedGroups(int id)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }
            _repository.UnsubscribeGroup(id, _UserManager.GetUserId(User));
            return RedirectToAction("SubscribedGroups");
        }
        [HttpGet]
        public IActionResult SubscribedPrayers()
        {
            List<Prayer> model = _repository.GetAllUserSubPrayer(_UserManager.GetUserId(User));
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SubscribedPrayers(int id)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }
            _repository.UnsubscribePrayerRequest(id, _UserManager.GetUserId(User));
            return RedirectToAction("SubscribedPrayers");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ViewPrayer(int id)
        {
            Prayer model = _repository.GetPrayer(id);
            ViewBag.ID = model.UserID;
            if (model.PostAnonymous == true)
                model.UserID = "Anonymous";
            else
                model.UserID = _UserManager.FindByIdAsync(model.UserID).GetAwaiter().GetResult().DisplayName;
            return View(model);
        }
        public static string IndexNav => "Index";

        public static string PrayerNav => "Prayers";

        public static string PrayerSubNav => "SubscribedPrayers";

        public static string GroupSubNav => "SubscribedGroups";
        public static string GroupNav => "Groups";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, IndexNav);
        public static string PrayerNavClass(ViewContext viewContext) => PageNavClass(viewContext, PrayerNav);
        public static string PrayerSubNavClass(ViewContext viewContext) => PageNavClass(viewContext, PrayerSubNav);
        public static string GroupSubNavClass(ViewContext viewContext) => PageNavClass(viewContext, GroupSubNav);
        public static string GroupNavClass(ViewContext viewContext) => PageNavClass(viewContext, GroupNav);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.RouteData.Values["Action"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}