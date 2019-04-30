using QiyuesuoSDK.Api;
using QiyuesuoSDK.Bean;
using QiyuesuoSDK.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/**
契约锁电子合同开放平台——远程签署模式
**/
namespace sdk_csharp_sample
{
    public class RemoteSign
    {
        private ISealService sealService;
        private IRemoteSignService signService;

        public RemoteSign(SDKClient client) {
            signService = new RemoteSignServiceImpl(client);
            sealService = new SealServiceImpl(client);
        }

        public void Process() {
            try
            {
                //根据模板创建合同
             //   string documentId = CreateByTemplate();

                //根据html创建合同
             //   documentId = CreateByHtml();

                //根据本地PDF文件创建合同
                string documentId = CreateByFile();

                //对接平台方签署
                SignByPlatform(documentId);

                //平台个人用户静默签署
             //   SignByPerson(documentId);

                //平台企业用户静默签署
             //   SignByCompany(documentId);

                //平台个人用户签署界面
            //    GetPersonSignUrl(documentId);

                //平台企业用户签署界面
            //    GetCompanySignUrl(documentId);

                //封存合同
           //     Complete(documentId);

                //合同查看界面
                GetViewUrl(documentId);

                //查询合同详情
           //     Detail(documentId);

                //下载合同
          //      Download(documentId);
            }
            catch (Exception e)
            {
                Console.WriteLine("远程签署失败，错误原因：{0}", e.Message);
            }

            Console.ReadKey();//防止闪退 
        }

        private string CreateByFile() {
            FileStream fileInput = new FileStream("D://023000874//融资服务协议.pdf", FileMode.Open);
            string documentid = signService.Create(fileInput, "远程签授权协议书");//有效期默认为30天，到期未完成签署，自动作废
           // documentid = signService.Create(fileInput, "远程签授权协议书", DateTime.Now.AddDays(7));//7天后未完成签署，自动作废
            fileInput.Close();
            Console.WriteLine("根据文件创建合同成功，文档ID:{0}", documentid);
            return documentid;
        }

        private string CreateByTemplate() {
            string documentid = "";
            TemplateInfo template = new TemplateInfo("2287912848399175726");
            template.AddParameter("param1", "参数1");
            template.AddParameter("param2", "参数2");
            template.AddParameter("param3", "参数3");

            documentid = signService.Create(template, "远程模板测试合同");//有效期默认为30天，到期未完成签署，自动作废
            // documentid = signService.Create(template, "远程模板测试合同", DateTime.Now.AddDays(7));//7天后未完成签署，自动作废
            Console.WriteLine("根据模板创建合同成功，文档ID:{0}", documentid);
            return documentid;
        }

        private string CreateByHtml() {
            string documentid = "";
            //根据html创建合同,不带有效时间
            string html = "<html><body><p>title</p><p>在线第三方电子合同平台。企业及个人用户可通过本平台与签约方快速完成合同签署，安全、合法、有效。</p></body></html>";
            //documentid = signService.Create(html, "测试html创建合同");

            //根据html创建合同,带有效时间
            documentid = signService.Create(html, "测试html创建合同", DateTime.Now.AddDays(7));
            Console.WriteLine("根据html创建合同成功，文档ID:{0}", documentid);
        
            return documentid;
        }

        private void SignByPlatform(string documentId) {
            // 平台签署,带签名外观
            string sealId = "2292201504040435826";// 平台印章，在契约锁云平台维护
            Stamper stamper = new Stamper(1, 0.5f, 0.5f);// 签名位置，根据坐标比确定位置
            //Stamper stamper = new Stamper("公章：", 0, 0);// 签名位置，根据关键字定位
            //remoteSignService.Sign(documentId);//无签名外观
            signService.Sign(documentId, sealId, stamper);
            Console.WriteLine("平台签署成功！");
        }

        private void SignByPerson(string documentId) {
            // 个人用户签署
            Person person = new Person("丁五");
            person.idcard = "311312193706205418";
            person.mobile = "13266668888";
            person.email = "wu.ding@qiyuesuo.com";
            person.gender = Gender.MALE;
            person.paperType = PaperType.IDCARD;

            // 个人无签名外观时调用
            // remoteSignService.Sign(documentId, person);
            string sealData = sealService.GenerateSeal(person);// 生成个人签名数据
           // Stamper personStamper = new Stamper(1, 0.2f, 0.2f);// 签名显示位置，根据坐标比定位
            Stamper personStamper = new Stamper("运营者签字：", 0, 0);// 签名位置，根据关键字定位
            // 个人签署接口，有签名外观
            signService.Sign(documentId, person, sealData, personStamper);
            Console.WriteLine("个人签署完成！");

        }

        private void SignByCompany(string documentId) {
            // 公司用户签署
            Company company = new Company("测试科技有限公司");
            company.registerNo = "12323432452";
            // 公司无签名外观时调用
            // remoteSignService.Sign(documentId, company);
            Stamper companyStamper = new Stamper(1, 0.3f, 0.3f);// 签名显示位置，根据坐标比定位
            //Stamper companyStamper = new Stamper("盖公章：", 0.1f, 0);// 签名位置，根据关键字定位
            string companySealData = sealService.GenerateSeal(company);
            signService.Sign(documentId, company, companySealData, companyStamper);
            Console.WriteLine("公司签署完成！");
        }

        private void GetPersonSignUrl(string documentId) {
            Person person = new Person("丁五");
            person.idcard = "311312193706205418";
            person.mobile = "13266668888";//SignType.SIGNWITHPIN时必填
            person.email = "wu.ding@qiyuesuo.com";
            person.gender = Gender.MALE;
            person.paperType = PaperType.IDCARD;

            //个人用户签署页面之不可见签名 
            string personSignUnvisibleUrl = signService.SignUrl(documentId, SignType.SIGNWITHPIN, person, "https://www.baidu.com/",null);
            Console.WriteLine("个人用户签署页面之可见签名 url：{0}", personSignUnvisibleUrl);
            //个人用户签署页面之可见签名
            //生成个人印章数据，用户可自定义签名图片
            string personSealData = sealService.GenerateSeal(person);// 生成个人印章数据，用户可自定义签名图片
            Stamper personSignUrlStamper = new Stamper(1, 0.2f, 0.2f);

            List<Stamper> stampers = new List<Stamper>();
            stampers.Add(personSignUrlStamper);
            string personSignVisibleUrl = signService.SignUrl(documentId, SignType.SIGNWITHPIN, person, personSealData, stampers, "https://www.baidu.com/",null);
            Console.WriteLine("个人用户签署页面之可见签名 url：{0}", personSignVisibleUrl);
        }

        private void GetCompanySignUrl(string documentId) {
            // 企业用户签署页面URL
            Company companySigner = new Company("哈治理测试科技有限公司");
            companySigner.registerNo="12323432452";
            companySigner.mobile="18701559988";//SignType.SIGNWITHPIN时必填
            //企业用户签署页面之不可见签名 
            string companySignUnvisibleUrl = signService.SignUrl(documentId, SignType.SIGNWITHPIN, companySigner, "https://www.baidu.com/",null);
            Console.WriteLine("企业用户签署页面之不可见签名url：{0}", companySignUnvisibleUrl);
            //企业用户签署页面之可见签名 
            // 生成企业印章数据，用户可自定义印章图片
            string companySealDate = sealService.GenerateSeal(companySigner);
            Stamper companySignUrlStamper = new Stamper(1, 0.1f, 0.5f);

            List<Stamper> stampers = new List<Stamper>();
            stampers.Add(companySignUrlStamper);

            string companySignVisibleUrl = signService.SignUrl(documentId, SignType.SIGNWITHPIN, companySigner, companySealDate, stampers, "https://www.baidu.com/",null);
            Console.WriteLine("企业用户签署页面之可见签名url：{0}", companySignVisibleUrl);
        }

        private void GetViewUrl(string documentId) {
            string viewUrl = signService.ViewUrl(documentId);
            Console.WriteLine("浏览合同URL：{0}", viewUrl);
        }


        private void MutiSignByPlatform(string documentId)
        {
            PlatformMutiSignRequest mutiSignRequest = new PlatformMutiSignRequest();
            List<RemoteStamper> remoteStampers = new List<RemoteStamper>();

            RemoteStamper stamper1 = new RemoteStamper();
            stamper1.documentId = 2560294199182234262;
            stamper1.sealType = RemoteSealType.SEAL;
            stamper1.sealId = 2553205357123998650;
            stamper1.offsetX = 0.1;
            stamper1.offsetY = 0.2;
            stamper1.page = 1;
            remoteStampers.Add(stamper1);

            RemoteStamper stamper2 = new RemoteStamper();
            stamper2.documentId = 2560294199182234262;
            stamper2.sealType = RemoteSealType.TIMESTAMP;
            stamper2.offsetX = 0.5;
            stamper2.offsetY = 0.6;
            stamper2.page = 1;
            remoteStampers.Add(stamper2);

            RemoteStamper stamper3 = new RemoteStamper();
            stamper3.documentId = 2560294199182234262;
            stamper3.sealType = RemoteSealType.ACROSS_PAGE;
            stamper3.sealId = 2553205357123998650;
            stamper3.offsetY = 0.2;
            remoteStampers.Add(stamper3);

            mutiSignRequest.stampers = remoteStampers;
            signService.MutiSign(mutiSignRequest);
        }


        private void MutiSignByCompany(string documentId)
        {
            Company signCompany = new Company("签署公司名称");
            CompanyMutiSignRequest mutiSignRequest = new CompanyMutiSignRequest(signCompany);

            List<RemoteStamper> remoteStampers = new List<RemoteStamper>();

            RemoteStamper stamper1 = new RemoteStamper();
            stamper1.documentId = 2560294199182234262;
            stamper1.sealType = RemoteSealType.SEAL;
            stamper1.sealImageBase64 = "印章Base64图片编码";
            stamper1.offsetX = 0.1;
            stamper1.offsetY = 0.2;
            stamper1.page = 1;
            remoteStampers.Add(stamper1);

            RemoteStamper stamper2 = new RemoteStamper();
            stamper2.documentId = 2560294199182234262;
            stamper2.sealType = RemoteSealType.TIMESTAMP;
            stamper2.offsetX = 0.5;
            stamper2.offsetY = 0.6;
            stamper2.page = 1;
            remoteStampers.Add(stamper2);

            RemoteStamper stamper3 = new RemoteStamper();
            stamper3.documentId = 2560294199182234262;
            stamper3.sealType = RemoteSealType.ACROSS_PAGE;
            stamper1.sealImageBase64 = "印章Base64图片编码";
            stamper3.offsetY = 0.2;
            remoteStampers.Add(stamper3);

            mutiSignRequest.stampers = remoteStampers;
            signService.MutiSign(mutiSignRequest);

        }


        private void MutiSignByPerson(string documentId)
        {
            Person signPerson= new Person("签署人名称");
            PersonMutiSignRequest mutiSignRequest = new PersonMutiSignRequest(signPerson);

            List<RemoteStamper> remoteStampers = new List<RemoteStamper>();

            RemoteStamper stamper1 = new RemoteStamper();
            stamper1.documentId = 2560294199182234262;
            stamper1.sealType = RemoteSealType.SEAL;
            stamper1.sealImageBase64 = "印章Base64图片编码";
            stamper1.offsetX = 0.1;
            stamper1.offsetY = 0.2;
            stamper1.page = 1;
            remoteStampers.Add(stamper1);

            RemoteStamper stamper2 = new RemoteStamper();
            stamper2.documentId = 2560294199182234262;
            stamper2.sealType = RemoteSealType.TIMESTAMP;
            stamper2.offsetX = 0.5;
            stamper2.offsetY = 0.6;
            stamper2.page = 1;
            remoteStampers.Add(stamper2);

            mutiSignRequest.stampers = remoteStampers;
            signService.MutiSign(mutiSignRequest);
        }

        private void Complete(string documentId) {
            // 签署完成
            signService.Complete(documentId);
            Console.WriteLine("签署完成。");
        }

        private void Detail(string docuemntId) {
            Contract result = signService.Detail(docuemntId);
            Console.WriteLine("合同状态：{0}", result.status);
        }

        private void Download(string documentId) {

            Stream stream = new FileStream("D://Download.pdf", FileMode.Create, FileAccess.Write);
            signService.DownloadDoc(documentId,ref stream);
            stream.Close();
            Console.WriteLine("合同文件下载成功！");
        }
    }
}
