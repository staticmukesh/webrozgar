using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;
using WebRozgar.Models;
using WebRozgar.ViewModels;

namespace WebRozgar.DAL
{
    public class WebRozgarRepo : IWebRozgarService, IDisposable
    {
        private WebRozgarContext _db;
        public WebRozgarRepo(WebRozgarContext db)
        {
            _db = db;
        }

        public void emailExist(string email)
        {
            var user = _db.UserProfiles.Where(r => r.Email == email).SingleOrDefault();
            if (user != null)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateEmail);
            }
        }
        public bool EmailExists(string email)
        {
            var user = _db.UserProfiles.Where(r => r.Email == email).SingleOrDefault();
            if (user != null)
            {
                return true;
            }
            return false;
        }
        public UserProfile getUserProfile(string username)
        {
            return _db.UserProfiles.Where(r => r.UserName == username).SingleOrDefault();
        }
        public bool SendMail(string to, string subject, string message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("");    //from Email Address
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                SmtpClient mailClient = new SmtpClient();
                mailClient.Host = "smtp.gmail.com";
                mailClient.Port = 587;
                string email = "";
                string password = "";
                mailClient.Credentials = new System.Net.NetworkCredential(email, password);
                mailClient.EnableSsl = true;
                mailClient.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string GetConfirmationToken(string email)
        {

            string token = null;
            if (string.IsNullOrEmpty(email))
            {
                return token;
            }

            UserProfile user = _db.UserProfiles.Where(r => r.Email == email).SingleOrDefault();
            if (user != null)
            {
                token = WebSecurity.GeneratePasswordResetToken(user.UserName);
            }
            return token;
        }
        public int Activate(string confirmationToken, string email)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.Email == email).SingleOrDefault();
            if (user == null)
            {
                return -1;      //Null entry
            }
            if (user.IsConfirmed == 1)
            {
                return 0;   //Already confirmed;
            }
            else
            {
                user.IsConfirmed = 1;
                _db.Entry(user).State = System.Data.EntityState.Modified;
                _db.SaveChanges();
                return 1; //Confirmed it
            }
        }
        public bool isConfirmed(string username)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.UserName == username).SingleOrDefault();
            if (user == null)
            {
                return false;
            }
            else
            {
                if (user.IsConfirmed == 1)
                {
                    return true;
                }
                return false;
            }

        }
        public void UpdateProfile(SeekerProfileViewModel model, string username)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.UserName == username).SingleOrDefault();
            Qualification qualification = _db.Qualifications.Where(r => r.UserName == username).SingleOrDefault();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.Gender = model.Gender;
            user.IsProfileCompleted = 1;

            if (qualification == null)
            {
                qualification = new Qualification();
                qualification.UserName = username;
                qualification.Education = model.Education;
                qualification.Bio = model.Bio;
                qualification.WorkExperience = model.WorkExperience;
                qualification.University = model.University;
                qualification.ResumePath = "~/App_Data/" + username + model.Resume.FileName;
                _db.Qualifications.Add(qualification);
            }
            else
            {
                if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(qualification.ResumePath)))
                {
                    File.Delete(System.Web.HttpContext.Current.Server.MapPath(qualification.ResumePath));
                }
                qualification.UserName = username;
                qualification.Education = model.Education;
                qualification.Bio = model.Bio;
                qualification.WorkExperience = model.WorkExperience;
                qualification.University = model.University;
                qualification.ResumePath = "~/App_Data/" + username + model.Resume.FileName;
                _db.Entry(qualification).State = System.Data.EntityState.Modified;
            }
            model.Resume.SaveAs(System.Web.HttpContext.Current.Server.MapPath(qualification.ResumePath));

            _db.Entry(user).State = System.Data.EntityState.Modified;
            _db.SaveChanges();
        }
        public void UpdateRecruiterProfile(RecruiterProfileViewModel model, string username)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.UserName == username).SingleOrDefault();
            CompanyInfo company = _db.CompanyInfos.Where(r => r.UserName == username).SingleOrDefault();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.Gender = model.Gender;
            user.IsProfileCompleted = 1;

            if (company == null)
            {
                company = new CompanyInfo();
                company.UserName = username;
                company.ComapnyName = model.ComapnyName;
                company.Website = model.Website;
                company.Type = model.Type;
                company.AboutComapany = model.AboutCompany;
                _db.CompanyInfos.Add(company);
            }
            else
            {
                company.UserName = username;
                company.ComapnyName = model.ComapnyName;
                company.Website = model.Website;
                company.Type = model.Type;
                company.AboutComapany = model.AboutCompany;
                _db.Entry(company).State = System.Data.EntityState.Modified;
            }
            _db.Entry(user).State = System.Data.EntityState.Modified;
            _db.SaveChanges();
        }
        public SeekerProfileViewModel GetSeekerProfile(string username)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.UserName == username).SingleOrDefault();
            Qualification qualification = _db.Qualifications.Where(r => r.UserName == username).SingleOrDefault();

            SeekerProfileViewModel model = new SeekerProfileViewModel();
            if ((user == null) || (qualification == null))
            {
                model.IsProfileCompleted = -1;
                return model;
            }

            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Gender = user.Gender;
            model.Phone = user.Phone;
            model.Education = qualification.Education;
            model.University = qualification.University;
            model.WorkExperience = qualification.WorkExperience;
            model.Bio = qualification.Bio;
            model.IsProfileCompleted = 1;
            return model;
        }
        public RecruiterProfileViewModel GetRecruiterProfile(string username)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.UserName == username).SingleOrDefault();
            CompanyInfo company = _db.CompanyInfos.Where(r => r.UserName == username).SingleOrDefault();
            RecruiterProfileViewModel model = new RecruiterProfileViewModel();

            if ((user == null) || (company == null))
            {
                model.IsProfileCompleted = -1;
                return model;
            }
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Gender = user.Gender;
            model.Phone = user.Phone;
            model.ComapnyName = company.ComapnyName;
            model.Website = company.Website;
            model.AboutCompany = company.AboutComapany;
            model.Type = company.Type;
            model.IsProfileCompleted = 1;
            return model;
        }
        public string GetResumePath(string username)
        {
            Qualification qualification = _db.Qualifications.Where(r => r.UserName == username).SingleOrDefault();
            if (qualification == null)
            {
                return null;
            }
            return qualification.ResumePath;
        }
        public bool IsProfileCompleted(string username)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.UserName == username).SingleOrDefault();
            if (user != null)
            {
                if (user.IsProfileCompleted == 1)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public UserProfile GetUser(string username)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.UserName == username).SingleOrDefault();
            return user;
        }
        public void SendForgotMail(string p)
        {
            UserProfile user = _db.UserProfiles.Where(r => (r.Email == p || r.UserName == p)).SingleOrDefault();
            string resetToken = WebSecurity.GeneratePasswordResetToken(user.UserName, 1440);
            string resetlink = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/account/resetPassword?resetToken=" + resetToken;
            string message = "Hello " + user.UserName + " </br> Click to reset your password " + resetlink;
            SendMail(user.Email, "Password Reset Request", message);
        }
        public bool VerifyResetToken(string resetToken)
        {
            if (WebSecurity.GetUserIdFromPasswordResetToken(resetToken) != -1)
            {
                return true;
            }
            return false;
        }
        public void DeleteResume(string username)
        {
            Qualification qualification = _db.Qualifications.Where(r => r.UserName == username).SingleOrDefault();
            if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(qualification.ResumePath)))
            {
                File.Delete(System.Web.HttpContext.Current.Server.MapPath(qualification.ResumePath));
            }
        }
        public int AddJob(FloatJobViewModel model, string username)
        {
            Job job = new Job();
            job.JobTitle = model.JobTitle;
            job.Location = model.Location;
            job.Description = model.Description;
            job.Experience = model.Experience;
            job.NoOfOpenings = model.NoOfOpenings;
            job.SkillsRequired = model.SkillsRequired;
            job.JobFloaterUsername = username;
            job.PostedDate = System.DateTime.Now;
            _db.Jobs.Add(job);

            _db.SaveChanges();

            return job.JobId;
        }
        public IQueryable<FloatJobViewModel> FloatHistory(string p)
        {
            return _db.Jobs.Where(r => r.JobFloaterUsername == p).Select(r => new FloatJobViewModel
            {
                JobTitle = r.JobTitle,
                PostedDate = r.PostedDate,
                NoOfOpenings = r.NoOfOpenings,
                Location = r.Location,
                JobId = r.JobId,
                Description = r.Description,
                SkillsRequired = r.SkillsRequired,
                Experience = r.Experience,
                FloatUsername = r.JobFloaterUsername
            }).OrderByDescending(r => r.PostedDate);
        }
        public IQueryable<FloatJobViewModel> AvailableJobs()
        {
            List<FloatJobViewModel> model= _db.Jobs.Select(r => new FloatJobViewModel
            {
                JobTitle = r.JobTitle,
                PostedDate = r.PostedDate,
                NoOfOpenings = r.NoOfOpenings,
                Location = r.Location,
                JobId = r.JobId,
                Description = r.Description,
                SkillsRequired = r.SkillsRequired,
                Experience = r.Experience,
                FloatUsername = r.JobFloaterUsername
            }).OrderByDescending(r => r.PostedDate).ToList();

            foreach (var item in model)
            {
                JobApplication app = _db.JobApplications.Where(r => r.JobId == item.JobId && r.UserName == WebSecurity.CurrentUserName).SingleOrDefault();
                if (app == null)
                {
                    item.Status = "-1";
                }
                else
                {
                    item.Status = "1";
                }
            }
            return model.AsQueryable();
        }
        public void EditJob(FloatJobViewModel model)
        {
            Job job = _db.Jobs.Where(r => r.JobId == model.JobId).SingleOrDefault();
            if (job == null)
                return;
            if (job.JobFloaterUsername != WebSecurity.CurrentUserName)
            {
                throw new Exception("Not Authorized");
            }

            job.JobTitle = model.JobTitle;
            job.Location = model.Location;
            job.NoOfOpenings = model.NoOfOpenings;
            job.SkillsRequired = model.SkillsRequired;
            job.Description = model.Description;
            job.Experience = model.Experience;
            _db.Entry(job).State = EntityState.Modified;
            _db.SaveChanges();
        }
        public FloatJobViewModel GetJob(int id)
        {
            return _db.Jobs.Where(r => r.JobId == id).Select(r => new FloatJobViewModel
            {
                JobTitle = r.JobTitle,
                PostedDate = r.PostedDate,
                NoOfOpenings = r.NoOfOpenings,
                Location = r.Location,
                JobId = r.JobId,
                Description = r.Description,
                SkillsRequired = r.SkillsRequired,
                Experience = r.Experience,
                FloatUsername = r.JobFloaterUsername
            }).SingleOrDefault();
        }
        public void DeleteJob(int id)
        {
            Job job = _db.Jobs.Where(r => r.JobId == id).SingleOrDefault();
            if (job.JobFloaterUsername != WebSecurity.CurrentUserName)
            {
                throw new Exception("Not authorized");
            }
            if (job != null)
            {
                _db.Jobs.Remove(job);
                _db.SaveChanges();
            }
        }
        public void SendMessage(string reciever, string sender, string body, string subject)
        {
            IQueryable<UserProfile> user = _db.UserProfiles.Where(r => r.UserName == reciever || r.UserName == sender).Take(2);
            if (user.Count()!=2)
            {
                throw new Exception("Error");
            }
            Message msg = new Message();
            msg.SenderUsername = sender;
            msg.UserName = reciever;
            msg.Subject = subject;
            msg.Body = body;
            msg.seen = 1;
            msg.SentTime = System.DateTime.Now;
            _db.Messages.Add(msg);
            _db.SaveChanges();
        }
        public PublicProfileViewModel GetPublicProfile(string id)
        {
            UserProfile user = _db.UserProfiles.Where(r => r.UserName == id).SingleOrDefault();
            if (user == null)
            {
                return null;
            }
            else
            {
                Qualification qual = _db.Qualifications.Where(r => r.UserName == user.UserName).SingleOrDefault();
                if (qual == null)
                {
                    CompanyInfo Companyinfo = _db.CompanyInfos.Where(r => r.UserName == user.UserName).SingleOrDefault();
                    if (Companyinfo == null)
                    {
                        return null;
                    }
                    return new PublicProfileViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        Phone = user.Phone,
                        Username = user.UserName,
                        Role = Roles.GetRolesForUser(user.UserName)[0],

                        ComapnyName = Companyinfo.ComapnyName,
                        Website = Companyinfo.Website,
                        AboutCompany = Companyinfo.AboutComapany,
                        Type = Companyinfo.Type

                    };
                }

                return new PublicProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    Phone = user.Phone,
                    Username = user.UserName,
                    Role = Roles.GetRolesForUser(user.UserName)[0],

                    Education = qual.Education,
                    University = qual.University,
                    Bio = qual.Bio,
                    WorkExperience = qual.WorkExperience,
                    Resume = qual.ResumePath
                };
            }

        }
        public int NoOfUnseenMessages(string username)
        {
            return _db.Messages.Where(r => r.UserName == username && r.seen == 1).Count();
        }
        public List<SearchResultViewModel> SearchJob(string query)
        {
            string stopwords = "he them most some water one used rst often into all not its been his more other but this their has can your such at it which you be an from on by that for with or are as is in to a and of the";
            string[] search = query.Split(' ');
            List<SearchResultViewModel> model1 = new List<SearchResultViewModel>();
            List<SearchResultViewModel> model2 = new List<SearchResultViewModel>();

            model1 = _db.Jobs.Where(r => r.Description.Contains(query) || r.JobTitle.Contains(query) || r.SkillsRequired.Contains(query)).Select(r => new SearchResultViewModel
            {
                JobId=r.JobId,
                JobDescription=r.Description,
                JobTitle=r.JobTitle,
                FloaterUsername=r.JobFloaterUsername,
                PostedDate=r.PostedDate,
                Location=r.Location,
            }).ToList();

            foreach (string item in search)
            {
                if (!stopwords.Contains(item))
                {
                    model2 = _db.Jobs.Where(r => r.Description.Contains(item) || r.JobTitle.Contains(item) || r.SkillsRequired.Contains(item)).Select(r => new SearchResultViewModel
                    {
                        JobId = r.JobId,
                        JobDescription = r.Description,
                        JobTitle = r.JobTitle,
                        FloaterUsername = r.JobFloaterUsername,
                        PostedDate = r.PostedDate,
                        Location = r.Location,
                    }).ToList();
                    int check = -1;
                    foreach (SearchResultViewModel newmodel in model1)
                    {
                        
                    }
                    model1.Union(model2).Distinct();
                }
            }
            return model1;
        }
        public IQueryable<JobApplicationViewModel> GetAppliedUser(int id)
        {
            return _db.JobApplications.Where(r => r.JobId == id).Select(r => new JobApplicationViewModel { 
                UserName=r.UserName,
                JobId=r.JobId,
                AppId=r.AppId,
                status=r.status
            });
        }
        public string SetStatus(int jobid, string userid, string status, string floatername)
        {
            try
            {
                Job job = _db.Jobs.Where(r => r.JobFloaterUsername == floatername && r.JobId == jobid).SingleOrDefault();

                if (job == null)
                    return "-4";

                JobApplication application = _db.JobApplications.Where(r => r.JobId == jobid && r.UserName == userid).SingleOrDefault();
                application.status = status;
                _db.Entry(application).State = EntityState.Modified;
                _db.SaveChanges();
                return status;
            }
            catch
            {
                return "-4";
            }
        }
        public string Apply(int jobid, string p)
        {
            JobApplication app = _db.JobApplications.Where(r => r.JobId == jobid && r.UserName == p).SingleOrDefault();
            if (app == null)
            {
                app = new JobApplication();
                app.JobId = jobid;
                app.status = "2";
                app.UserName = p;
                _db.JobApplications.Add(app);
                _db.SaveChanges();
                return "Applied";
            }
            else
            {
                return "Already applied";
            }
        }
        public IQueryable<Message> GetAllMessages(string username)
        {
            return _db.Messages.Where(r => r.UserName == username).Select(r => r).OrderByDescending(r=>r.SentTime);
        }
        public string GetUsernameFromJobId(int jobid)
        {
            return _db.Jobs.Where(r => r.JobId == jobid).Select(r => r.JobFloaterUsername).SingleOrDefault();
        }
        public Message GetMessage(int id)
        {
            Message msg = _db.Messages.Where(r => r.Id == id && r.UserName == WebSecurity.CurrentUserName).SingleOrDefault();
            if (msg != null)
            {
                msg.seen = 0;
                _db.Entry(msg).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return msg;
        }
        public string GetUsername(int id)
        {
            return _db.UserProfiles.Where(r => r.UserId == id).Select(r => r.UserName).SingleOrDefault();
        }
        public IQueryable<JobApplicationViewModel> GetApplicationStatus(string p)
        {
            return _db.JobApplications.Where(r => r.UserName == p).Select(r => new JobApplicationViewModel
            {
                JobId=r.JobId,
                status=r.status,
                AppId=r.AppId,
                UserName=r.UserName
            });
        }
        public void ContactUs(ContactViewModel model)
        {
            SendMail("John@doe", "Message from "+model.Name+" and Email " + model.Email, model.Message);
        }
        public void Dispose()
        {
            _db.Dispose();
        }

    }

    public interface IWebRozgarService
    {
        string GetUsername(int id);
        void emailExist(string email);
        bool EmailExists(string email);
        Message GetMessage(int id);
        string GetConfirmationToken(string email);
        string GetUsernameFromJobId(int jobid);
        int Activate(string confirmationToken, string email);
        bool SendMail(string to, string subject, string message);
        bool isConfirmed(string username);
        UserProfile getUserProfile(string username);
        bool IsProfileCompleted(string username);
        void UpdateProfile(SeekerProfileViewModel model, string username);
        UserProfile GetUser(string username);
        void SendForgotMail(string p);
        bool VerifyResetToken(string resetToken);
        SeekerProfileViewModel GetSeekerProfile(string username);
        string GetResumePath(string username);
        void UpdateRecruiterProfile(RecruiterProfileViewModel model, string username);
        RecruiterProfileViewModel GetRecruiterProfile(string username);
        void DeleteResume(string p);
        int AddJob(FloatJobViewModel job, string username);
        IQueryable<FloatJobViewModel> FloatHistory(string p);
        void EditJob(FloatJobViewModel job);
        FloatJobViewModel GetJob(int id);
        IQueryable<FloatJobViewModel> AvailableJobs();
        void DeleteJob(int id);
        List<SearchResultViewModel> SearchJob(string query);
        IQueryable<JobApplicationViewModel> GetAppliedUser(int id);
        string SetStatus(int jobid, string userid, string status, string floaterid);
        string Apply(int jobid, string p);
        PublicProfileViewModel GetPublicProfile(string id);
        int NoOfUnseenMessages(string username);
        IQueryable<Message> GetAllMessages(string username);
        void SendMessage(string reciever, string sender, string body, string subject);
        IQueryable<JobApplicationViewModel> GetApplicationStatus(string p);
        void ContactUs(ContactViewModel model);
    }
}
