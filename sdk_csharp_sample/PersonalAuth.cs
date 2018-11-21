using QiyuesuoSDK.Api;
using QiyuesuoSDK.Auth;
using QiyuesuoSDK.Bean;
using QiyuesuoSDK.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace sdk_csharp_sample
{
    public class PersonalAuthClass
    {
        private IPersonalAuthService personalAuthService;

        public PersonalAuthClass(SDKClient client)
        {
            personalAuthService = new PersonalAuthServiceImpl(client);
        }

        public void Process()
        {
            try
            {
                //获取认证链接
                GetAuthUrl();

                //获取认证结果
                VerifyAuth();

            }
            catch (Exception e)
            {
                Console.WriteLine("标准签署模式异常，错误信息{0}", e.Message);
            }
            Console.ReadKey();//防止闪退 
        }

        public String GetAuthUrl()
        {
            PersonalAuth personalAuth = new PersonalAuth();
            personalAuth.username = "张大壮";
            personalAuth.idCardNo = "510108199002181211";
            personalAuth.bizId = "33333";
            String url=personalAuthService.GetAuthUrl(personalAuth);
            Console.WriteLine("获取实名认证地址成功，地址：{0}",url);
            return url;
        }

        public PersonalAuthResponse VerifyAuth()
        {
            String bizId = "33333";
            PersonalAuthResponse response=personalAuthService.GetDetail(bizId);
            Console.WriteLine("获取业务号:{0}认证信息成功，认证结果为：{1}",bizId,response.status);
            return response;
        }
    }
}
