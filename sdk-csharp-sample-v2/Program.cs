using QiyuesuoClient.Http;

namespace sdk_csharp_sample_v2
{
    class Program
    {
        static void Main(string[] args)
        {
            string accessSecret = "kAt5C7ibSuoZiM5ixOUP0NbjmSmkvX";
            string accessToken = "v7Xb9KNogN";
            string serverUrl = "https://openapi.qiyuesuo.me";   //测试环境：https://openapi.qiyuesuo.cn ; 正式环境：https://openapi.qiyuesuo.com

            SDKClient client = new SDKClient(accessToken, accessSecret, serverUrl);


            // 利用业务分类配置进行合同发送
            //SendByCategory example = new SendByCategory();
            //example.Process(client);
            
            SendByCodeSetting example = new SendByCodeSetting();
            example.Process(client);
            

        }
    }
}
