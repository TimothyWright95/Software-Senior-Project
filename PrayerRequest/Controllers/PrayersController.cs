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
    public class PrayersController : Controller
    {
        private IDBRepository _repository;
        private UserManager<PrayerRequestUser> _UserManager;
        public PrayersController(UserManager<PrayerRequestUser> UserManager, IDBRepository repo)
        {
            _UserManager = UserManager;
            _repository = repo;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Prayer prayer)
        {
             if (!ModelState.IsValid)
            {
                return View("Error");
            }
            List<Prayer> temp = _repository.GetAllUserPrayer(_UserManager.GetUserId(User));
            int count = temp.Count;
            if (count >= 50)
            {
                return RedirectToAction("Error");
            }
            else
            {
                prayer.UserID = _UserManager.GetUserId(User);
                _repository.CreatePrayerRequest(prayer);
                return RedirectToAction("Prayers", "User");
            }
        }
        public IActionResult Subscribe()
        {
            ViewBag.textfilter = "";
            ViewBag.catagorie = 0;
            ViewBag.state = "";
            List<Prayer> model = _repository.GetAllPrayerRequests("", 0, 0);
            List<int> usersubprayers = _repository.GetUserSubscribedPrayers(_UserManager.GetUserId(User));
            List<int> groups = _repository.GetUserSubscribedGroups(_UserManager.GetUserId(User));
            List<string> groupusers = new List<string>();
            foreach (int group in groups)
            {
                groupusers = groupusers.Union(_repository.GetGroupSubscribers(group)).ToList();
            }

            List<Prayer> Usergroupprayers = model.Where(o => groupusers.Contains(o.UserID)).ToList();
            model = model.Where(o => !Usergroupprayers.Contains(o)).ToList();
            Usergroupprayers = Usergroupprayers.Concat(model).ToList();
            foreach (Prayer prayer in Usergroupprayers)
            {
                if (usersubprayers.Contains(prayer.ID))
                {
                    Usergroupprayers.Remove(prayer);
                }
                else
                {
                    if (prayer.PostAnonymous == true)
                        prayer.UserID = "Anonymous";
                    else
                        prayer.UserID = _UserManager.FindByIdAsync(prayer.UserID).GetAwaiter().GetResult().DisplayName;
                }
            }
            return View(Usergroupprayers);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Subscribe(string textfilter, PrayerRequest.Repository.Catagorie catagorie, PrayerRequest.Repository.States state)
        {
            if (textfilter == null) textfilter = "";
            List<Prayer> model = _repository.GetAllPrayerRequests(textfilter, catagorie, state);
            List<int> usersubprayers = _repository.GetUserSubscribedPrayers(_UserManager.GetUserId(User));
            List<int> groups = _repository.GetUserSubscribedGroups(_UserManager.GetUserId(User));
            List<string> groupusers = new List<string>();
            foreach (int group in groups)
            {
                groupusers = groupusers.Union(_repository.GetGroupSubscribers(group)).ToList();
            }

            List<Prayer> Usergroupprayers = model.Where(o => groupusers.Contains(o.UserID)).ToList();
            model = model.Where(o => !Usergroupprayers.Contains(o)).ToList();
            Usergroupprayers = Usergroupprayers.Concat(model).ToList();
            foreach (Prayer prayer in Usergroupprayers)
            {
                if (usersubprayers.Contains(prayer.ID))
                {
                    Usergroupprayers.Remove(prayer);
                }
                else
                {
                    if (prayer.PostAnonymous == true)
                        prayer.UserID = "Anonymous";
                    else
                        prayer.UserID = _UserManager.FindByIdAsync(prayer.UserID).GetAwaiter().GetResult().DisplayName;
                }
            }
            ViewBag.textfilter = textfilter;
            ViewBag.catagorie = (int)catagorie;
            ViewBag.state = (int)state;
            return View(Usergroupprayers);

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SubscribeToRequest(int ID)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }
            _repository.SubscribeToPrayer(ID, _UserManager.GetUserId(User));
            return RedirectToAction("SubscribedPrayers", "User");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ViewRequest(int ID)
        {
            Prayer model = _repository.GetPrayer(ID);
            if (model.PostAnonymous == true)
                model.UserID = "Anonymous";
            else
                model.UserID = _UserManager.FindByIdAsync(model.UserID).GetAwaiter().GetResult().DisplayName;
            return View(model);
        }
        public IActionResult Error()
        {
            return View();
        }



        public static string IndexNav => "Index";

        public static string CreateNav => "Create";

        public static string SubscribeNav => "Subscribe";
        

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, IndexNav);
        public static string CreateNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateNav);
        public static string SubscribeNavClass(ViewContext viewContext) => PageNavClass(viewContext, SubscribeNav);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.RouteData.Values["Action"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }

}