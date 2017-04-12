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
            string serviceUrl = "http://openapi.qiyuesuo.net";//开放平台服务地址，测试环境：https://openapi.qiyuesuo.me； 正式环境：https://openapi.qiyuesuo.com
            string accessToken = "upwwMOaavg"; //对接平台标识，在契约锁云平台完成企业实名认证，并成功申请开放平台后获得；
            string accessSeret = "XBy3aEQVwAhrojauLcg7ncRIxl1oJ6";//对接平台密码，在契约锁云平台完成企业实名认证，并成功申请开放平台后获得；
            SDKClient client = new SDKClient(accessToken, accessSeret, serviceUrl);

            //远程签署
            // RemoteSign remoteSign = new RemoteSign(client);
            // remoteSign.Process();

            //标准签署
            StandardSign standardSign = new StandardSign(client);
            standardSign.Process();
        }
    }
}
