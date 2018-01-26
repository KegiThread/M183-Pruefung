using System;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using Pruefung_Praktisch_Musterloesung.Models;

namespace Pruefung_Praktisch_Musterloesung.Controllers
{
    public class Lab4Controller : Controller
    {

        /**
        * 
        * Es is spannend, weil bei mehreren Fehlern und Falscheingaben k;nnen muster entstehen
        * somit kann man herleiten das gewisse requests einer IP als Hacker entlarvt werden kann 
        * 
        * */

        public ActionResult Index()
        {

            Lab4IntrusionLog model = new Lab4IntrusionLog();
            return View(model.getAllData());
        }

        [HttpPost]
        public ActionResult Login()
        {
            var username = Request["username"];
            var password = Request["password"];

            var emailaddress = Request["email"];

            bool intrusion_detected = false;

            var browser = Request.Browser.Platform;
            var ip = Request.UserHostAddress;

            Lab4IntrusionLog model = new Lab4IntrusionLog();

            if (!CheckEmail(emailaddress))
            {
                model.logIntrusion(ip, browser, "Email dont have a valid format");
            }
            if (!CheckPW(password))
            {
                model.logIntrusion(ip, browser, "Password dont have a valid format ");
            }

            if (intrusion_detected)
            {
                return RedirectToAction("Index", "Lab4");
            }
            else
            {
                // check username and password
                // this does not have to be implemented!
                return RedirectToAction("Index", "Lab4");
            }
        }

        public bool CheckEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress); // Check if valid email address format

                if (emailaddress.ToLower() == emailaddress) // Check if lowercase is same as input
                {
                    return true;
                }
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool CheckPW(string password)
        {
            if (password.Length >= 10 && password.Length <= 20) // Check if 
            {
                if (password.ToLower() != password && password.ToUpper() != password)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}