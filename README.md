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

設定document的顯示資訊

	 services.AddSwaggerDocument(document =>
            {
                document.PostProcess = d =>
                {
                    d.Info.Title = "This is title";
                    d.Info.Contact = new OpenApiContact()
                    {
                        Name = "Junyu Wang",
                        Url = "https://github.com/qazs10015"
                    };
                    d.Info.Description = "This is my sample swagger";
                    d.Info.Version = "This is version";
                };
            });
	    
設定OpenAPI的scheme

	// 注入OpenAPI v3.0
	services.AddOpenApiDocument(config =>
	{
	// 這個 OpenApiSecurityScheme 物件請勿加上 Name 與 In 屬性，否則產生出來的 OpenAPI Spec 格式會有錯誤！
	var apiScheme = new OpenApiSecurityScheme()
	{
	    Type = OpenApiSecuritySchemeType.Http,
	    Scheme = JwtBearerDefaults.AuthenticationScheme,
	    Description = "Copy JWT Token into the value field: {token}"
	};

	// 這裡會同時註冊 SecurityDefinition (.components.securitySchemes) 與 SecurityRequirement (.security)
	config.AddSecurity("Bearer", apiScheme);

	// 這段是為了將 "Bearer" 加入到 OpenAPI Spec 裡 Operator 的 security (Security requirements) 中
	config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
	});

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

新增JwtHelpers.cs並做基本設定(可以自行設定預設過期時間、角色資料等)

於StartUp.cs注入JwtHelpers.cs並開啟驗證

	// inject jwtHelpers
	services.AddSingleton<JwtHelpers>();
	// inject Authentication
	// 可自行使用codeSnippet實作
	 services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true; // Default: true

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Let "sub" assign to User.Identity.Name
                        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        // Let "roles" assign to Roles for [Authorized] attributes
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                        // Validate the Issuer
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),

                        ValidateAudience = false,
                        //ValidAudience = "JwtAuthDemo", // TODO

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = false,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSettings:SignKey")))
                    };
                });
	
加入驗證的 middleware

	app.UseAuthentication();
	
建立AccountuController
加入注入的JwtHelpers服務，並驗證登入資料，驗證成功後產生token

	[HttpPost("")]
        public ActionResult<LoginResult> Login(Login model)
        {
            if (model.userName == "junyu")
            {
                // 如果需要設定角色權限可以自行設定role
                // 範例為Admin
                return new LoginResult() { Token = jwt.GenerateToken(model.userName, "Admin") };
            }
            else
            {
                return BadRequest();
            }
        }

	
