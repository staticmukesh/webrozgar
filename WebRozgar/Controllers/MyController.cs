using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRozgar.DAL;
using WebRozgar.Helpers;
using WebRozgar.Models;
using WebRozgar.ViewModels;

namespace WebRozgar.Controllers
{
    [CustomAuthorize]
    public class MyController : Controller
    {
        //
        // GET: /My/
        private IWebRozgarService _Service;

        public MyController(IWebRozgarService service)
        {
            _Service = service;
        }


        public ActionResult Profile()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);

            if (User.IsInRole("seeker"))
            {
                SeekerProfileViewModel smodel = _Service.GetSeekerProfile(User.Identity.Name);
                MyProfileViewModel model = new MyProfileViewModel();
                model.FirstName = smodel.FirstName;
                model.LastName = smodel.LastName;
                model.University = smodel.University;
                model.Bio = smodel.Bio;
                model.Education = smodel.Education;

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

                model.Education = items.Select(m => m).Where(r => r.Value == smodel.Education).SingleOrDefault().Text;


                List<SelectListItem> expitems = new List<SelectListItem>();
                expitems.Add(new SelectListItem { Text = "Fresher", Value = "1" });
                expitems.Add(new SelectListItem { Text = "0-6 months", Value = "2" });
                expitems.Add(new SelectListItem { Text = "6-12 months", Value = "3" });
                expitems.Add(new SelectListItem { Text = "1-3 years", Value = "4" });
                expitems.Add(new SelectListItem { Text = "3-7 years", Value = "5" });
                expitems.Add(new SelectListItem { Text = ">7 years", Value = "6" });

                model.WorkExperience = expitems.Select(m => m).Where(r => r.Value == smodel.WorkExperience).SingleOrDefault().Text;

                return View(model);
            }
            else if (User.IsInRole("recruiter"))
            {
                RecruiterProfileViewModel model = _Service.GetRecruiterProfile(User.Identity.Name);
                List<SelectListItem> typeitem = new List<SelectListItem>();
                typeitem.Add(new SelectListItem { Text = "Startup", Value = "1" });
                typeitem.Add(new SelectListItem { Text = "Government", Value = "2" });
                typeitem.Add(new SelectListItem { Text = "Public", Value = "3" });
                typeitem.Add(new SelectListItem { Text = "Private", Value = "4" });

                model.Type = typeitem.Select(m => m).Where(r => r.Value == model.Type).SingleOrDefault().Text;
                return View("recruiterprofileview", model);
            }
            else
            {
                //Have to think about it :p
                return View();
            }
        }
        public ActionResult Messages()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);

            if (User.IsInRole("seeker"))
            {
                SeekerProfileViewModel tempmodel = _Service.GetSeekerProfile(User.Identity.Name);
                ViewBag.FirstName = tempmodel.FirstName;
                ViewBag.LastName = tempmodel.LastName;
            }
            else
            {
                RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
                ViewBag.FirstName = tempmodel.FirstName;
                ViewBag.LastName = tempmodel.LastName;
            }
            IQueryable<Message> messages = _Service.GetAllMessages(User.Identity.Name);
            return View(messages);
        }
        public ActionResult Message(int id)
        {
            if (User.IsInRole("seeker"))
            {
                SeekerProfileViewModel tempmodel = _Service.GetSeekerProfile(User.Identity.Name);
                ViewBag.FirstName = tempmodel.FirstName;
                ViewBag.LastName = tempmodel.LastName;
            }
            else
            {
                RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
                ViewBag.FirstName = tempmodel.FirstName;
                ViewBag.LastName = tempmodel.LastName;
            }
            Message msg = _Service.GetMessage(id);
            if (msg == null)
            {
                return HttpNotFound();
            }
            return View(msg);
        }
        
        [Authorize(Roles = "seeker")]
        public ActionResult ApplicationStatus()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);

            SeekerProfileViewModel tempmodel = _Service.GetSeekerProfile(User.Identity.Name);
            ViewBag.FirstName = tempmodel.FirstName;
            ViewBag.LastName = tempmodel.LastName;
            IQueryable<JobApplicationViewModel> model = _Service.GetApplicationStatus(User.Identity.Name);
            return View(model);
        }

        public ActionResult Reply(string id,string subject)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);

            if (User.IsInRole("seeker"))
            {
                SeekerProfileViewModel tempmodel = _Service.GetSeekerProfile(User.Identity.Name);
                ViewBag.FirstName = tempmodel.FirstName;
                ViewBag.LastName = tempmodel.LastName;
            }
            else
            {
                RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
                ViewBag.FirstName = tempmodel.FirstName;
                ViewBag.LastName = tempmodel.LastName;
            }

            MessageViewModel model = new MessageViewModel();
            model.UserName = id;
            model.Subject = "Re: " + subject;
            return View("SendMessage", model);
        }

        [Authorize(Roles = "seeker")]
        public ActionResult Resume()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);

            SeekerProfileViewModel smodel = _Service.GetSeekerProfile(User.Identity.Name);
            MyProfileViewModel model = new MyProfileViewModel();
            model.FirstName = smodel.FirstName;
            model.LastName = smodel.LastName;
            return View(model);
        }

        [Authorize(Roles = "recruiter")]
        public ActionResult PublicResume(string username)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);

            SeekerProfileViewModel smodel = _Service.GetSeekerProfile(username);
            MyProfileViewModel model = new MyProfileViewModel();
            model.FirstName = smodel.FirstName;
            model.LastName = smodel.LastName;
            return View(model);
        }

        public ActionResult SendMessage(string username, string subject)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            if (User.IsInRole("recruiter"))
            {
                RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
                ViewBag.FirstName = tempmodel.FirstName;
                ViewBag.LastName = tempmodel.LastName;
            }
            else if (User.IsInRole("seeker"))
            {
                SeekerProfileViewModel tempmodel = _Service.GetSeekerProfile(User.Identity.Name);
                ViewBag.FirstName = tempmodel.FirstName;
                ViewBag.LastName = tempmodel.LastName;
            }
            MessageViewModel model = new MessageViewModel();
            if ((username != null) ||(subject != null))
            {
                model.UserName = username;
                model.Subject = subject;
            }
            return View(model);
        }

        [HttpPost]
        public string SendMessage(Message message)
        {
            try
            {
                _Service.SendMessage(message.UserName, User.Identity.Name, message.Body, message.Subject);
                return "Message sent successfully";
            }
            catch
            {
                return "Message can't be sent";
            }
        }

        [Authorize(Roles = "seeker")]
        public ActionResult Status()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            return View();
        }

        [Authorize(Roles = "recruiter")]
        public ActionResult FloatHistory()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);

            RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
            @ViewBag.FirstName = tempmodel.FirstName;
            @ViewBag.LastName = tempmodel.LastName;
            IEnumerable<FloatJobViewModel> model = _Service.FloatHistory(User.Identity.Name);
            return View(model);
        }
    }
}
