using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Добавим шифрование куки
builder.Services.AddDataProtection();

var app = builder.Build();



app.MapGet("/login", (HttpContext context, IDataProtectionProvider dp) => {
    var protector = dp.CreateProtector("auth-cookie");
    var protectedPayload = protector.Protect("user:iam");
    
// 1. просим браузер устанавливать куки. set cookie, please
#region SignIn
    context.Response.Headers.SetCookie = $"auth={protectedPayload}";
#endregion

    return "Logged in";
});

// 2. Теперь браузер по нашей просьбе устанавливает куки в каждый запрос, которые мы может читать.

app.MapGet("/user", (HttpContext context, IDataProtectionProvider dp) => {
    var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var protectedPayload = authCookie.Split("=").Last();


    var protector = dp.CreateProtector("auth-cookie");    
    var payload = protector.Unprotect(protectedPayload);
    var parts = payload.Split(':');
    
    var key = parts[0];
    var value = parts[1];
    
    return value;
});

app.Run();