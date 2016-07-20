using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using WebRozgar.DAL;
using WebRozgar.ViewModels;

namespace WebRozgar.Controllers
{
    public class HomeController : Controller
    {
        private IWebRozgarService _Service;

        public HomeController(IWebRozgarService service)
        {
            _Service = service;
        }

        public ActionResult Index(string query = null)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            if (Request.IsAjaxRequest())
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    List<SearchResultViewModel> model = _Service.SearchJob(query);
                    return PartialView("_SearchResult", model);
                }
                return PartialView("_SearchResult", null);
            }
            List<SearchResultViewModel> model1 = null;
            return View(model1);
        }

        public ActionResult About()
        {
            return View();
        }

        [Authorize]
        public ActionResult Profile(string id = null)
        {
            if (id == null)
            {
                return RedirectToAction("Profile", "My");
            }

            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);

            PublicProfileViewModel model = _Service.GetPublicProfile(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            if (model.Role == "seeker")
            {

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Not Pursuing Graduation", Value = "1" });
                items.Add(new SelectListItem { Text = "B.A", Value = "2" });
                items.Add(new SelectListItem { Text = "B.Arch", Value = "3" });
                items.Add(new SelectListItem { Text = "BCA", Value = "4" });
                items.Add(new SelectListItem { Text = "B.B.A", Value = "5" });
                items.Add(new SelectListItem { Text = "B.Com", Value = "6" });
                items.Add(new SelectListItem { Text = "B.Ed", Value = "7" });
                items.Add(new SelectListItem { Text = "BDS", Value = "8" });
                items.Add(new SelectListItem { Text = "BHM", Value = "9" });
                items.Add(new SelectListItem { Text = "B.Pharma", Value = "10" });
                items.Add(new SelectListItem { Text = "B.Sc", Value = "11" });
                items.Add(new SelectListItem { Text = "B.Tech/B.E.", Value = "12" });
                items.Add(new SelectListItem { Text = "LLB", Value = "13" });
                items.Add(new SelectListItem { Text = "MBBS", Value = "14" });
                items.Add(new SelectListItem { Text = "Diploma", Value = "15" });
                items.Add(new SelectListItem { Text = "BVSC", Value = "16" });
                items.Add(new SelectListItem { Text = "Others", Value = "999" });

                model.Education = items.Select(m => m).Where(r => r.Value == model.Education).SingleOrDefault().Text;


                List<SelectListItem> expitems = new List<SelectListItem>();
                expitems.Add(new SelectListItem { Text = "Fresher", Value = "1" });
                expitems.Add(new SelectListItem { Text = "0-6 months", Value = "2" });
                expitems.Add(new SelectListItem { Text = "6-12 months", Value = "3" });
                expitems.Add(new SelectListItem { Text = "1-3 years", Value = "4" });
                expitems.Add(new SelectListItem { Text = "3-7 years", Value = "5" });
                expitems.Add(new SelectListItem { Text = ">7 years", Value = "6" });

                model.WorkExperience = expitems.Select(m => m).Where(r => r.Value == model.WorkExperience).SingleOrDefault().Text;
            }
            else
            {
                List<SelectListItem> typeitem = new List<SelectListItem>();
                typeitem.Add(new SelectListItem { Text = "Startup", Value = "1" });
                typeitem.Add(new SelectListItem { Text = "Government", Value = "2" });
                typeitem.Add(new SelectListItem { Text = "Public", Value = "3" });
                typeitem.Add(new SelectListItem { Text = "Private", Value = "4" });

                model.Type = typeitem.Select(m => m).Where(r => r.Value == model.Type).SingleOrDefault().Text;
            }
            return View(model);
        }

        [HttpPost]
        public string Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return "Error while processing your request";
            }
            try
            {
                _Service.ContactUs(model);
                return "We will contact you shortly";
            }
            catch
            {
                return "Error while processing your request";
            }
        }
    }
}
