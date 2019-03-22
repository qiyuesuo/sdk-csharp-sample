using QiyuesuoSDK.Bean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sdk_csharp_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceUrl = "https://openapi.qiyuesuo.cn";//开放平台服务地址，测试环境：https://openapi.qiyuesuo.cn； 正式环境：https://openapi.qiyuesuo.com
            string accessToken = "qAJ6biIGwp"; //对接平台标识，在契约锁云平台完成企业实名认证，并成功申请开放平台后获得；
            string accessSeret = "iMFLfa2785eVP3IWcW6VGErhQqyZ7pgsI";//对接平台密码，在契约锁云平台完成企业实名认证，并成功申请开放平台后获得；
            SDKClient client = new SDKClient(accessToken, accessSeret, serviceUrl);

            //远程签署
            //RemoteSign remoteSign = new RemoteSign(client);
             //remoteSign.Process();

            //标准签署
            //StandardSign standardSign = new StandardSign(client);
            //standardSign.Process();


            //本地签署
            //LocalSign localSign = new LocalSign(client);
            //localSign.Process();

            //个人认证
            //PersonalAuthClass personalAuth = new PersonalAuthClass(client);
            //personalAuth.Process();

            //企业认证
            //CompanyAuthClass companyAuth = new CompanyAuthClass(client);
            //companyAuth.Process();

            //文件存证
            EVS_SDK.Bean.SDKClient evsClient = new EVS_SDK.Bean.SDKClient(accessToken, accessSeret, serviceUrl);
            Storage storage = new Storage(evsClient);
            storage.Process();

        }
    }
}
