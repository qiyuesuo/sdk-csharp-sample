using QiyuesuoClient.Http;
using System;

namespace sdk_csharp_sample_v2
{
    class Program
    {
        static void Main(string[] args)
        {
            string accessSecret = "替换为开放平台的SECRET";
            string accessToken = "替换为开放平台的TOKEN";
            string serverUrl = "替换为开放平台请求地址";

            SDKClient client = new SDKClient(accessToken, accessSecret, serverUrl);


            // 利用业务分类配置进行合同发送
            //SendByCategory example = new SendByCategory();
            //example.Process(client);

            SendByCodeSetting example = new SendByCodeSetting();
            example.Process(client);
            Console.WriteLine("合同草稿创建成功");
        }
    }
}
