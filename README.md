# 契约锁.NET SDK示例

[契约锁](http://www.qiyuesuo.com) 成立于2015年，是新一代数字签名服务领域的领军企业。依托雄厚的企业管理软件服务经验，致力为全国的企业及个人用户提供最具可用性、稳定性及前瞻性的电子签署、数据存证及数据交互服务。 契约锁深耕企业级市场，产品线涵盖电子签署、合同管理、公文签发、数据存证等企业内签署场景，并提供本地签、远程签、标准签等多种API调用方式接入企业内部管理系统。目前主要为教育、旅游、互联网金融、政府事业单位、集团企业、B2B电商、地产中介、O2O等企业及个人提供签署、存证服务。 契约锁严格遵守《中华人民共和国电子签名法》，并联合公安部公民网络身份识别系统（eID）、工商相关身份识别系统、权威CA机构、公证处及律师事务所，确保在契约锁上签署的每一份合同真实且具有法律效力。 契约锁平台由上海亘岩网络科技有限公司运营开发，核心团队具有丰富的企业管理软件、金融支付行业、数字证书行业从业经验，致力于通过技术手段让企业合同签署及管理业务更简单、更便捷。

了解更多契约锁详情请访问 [www.qiyuesuo.com](http://www.qiyuesuo.com).


Requirements
============
.NET Framework 3.5 or later.  

Installation
============

前往 [契约锁开放平台](http://open.qiyuesuo.com/download)下载.NET SDK及依赖包，并添加到项目中。

### Visual Studio users
打开Visual Studio编译器，在Solution Explorer标签栏里，右击“Reference”->“Add Reference”->“Browse”->选择下载.NET SDK包中的QiyuesuoSDK.DLL。

Usage
=====

#### 远程签
将文件上传的开放平台进行签署，或者使用开放平台的模板进行签署。

详情请参考： [RemoteSign.cs](https://github.com/qiyuesuo/sdk-csharp-sample/blob/master/sdk_csharp_sample/RemoteSign.cs)

#### 标准签
调用方通过接口发起或者签署合同，接收方通过短信、APP或者云平台登录契约锁完成签署。

详情请参考： [StandardSign.cs](https://github.com/qiyuesuo/sdk-csharp-sample/blob/master/sdk_csharp_sample/StandardSign.cs)

#### 本地签
通过SDK提供该种签署服务，对接方将合同文件数据传入SDK，SDK对文件进行摘要运算，将摘要值发送到契约锁平台上进行签署。由于摘要运算是不可逆的，契约锁平台无法通过摘要值来获取合同的原文，从而实现了对接方合同隐私的保全。

详情请参考： [LocalSign.cs](https://github.com/qiyuesuo/sdk-csharp-sample/blob/master/sdk_csharp_sample/LocalSign.cs)

Notes
=======
示例代码中的参数均为测试环境参数，实际运行时需要将相关参数修改为生产环境参数，或沙箱测试环境参数。

扫码关注契约锁公众号,了解契约锁最新动态。

![契约锁公众号](qrcode.png)