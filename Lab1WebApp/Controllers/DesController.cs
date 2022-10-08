using LAB1;
using Lab1WebApp.Models;
using Lab1WebApp.Models.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Lab1WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DesController: ControllerBase
    {
        private readonly IDES Des;

        public DesController(IDES Des)
        {
            this.Des = Des;
        }

        [HttpGet("Encrypt")]
        public IActionResult EncryptData([FromQuery] EncryptDataModel encryptDataModel) 
        {
            var responseModel = new Response<EncryptMetadataModel>();

            if (string.IsNullOrEmpty(encryptDataModel.Text) || string.IsNullOrEmpty(encryptDataModel.Key)) 
            {
                responseModel.Success = false;
                responseModel.Errors = new List<string>() { "Invalid input data" };
                return BadRequest(responseModel);
            }

            if (encryptDataModel.Key.Length > 8) 
            {
                responseModel.Success = false;
                responseModel.Errors = new List<string>() { "Key is greater than 64 bits" };
                return BadRequest(responseModel);
            }

            try
            {
                var response = Des.Encrypt(encryptDataModel.Text, encryptDataModel.Key);
                var metaDataModel = new EncryptMetadataModel()
                {
                    EntropiaData = Des.GetEntropiaPerRound,
                    KeyModels = Des.GetkeyForRounds.Select(x => new KeyModel()
                    {
                        RawData = x,
                        StringFormat = Des.ConvertBitsToText(x.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray(), 48)
                    }).ToList()
                };
                responseModel.Success = true;
                responseModel.ResponseText = response.ToString();
                responseModel.Metadata = metaDataModel;
            }
            catch (System.Exception msg)
            {
                responseModel.Success = false;
                responseModel.Errors = new List<string>() {  msg.Message};
                return BadRequest(responseModel);
            }

            return Ok(responseModel);
        }

        [HttpGet("Decrypt")]
        public IActionResult DecryptData([FromQuery] DecryptDataModel encryptDataModel) 
        {
            var responseModel = new Response<DecryptMetadataModel>();
            if (string.IsNullOrEmpty(encryptDataModel.EncryptedText) || string.IsNullOrEmpty(encryptDataModel.Key))
            {
                responseModel.Success = false;
                responseModel.Errors = new List<string>() { "Invalid input data" };
                return BadRequest(responseModel);
            }

            if (encryptDataModel.Key.Length > 8)
            {
                responseModel.Success = false;
                responseModel.Errors = new List<string>() { "Key is greater than 64 bits" };
                return BadRequest(responseModel);
            }


            try
            {
                var decryptedMessage = Des.Decrypt(encryptDataModel.EncryptedText, encryptDataModel.Key);
                responseModel.Success = true;
                responseModel.ResponseText = decryptedMessage;
                
            }
            catch (System.Exception msg)
            {
                responseModel.Success = false;
                responseModel.Errors = new List<string>() { msg.Message };
                return BadRequest(responseModel);
            }

            return Ok(responseModel);
        }
    }
}
