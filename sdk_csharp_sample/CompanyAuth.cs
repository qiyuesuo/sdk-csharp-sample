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
    public class CompanyAuthClass
    {
        private ICompanyAuthService companyAuthService;

        public CompanyAuthClass(SDKClient client)
        {
            companyAuthService = new CompanyAuthServiceImpl(client);
        }

        public void Process()
        {
            try
            {
                //企业基本认证
                BasicUrl();

                //企业加强认证
                FullUrl();

                //查询认证详情
                GetAuthDetail();

                //基本认证页面地址
                ApplyBasicAuth();

                //加强认证页面地址
                ApplyFullAuth();

                //下载认证文件
                DownloadAuthFile();

                //认证信息查看页面地址
                viewUrl();
            }
            catch (Exception e)
            {
                Console.WriteLine("标准签署模式异常，错误信息{0}", e.Message);
            }
            Console.ReadKey();//防止闪退 
        }

        public long ApplyBasicAuth()
        {
            CompanyAuth company = new CompanyAuth();
            company.name = "基本认证公司名称";
            company.registerNo = "123123789132";
            Stream operAuthorization = new FileStream("auth.jpg", FileMode.Open);
            Stream license = new FileStream("license.jpg", FileMode.Open);
            company.license = license;
            company.operAuthorization = operAuthorization;
            company.legalPerson = "法人名称";
            company.contactPhone = "1660000000";
            long authId=companyAuthService.ApplyBasic(company);
            Console.WriteLine("企业基本认证提交完成，认证Id：{0]", authId);
            operAuthorization.Close();
            license.Close();
            return authId;
        }

        public long ApplyFullAuth()
        {
            CompanyAuth company = new CompanyAuth();
            company.name = "基本认证公司名称";
            company.registerNo = "123123789132";
            Stream operAuthorization = new FileStream("auth.jpg", FileMode.Open);
            Stream license = new FileStream("license.jpg", FileMode.Open);
            company.license = license;
            company.operAuthorization = operAuthorization;
            company.legalPerson = "法人名称";
            company.contactPhone = "1660000000";
            long authId = companyAuthService.ApplyFull(company);
            Console.WriteLine("企业加强认证提交完成，认证Id：{0]", authId);
            operAuthorization.Close();
            license.Close();
            return authId;
        }

        public void GetAuthDetail()
        {
            CompanyAuthDetailResponse response = companyAuthService.Detail("基本认证公司名称");
            Console.WriteLine("获取认证结果成功，认证状态为{0]", response.status);
        }

        public void DownloadAuthFile()
        {
            Stream downloadFile = new FileStream("Download.png", FileMode.CreateNew);
            long authId = 123412345;
            companyAuthService.DownloadFile(authId, AuthFileType.LICENSE, ref downloadFile);
            downloadFile.Close();
            Console.WriteLine("下载文件成功");
        }

        public void BasicUrl()
        {
            AuthUrlResponse response = companyAuthService.BasicUrl("基本认证公司", "123123", "", "");
            Console.WriteLine("申请基本认证链接成功，链接:{0}，Token:{1}", response.url, response.token);
        }


        public void FullUrl()
        {
            AuthUrlResponse response = companyAuthService.FullUrl("基本认证公司", "123123", "", "");
            Console.WriteLine("申请加强认证链接成功，链接:{0}，Token:{1}", response.url, response.token);
        }

        public void viewUrl()
        {
            AuthUrlResponse response = companyAuthService.ViewUrl("基本认证公司");
            Console.WriteLine("申请查看认证信息的页面地址成功，链接:{0}，Token:{1}", response.url, response.token);
        }
    }
}
