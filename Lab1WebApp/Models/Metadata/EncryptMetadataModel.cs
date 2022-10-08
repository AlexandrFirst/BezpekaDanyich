using System.Collections.Generic;

namespace Lab1WebApp.Models.Metadata
{
    public class EncryptMetadataModel
    {
        public int[][] EntropiaData { get; set; }
        public List<KeyModel> KeyModels { get; set; }
    }

    public class KeyModel 
    {
        public string RawData { get; set; }
        public string StringFormat { get; set; } 
    }
}
