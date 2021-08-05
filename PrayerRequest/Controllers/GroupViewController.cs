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
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PrayerRequest.Controllers
{
    [Authorize]
    public class GroupViewController : Controller
    {
        private IDBRepository _repository;
        private UserManager<PrayerRequestUser> _UserManager;
        private readonly IHostingEnvironment _environment;
        public GroupViewController(UserManager<PrayerRequestUser> UserManager, IDBRepository repo, IHostingEnvironment hosting)
        {
            _UserManager = UserManager;
            _repository = repo;
            _environment = hosting;

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index(int? ID)
        {
            IActionResult var = SetViewBag(ID);
            if (var != null)
            {
                return var;
            }
            string img_path = _repository.GetGroupImage((int)ID);
            ViewBag.IMG_PATH = img_path;
            Group model = _repository.GetGroup((int)ID);
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Bulletin(int? ID, int? todelete, Bulitin bulitin)
        {
            if(todelete != null)
            {
                _repository.DeleteBulitin((int)todelete, null, (int)ID);
            }
            if(bulitin.Bulitinmessage != null)
            {
                bulitin.DatePosted = System.DateTime.Now;
                _repository.AddBulitin(bulitin, (int)ID);
            }
            List<Bulitin> model = _repository.GetAllBullitin((int)ID);
            IActionResult var = SetViewBag(ID);
            if (var != null)
            {
                return var;
            }
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Chat(int? ID, int? todelete)
        {
            IActionResult var = SetViewBag(ID);
            if (var != null)
            {
                return var;
            }
            if(todelete != null)
            {
                _repository.DeletePost((int)todelete,null,(int)ID);
            }
            List<Post> posts = _repository.GetAllPosts((int)ID);
            foreach (Post post in posts)
            {
                
                post.DisplayName = _UserManager.FindByIdAsync(post.OwnerID).GetAwaiter().GetResult().DisplayName;

            }
            return View(posts);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Prayers(int? ID)
        {
            List<string> groupsubs = _repository.GetGroupSubscribers((int)ID);
            List<Prayer> model = _repository.GetAllPrayerRequests("", 0, 0);
            model = model.Where(o => groupsubs.Contains(o.UserID)).ToList();
            List<Prayer> usersublist = _repository.GetAllUserSubPrayer(_UserManager.GetUserId(User));
            List<int> usersublist_int = usersublist.Select(o => o.ID).ToList();
            model = model.Where(o => !usersublist_int.Contains(o.ID)).ToList();
            IActionResult var = SetViewBag(ID);
            if (var != null)
            {
                return var;
            }
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Manage(int? ID, int? uploadfile)
        {
            if(uploadfile != null)
            {
                ID = uploadfile;
                var newFileName = string.Empty;

                if (HttpContext.Request.Form.Files != null)
                {
                    var fileName = string.Empty;
                    string PathDB = string.Empty;

                    var files = HttpContext.Request.Form.Files;

                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            var FileExtension = Path.GetExtension(fileName);
                            newFileName = myUniqueFileName + FileExtension;
                            fileName = Path.Combine(_environment.WebRootPath, "GroupImages") + $@"\{newFileName}";
                            PathDB = "../GroupImages/" + newFileName;
                            _repository.SetGroupImage(PathDB, (int)uploadfile);

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                    }


                }
            }
            IActionResult var = SetViewBag(ID);
            if (var != null)
            {
                return var;
            }
            string img_path = _repository.GetGroupImage((int)ID);
            ViewBag.IMG_PATH = img_path;
            Group model = _repository.GetGroup((int)ID);

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ManageMembers(int? ID, string todemote, string toremove, string topromote)
        {
            //list users
            //list moderators
            if(todemote != null)
            {
                _repository.DemoteMod((int)ID, null, todemote);
            }
            else if(toremove != null)
            {
                _repository.RemoveMember((int)ID, null, toremove);
            }
            else if (topromote != null)
            {
                _repository.PromoteMod((int)ID, null, topromote);
            }

            List<string> moderators = _repository.GetGroupMod((int)ID).Except(new List<string> { _repository.GetGroup((int)ID).OwnerID }).ToList();
            List<string> members = _repository.GetGroupSubscribers((int)ID).Except(moderators).ToList().Except(new List<string> { _repository.GetGroup((int)ID).OwnerID }).ToList();

            List<PrayerRequestUser> moderators_users = new List<PrayerRequestUser>();
            List<PrayerRequestUser> members_users = new List<PrayerRequestUser>();

            foreach(var user in moderators)
            {
                moderators_users.Add(_UserManager.FindByIdAsync(user).GetAwaiter().GetResult());
            }
            foreach (var user in members)
            {
                members_users.Add(_UserManager.FindByIdAsync(user).GetAwaiter().GetResult());
            }
            List<List<PrayerRequestUser>> model = new List<List<PrayerRequestUser>> { moderators_users, members_users };

            IActionResult var = SetViewBag(ID);
            if (var != null)
            {
                return var;
            }

            return View(model);


        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Disband(int Id)
        {
            _repository.DisbandGroup((int)Id, null);
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreateEvent(int? ID)
        {
            Bulitin model = new Bulitin();
            IActionResult var = SetViewBag(ID);
            if (var != null)
            {
                return var;
            }
            return View(model);
        }
        public static string IndexNav => "Index";

        public static string ChatNav => "Chat";

        public static string BulletinNav => "Bulletin";
        public static string PrayersNav => "Prayers";
        public static string ManageNav => "Manage";

        public static string ManageMembersNav => "ManageMembers";


        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, IndexNav);
        public static string ChatNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChatNav);
        public static string BulletinNavClass(ViewContext viewContext) => PageNavClass(viewContext, BulletinNav);
        public static string PrayersNavClass(ViewContext viewContext) => PageNavClass(viewContext, PrayersNav);
        public static string ManageNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageNav);

        public static string ManageMembersNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageMembersNav);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.RouteData.Values["Action"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "btn-primary" : "btn-default";
        }

        private bool CheckMod(int id)
        {
            List<string> list = _repository.GetGroupMod(id);
            if (list.Contains(_UserManager.GetUserId(User)))
                return true;
            else
                return false;
        }
        private IActionResult SetViewBag(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Groups", "User", null);
            }
            ViewBag.ID = id;
            Group temp = _repository.GetGroup((int)id);
            ViewBag.GroupName = temp.GroupName;
            if(_UserManager.GetUserId(User) == temp.OwnerID)
            {
                ViewBag.Owner = "true";
            }
            if (CheckMod((int)id))
            {
                ViewBag.IsModderator = "true";
            }
            if(_UserManager.IsInRoleAsync(_UserManager.GetUserAsync(User).GetAwaiter().GetResult(), "Admin").GetAwaiter().GetResult())
            {
                ViewBag.Admin = "true";
            }
            return null;
        }

        
    }

}