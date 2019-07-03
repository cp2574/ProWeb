using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiDu_OCR
{
    class BaiduAipOcr
    {
        static string ApiKey = "evWfVOlWzGPQau6wWQOcBeoD";
        static string SecretKey = "NPsNpzveOc57itLyvAi4KsxrtP43hEsQ";
        public static string TextDiscern(string imgfile)
        {
            var image = File.ReadAllBytes(imgfile);
            var client = new Baidu.Aip.Ocr.Ocr(ApiKey, SecretKey);
            // 通用文字识别
            var result = client.GeneralBasic(image, null);

            var wenzi = result.Root.Last.Values();

            string message = null;
            foreach (JToken item in wenzi)
            {
                var wenZiMessage = JsonConvert.DeserializeObject<WenZiMessage>(item.ToString());
                message += wenZiMessage.words + ",";
            }
            message = message.TrimEnd(',') + "。";

            return message;
        }
    }

    public class WenZiMessage {
        public string words { get; set; }
    }
}
