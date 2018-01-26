using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using Pruefung_Praktisch_Musterloesung.Models;

namespace Pruefung_Praktisch_Musterloesung.Controllers
{
    public class Lab2Controller : Controller
    {

        /**
        * 
        * --Session Fixation--
        * Hacker geht auf IndexSeite, beim Element FORM ACTION kann er die generierte SessionID auslesen
        * und weiss wie sie heisst. Wenn der Hacker nun eine Login-URL an ein Opfer schickt,
        * mit der ausgelesenen SID, zb http://localhost:8080/lab2/login?sid={SID}, und das Opfer sich 
        * einloggt, kann der Hacker mit der selben URL auf den Account zugreifen.
        * 
        * 
        * --Cross-Site-Tracing--
        * Beispielsweise kann man ein Kommentar auf einer Platform  posten(mit script),welches
        * auf die Bank eines Opfers referenziert. Wenn das Opfer
        * nun eine aktive Session bei der Bank hat, könnte man Transaktionen ausfuehren lassen,
        * nur wenn das Opfer den Kommentar anschaut.

        * 
        * 
        * */

        public ActionResult Index() {

            var sessionid = Request.QueryString["sid"];

            if (string.IsNullOrEmpty(sessionid))
            {
                var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
                sessionid = string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
            }

            ViewBag.sessionid = sessionid;

            return View();
        }

        [HttpPost]
        public ActionResult Login()
        {
            var username = Request["username"];
            var password = Request["password"];
            var sessionid = Request.QueryString["sid"];

            // hints:
            //var used_browser = Request.Browser.Platform;
            //var ip = Request.UserHostAddress;

            Lab2Userlogin model = new Lab2Userlogin();

            if (model.checkCredentials(username, password))
            {
                //encryption of SID
                if (string.IsNullOrEmpty(sessionid))
                {
                    var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
                    sessionid = string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
                }

                ViewBag.sessionid = sessionid;
                model.storeSessionInfos(username, password, sessionid);

                HttpCookie c = new HttpCookie("sid");
                c.Expires = DateTime.Now.AddMonths(2);
                c.Value = sessionid;
                Response.Cookies.Add(c);

                return RedirectToAction("Backend", "Lab2");
            }
            else
            {
                ViewBag.message = "Wrong Credentials";
                return View();
            }
        }

        public ActionResult Backend()
        {
            var sessionid = "";

            if (Request.Cookies.AllKeys.Contains("sid"))
            {
                sessionid = Request.Cookies["sid"].Value.ToString();
            }           

            if (!string.IsNullOrEmpty(Request.QueryString["sid"]))
            {
                sessionid = Request.QueryString["sid"];
            }
            
            // hints:
            //var used_browser = Request.Browser.Platform;
            //var ip = Request.UserHostAddress;

            Lab2Userlogin model = new Lab2Userlogin();

            if (model.checkSessionInfos(sessionid))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Lab2");
            }              
        }
    }
}