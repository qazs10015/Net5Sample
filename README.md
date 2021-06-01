# Net5Sample

整理的NetCore5範例，已建立JWT、基本的Model、NSwag
並使用DBFirstr方式開發

建立Model的工具是使用
https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools

大致上的流程
DBFirst

設定appSetting.json的連接字串
建立Models，可使用VisualStudio 的EF Core power tool
至startUp.cs設定dbContext的DI
新增ApiController
swagger測試
--------------------------------------------------------------------------------------------------------------------
(optional)替換成NSwag
dotnet add package NSwag.AspNetCore

	// 注入OpenAPI v3.0 
	// 可以自行設定document的顯示資訊
    services.AddOpenApiDocument();
加入middleware
	// 開啟openAPI功能
	app.UseOpenApi();
	// 開啟openAPI的swaggerUI介面
	app.UseSwaggerUi3();
	// 設定reDoc的路由，預設路徑會和swagger路徑衝突
	app.UseReDoc(config => config.Path = "/redoc");

--------------------------------------------------------------------------------------------------------------------

加入JWT套件
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

新增JwtHelpers.cs並做基本設定
	// inject jwtHelpers
	services.AddSingleton<JwtHelpers>();
	// inject Authentication
	// 可自行使用codeSnippet實作
	services.AddAuthentication....
	
加入驗證的 middleware
	app.UseAuthentication();
建立AccountuController



