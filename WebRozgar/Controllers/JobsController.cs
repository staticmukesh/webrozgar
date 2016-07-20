using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRozgar.Models;
using WebRozgar.DAL;
using WebRozgar.ViewModels;
using WebRozgar.Helpers;
using System.Text;

namespace WebRozgar.Controllers
{
    [CustomAuthorize]    
    public class JobsController : Controller
    {
        private WebRozgarContext db = new WebRozgarContext();
        private IWebRozgarService _Service;

        public JobsController(IWebRozgarService service)
        {
            _Service = service;
        }
        public ActionResult Index()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            return View(db.Jobs.ToList());
        }
        public ActionResult Details(int id = 0)
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
            FloatJobViewModel job = _Service.GetJob(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            List<JobApplicationViewModel> applications = _Service.GetAppliedUser(id).ToList();
            DetailViewModel model = new DetailViewModel();
            model.jobmodel = job;
            int check = -1;
            foreach(var item in applications)
            {
                if ((item.JobId == job.JobId)&&(item.UserName==User.Identity.Name))
                {
                    check = 1;
                    break;
                }
            }
            if (check == 1)
            {
                model.jobmodel.Status = "1";
            }
            else
            {
                model.jobmodel.Status = "-1";
            }
            model.applicationmodel = applications.AsQueryable();
            return View(model);
        }

        [Authorize(Roles="recruiter")]
        public string SetStatus(int jobid, string userid, string status)
        {
            string st= _Service.SetStatus(jobid, userid, status, User.Identity.Name);
            if (st == "1")
            {
                _Service.SendMessage(userid, User.Identity.Name, "You have been selected in ", "Congratulation");
                return "Selected";
            }
            else if (st == "0")
            {
                _Service.SendMessage(userid, User.Identity.Name, "You have been shortlisted in ", "Congratulation");
                return "Shortlisted";
            }
            else if (st == "-1")
            {
                _Service.SendMessage(userid, User.Identity.Name, "You have been not shortlisted in ", "Sorry");
                return "Not Shortlisted";
            }
            else if (st == "-2")
            {
                _Service.SendMessage(userid, User.Identity.Name, "You have been not selected in ", "Sorry");
                return "Not Selected";
            }
            return "Error";
        }

        [Authorize(Roles = "recruiter")]
        public ActionResult FloatJob()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            FloatJobViewModel model = new FloatJobViewModel();
            RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
            ViewBag.FirstName = tempmodel.FirstName;
            ViewBag.LastName = tempmodel.LastName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="recruiter")]
        public ActionResult FloatJob(FloatJobViewModel job)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
            ViewBag.FirstName = tempmodel.FirstName;
            ViewBag.LastName = tempmodel.LastName;
            if (ModelState.IsValid)
            {
                int JobId= _Service.AddJob(job,User.Identity.Name);
                return RedirectToAction("Details", new { Id= JobId });
            }
            return View(job);
        }

        [Authorize(Roles="recruiter")]
        public ActionResult Edit(int id = 0)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
            ViewBag.FirstName = tempmodel.FirstName;
            ViewBag.LastName = tempmodel.LastName;

            FloatJobViewModel job = _Service.GetJob(id);
            
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "recruiter")]
        public ActionResult Edit(FloatJobViewModel job)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            if (ModelState.IsValid)
            {
                try
                {
                    _Service.EditJob(job);
                    return RedirectToAction("FloatHistory","My");
                }
                catch
                {
                    return View(job);
                }
            }
            return View(job);
        }

        [Authorize(Roles="recruiter")]
        public ActionResult Delete(int id = 0)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            RecruiterProfileViewModel tempmodel = _Service.GetRecruiterProfile(User.Identity.Name);
            ViewBag.FirstName = tempmodel.FirstName;
            ViewBag.LastName = tempmodel.LastName;
            FloatJobViewModel job = _Service.GetJob(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            _Service.DeleteJob(id);
            return RedirectToAction("FloatHistory","My");
        }

        [Authorize(Roles="seeker")]
        public ActionResult AvailableJobs()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            SeekerProfileViewModel tempmodel = _Service.GetSeekerProfile(User.Identity.Name);
            ViewBag.FirstName = tempmodel.FirstName;
            ViewBag.LastName = tempmodel.LastName;
            IQueryable<FloatJobViewModel> model = _Service.AvailableJobs();
            return View(model);
        }

        [Authorize(Roles = "seeker")]
        [HttpPost]
        public string Apply(int jobid)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            string msg1 = _Service.Apply(jobid, User.Identity.Name);
            if (msg1 == "Applied")
            {
                string msg2 = User.Identity.Name + " has applied to your job" + Url.Action("Details", "Jobs", new { Id= jobid });

                StringBuilder builder = new StringBuilder(100000);
                builder.Append("<a href=\"");
                builder.Append(Url.Action("Profile","Home",new {id = User.Identity.Name}));
                builder.Append("\">");
                builder.Append(User.Identity.Name);
                builder.Append("</a> has applied to ");
                builder.Append("<a href=\"");
                builder.Append(Url.Action("Details", "Jobs", new { id = jobid }));
                builder.Append("\">Job</a> floated by you.");
                builder.Append("</br>");
                _Service.SendMessage(_Service.GetUsernameFromJobId(jobid), User.Identity.Name, builder.ToString(), "Application recieved");
                return "Applied";
            }
            return "Already Applied";
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}