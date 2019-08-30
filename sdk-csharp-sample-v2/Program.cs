using QiyuesuoClient.Http;

namespace sdk_csharp_sample_v2
{
    class Program
    {
        static void Main(string[] args)
        {
            string accessSecret = "替换为开放平台SECRET";
            string accessToken = "替换为开放平台TOKAN";
            string serverUrl = "https://openapi.qiyuesuo.cn";   //测试环境：https://openapi.qiyuesuo.cn ; 正式环境：https://openapi.qiyuesuo.com

            SDKClient client = new SDKClient(accessToken, accessSecret, serverUrl);


            // 利用业务分类配置进行合同发送
            //SendByCategory example = new SendByCategory();
            //example.Process(client);

            SendByCodeSetting example = new SendByCodeSetting();
            example.Process(client);


        }
    }
}
