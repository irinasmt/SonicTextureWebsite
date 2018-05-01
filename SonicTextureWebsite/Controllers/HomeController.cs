using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using WebApplication2.Models;
using WebApplication2.Repos;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactUsModel contactusModel)
        {
            MailMessage mail = new MailMessage(contactusModel.Email, "admin@sonictexture.co.uk");
            var client = GenerateSmtpClient();
            mail.Subject = "client contacting";
            mail.Body = contactusModel.Message;
            client.Send(mail);

            return View();
        }

        [HttpGet]
        public ActionResult TestApi()
        {
            var isUserCertified = User.IsInRole("Production");
            ViewBag.Message = isUserCertified
                ? "You are certified to use the Api"
                : "You are no certified to use the skill, you can enter your details bellow to submit for certification process.";
            return View();
        }

        private static SmtpClient GenerateSmtpClient()
        {

            SmtpClient client = new SmtpClient
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                Credentials = new System.Net.NetworkCredential("admin@sonictexture.co.uk", "Rocket2019"),
                EnableSsl = true

            };
            return client;
        }

        public ActionResult Demos()
        {
            ViewBag.Message = "Demos";

            return View();
        }

        public ActionResult HowItWorks()
        {
            ViewBag.Message = "How it works";

            return View();
        }

        [HttpPost]
        [Authorize]
        public bool AddTestEmail(string firstEmail, string secondEmail)
        {
            var emails = new List<string>();
            if (!string.IsNullOrWhiteSpace(firstEmail) && EmailAddress.IsValidEmail(firstEmail))
            {
                emails.Add(firstEmail);
            }
            if (!string.IsNullOrWhiteSpace(secondEmail) && EmailAddress.IsValidEmail(secondEmail))
            {
                emails.Add(secondEmail);
            }

            if (emails.Count == 0)
            {
                return false;
            }
            
            var userId = User.Identity.GetUserId();
            var userRepo = new UsersRepo();
            return userRepo.InsertTestEmails(userId, emails);
        }


        [HttpPost]
        [Authorize]
        public bool InsertCertification(string name, string email, string intention, string exampleMessage)
        {
            if (!EmailAddress.IsValidEmail(email) || string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(intention) || string.IsNullOrWhiteSpace(exampleMessage))
                return false;
            var businessRepo = new BusinessRepo();
            return businessRepo.InsertSubmitCertification(name, email, intention, exampleMessage);
        }

        [HttpGet]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        
    }
}