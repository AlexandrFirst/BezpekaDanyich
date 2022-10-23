using LAB2.Models;
using LAB2.Services;
using LAB2.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;

namespace LAB2.Controllers
{
    public class RSAController : Controller
    {
        private readonly IRsaManager manager;

        public RSAController(IRsaManager manager)
        {
            this.manager = manager;
        }

        public IActionResult Login() 
        {
            var publicKeys = manager.GetPublicKey();

            return View(new LoginModel() 
            {
                PublicRSAKey = publicKeys.pemFormat.ToBase64()
            });
        }

        [HttpGet]
        public IActionResult ShowInfo() 
        {
            var publicKeys = manager.GetPublicKey();
            var privateKeys = manager.GetPrivateKey();

            var showInfoModel = new ShowInfoModel()
            {
                PrivateKey = privateKeys.pemFormat,
                PublicKey = publicKeys.pemFormat,
            };
            return View(showInfoModel);
        }

        [HttpGet]
        public IActionResult UpdateKey() 
        {
            manager.UpdateKeys();
            return RedirectToAction("ShowInfo", "RSA");
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginPostModel loginPostModel) 
        {
            var privateKey = manager.GetPrivateKey();
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privateKey.rawFormat);

            var decryptedMail = decryptInfo(csp, loginPostModel.Mail);
            var decryptedPassword = decryptInfo(csp, loginPostModel.Password);

            if (decryptedMail.Equals("admin@gmail.com") && decryptedPassword.Equals("admin")) 
            {
                return Ok(new { message = "Creds are correct"});
            }
            else 
            {
                return BadRequest(new { message = "Creds are INcorrect" });
            }
            
        }

        private string decryptInfo(RSACryptoServiceProvider csp, string data) 
        {
            var bytesCypherText = Convert.FromBase64String(data);
            var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

            var plainTextData = System.Text.Encoding.ASCII.GetString(bytesPlainTextData);
            return plainTextData;
        }

    }
}
