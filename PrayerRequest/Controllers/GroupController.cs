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
    public class GroupController : Controller
    {
        private IDBRepository _repository;
        private UserManager<PrayerRequestUser> _UserManager;
        public GroupController(UserManager<PrayerRequestUser> UserManager, IDBRepository repo)
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
        public IActionResult Create(Group group)
        {
            if (group.GroupPassword == null)
            {
                ModelState.Remove("GroupPassword");
            }
            if (!ModelState.IsValid)
            {
                return View("Error");
            }
            
            group.OwnerID = _UserManager.GetUserId(User);
            _repository.CreateGroup(group);
            return RedirectToAction("Groups", "User");

        }
        [HttpGet]
        public IActionResult Subscribe()
        {
            ViewBag.textfilter = "";
            ViewBag.catagorie = 0;
            ViewBag.state = "";
            List<Group> model = _repository.GetAllGroups("", 0);
            List<int> usergroups = _repository.GetUserSubscribedGroups(_UserManager.GetUserId(User));
            for(int ii = model.Count - 1; ii >= 0; ii--)
            {
                if(usergroups.Contains(model[ii].ID))
                {
                    model.Remove(model[ii]);
                }
                else
                    model[ii].OwnerID = _UserManager.FindByIdAsync(model[ii].OwnerID).GetAwaiter().GetResult().DisplayName;

            }
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Subscribe(string textfilter, PrayerRequest.Repository.States state)
        {
            if (textfilter == null) textfilter = "";
            List<Group> model = _repository.GetAllGroups(textfilter, state);
            List<int> usergroups = _repository.GetUserSubscribedGroups(_UserManager.GetUserId(User));
            foreach (Group group in model)
            {
                if (usergroups.Contains(group.ID))
                {
                    model.Remove(group);
                }
                else
                    group.OwnerID = _UserManager.FindByIdAsync(group.OwnerID).GetAwaiter().GetResult().DisplayName;

            }
            ViewBag.textfilter = textfilter;
            ViewBag.state = (int)state;
            return View(model);

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SubscribeToGroup(int ID)
        {
            Group temp = _repository.GetGroup(ID);
            if (temp.GroupPassword != "" && temp.GroupPassword != null)
            {
                //password exists redirect to password check page
                temp.GroupPassword = "";
                return View("GroupPasswordCheck", temp);
            }
            else
            {
                _repository.SubscribeToGroup(ID, _UserManager.GetUserId(User));
                return RedirectToAction("SubscribedGroups", "User");
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ViewGroup(int ID)
        {
            Group model = _repository.GetGroup(ID);
            return View(model);
        }
        [HttpGet]
        public IActionResult GroupPasswordCheck()
        {
            return RedirectToAction("Index");
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult GroupPasswordCheck(Group group)
        {

            Group temp = _repository.GetGroup(group.ID);
            if (temp.GroupPassword != group.GroupPassword)
            {
                //password exists redirect to password check page
                ViewBag.ERROR = "Incorect Password Entered Please Try Again.";
                temp.GroupPassword = "";
                return View("GroupPasswordCheck", temp);
            }
            else
            {
                _repository.SubscribeToGroup(group.ID, _UserManager.GetUserId(User));
                return RedirectToAction("SubscribedGroups", "User");
            }
        }


        public static string IndexNav => "Index";

        public static string GroupCreateNav => "Create";

        public static string GroupSubNav => "Subscribe";
        

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, IndexNav);
        public static string CreateNavClass(ViewContext viewContext) => PageNavClass(viewContext, GroupCreateNav);
        public static string SubscribeNavClass(ViewContext viewContext) => PageNavClass(viewContext, GroupSubNav);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.RouteData.Values["Action"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}