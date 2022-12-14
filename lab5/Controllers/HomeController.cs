using lab5.Models;
using LiqPay.SDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using XSystem.Security.Cryptography;
using static System.Collections.Specialized.BitVector32;

namespace lab5.Controllers
{
    public class HomeController : Controller
    {
        private readonly LiqPayData liqpayDataOptions;

        public HomeController(IOptions<LiqPayData> liqpayDataOptions)
        {
            this.liqpayDataOptions = liqpayDataOptions.Value;
        }

        public IActionResult Index()
        {

            var liqPayClient = new LiqPayClient(liqpayDataOptions.PublicKey, liqpayDataOptions.PrivateKey);
            var data = liqPayClient.GenerateDataAndSignature(new LiqPay.SDK.Dto.LiqPayRequest()
            {
                Version = 3,
                ActionPayment = LiqPay.SDK.Dto.Enums.LiqPayRequestActionPayment.Paydonate,
                Action = LiqPay.SDK.Dto.Enums.LiqPayRequestAction.Paydonate,
                Amount = 10,
                Currency = "USD",
                Description = "123",
                OrderId = Guid.NewGuid().ToString()
            });
            //var liqpay_data_object = new
            //{
            //    version = 3,
            //    public_key = liqpayDataOptions.PublicKey,
            //    action = "paydonate",
            //    amount = 10,
            //    currency = "USD",
            //    description = "Whatever",
            //    order_id = "abc"
            //};

            //var string_serializedData = JsonConvert.SerializeObject(liqpay_data_object);
            ////var encodedData = objectToString(string_serializedData);
            //var encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(string_serializedData));


            //var signatureStringData = liqpayDataOptions.PrivateKey + string_serializedData + liqpayDataOptions.PrivateKey;
            //var signature_sha1 = Hash(Encoding.UTF8.GetBytes(signatureStringData));
            //var encodedSignaturte = Convert.ToBase64String(Encoding.Unicode.GetBytes(signature_sha1));

            return View(new HomePaymentInfo()
            {
                Data = data.Key.ToBase64String(),
                Signature = data.Value.ToBase64String()
            });
        }

        public string Hash(byte[] temp)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(temp);
                return Convert.ToBase64String(hash);
            }
        }

        private string objectToString(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private object stringToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}
