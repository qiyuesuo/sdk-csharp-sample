using QiyuesuoClient.Http;
using QiyuesuoSDK.Bean;
using QiyuesuoSDK.Bean.Request;
using QiyuesuoSDK.Bean.Response;
using QiyuesuoSDK.Tools;
using System;

namespace sdk_csharp_sample_v2
{
    public class SendByCategory
    {


        public void Process(SDKClient client)
        {
            // 利用业务分类配置创建合同并发起（需配置签署位置，以及合同文件）
            SdkResponse<Contract> contract = this.DraftByCategory(client);
            Console.WriteLine("根据业务分类创建合同草稿成功，并成功发起，合同ID：{0}", contract.Result.Id);
            // 利用业务分类配置进行公章签署（需配置签署位置以及签署印章）
            this.CompanySealSignByCategoryConfig(client, contract.Result.Id);
            Console.WriteLine("根据业务分类指定位置签署公章成功");
            /**
              *  平台方签署完成，签署方签署可采用
              * （1）接收短信的方式登录契约锁云平台进行签署
              * （2）生成内嵌页面签署链接进行签署（下方生成的链接）
              * （3）JS-SDK签署（仅支持个人）
              */
            SdkResponse<PageUrlResult> pageUrl = this.GerenateSignUrl(client, contract.Result.Id);
            Console.WriteLine("生成签署方签署页面成功，签署页面：{0}", pageUrl.Result.PageUrl);
            Console.ReadKey();
        }

        /**
         *  用户通过配置好的业务分类进行合同发起
         *  需在业务分类里配置好将要发起的合同文件或合同模板
         *  配置好整个签署流程，以及签署流程中使用的印章
         * 
         **/
        private SdkResponse<Contract> DraftByCategory(SDKClient client)
        {
            Contract draftContract = new Contract();
            draftContract.Subject = "合同主体";   // 设置合同名称
            draftContract.Category = new Category("2584280095237849689");   //设置将要使用的业务分类配置
            draftContract.AddTemplateParam(new TemplateParam("接收方2", "发起方填参"));   //若业务分类配置了参数模板，进行发起方填参
            draftContract.AddTemplateParam(new TemplateParam("接收方1", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方3", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方4", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方5", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方6", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方7", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方8", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方9", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方10", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方11", "发起方填参2"));
            draftContract.AddTemplateParam(new TemplateParam("接收方12", "发起方填参2"));
            //设置 合同接收方，该设置的合同接收方需要与业务分类配置的接收方流程一致
            Signatory companySignatory = new Signatory("COMPANY", new User("17621699044", "MOBILE"), 1);
            companySignatory.TenantName = "大头橙橙汁公司";
            draftContract.AddSignatory(companySignatory);
            Signatory personalSignatory = new Signatory("PERSONAL", new User("15021504325", "MOBILE"), 2);
            personalSignatory.TenantName = "邓茜茜";
            draftContract.AddSignatory(personalSignatory);
            draftContract.Send = true;  //设置发起默认发起，业务分类中必须要有可用的签署文件

            string response = null;
            try
            {
                response = client.Service(new ContractDraftRequest(draftContract));
            }
            catch (Exception e)
            {
                throw new Exception("创建合同草稿请求服务器失败,错误原因：" + e.Message);
            }
            SdkResponse<Contract> sdkResponse = HttpJsonConvert.DeserializeResponse<Contract>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("创建合同草稿失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        /**
         * 
         * 利用业务分类配置进行公章签署
         * 签署位置需要在业务分类中指定，若没指定则需要传递签署位置
         * 
         * */
        private SdkResponse<Object> CompanySealSignByCategoryConfig(SDKClient client, string contractId)
        {
            SdkResponse<Contract> detail = this.Detail(client, contractId);
            ContractSignCompanyRequest request = new ContractSignCompanyRequest();
            request.ContractId = contractId;

            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("公章签署请求服务器失败，失败原因：" + e.Message);
            }
            SdkResponse<Object> sdkResponse = HttpJsonConvert.DeserializeResponse<Object>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("公章签署失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        private SdkResponse<Object> LpSignByCategoryConfig(SDKClient client, string contractId)
        {
            ContractSignLPRequest request = new ContractSignLPRequest();
            request.ContractId = contractId;

            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("法人章签署请求服务器失败,失败原因：" + e.Message);
            }
            SdkResponse<Object> sdkResponse = HttpJsonConvert.DeserializeResponse<Object>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("法人章签署失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        private SdkResponse<PageUrlResult> GerenateSignUrl(SDKClient client, string contractId)
        {
            ContractPageRequest request = new ContractPageRequest();
            request.ContractId = contractId;
            request.User = new User("15021504325", "MOBILE");
            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("生成签署链接请求服务器失败，失败原因：" + e.Message);
            }
            SdkResponse<PageUrlResult> sdkResponse = HttpJsonConvert.DeserializeResponse<PageUrlResult>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("生成签署链接失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        private SdkResponse<Contract> Detail(SDKClient client, string contractId)
        {
            ContractDetailRequest detailRequest = new ContractDetailRequest()
            {
                ContractId = contractId
            };
            string response = null;
            try
            {
                response = client.Service(detailRequest);
            }
            catch (Exception e)
            {
                throw new Exception("查询合同详情请求服务器失败，错误原因：" + e.Message);
            }
            SdkResponse<Contract> sdkResponse = HttpJsonConvert.DeserializeResponse<Contract>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("查询合同详情失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

    }

}
