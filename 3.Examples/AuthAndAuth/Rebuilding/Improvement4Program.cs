using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Добавим шифрование куки
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();


// Заберём все данныые из куки, используюя отдельный middleware, который объявим до всего остального 
app.Use((context, next) => {
    var dp = context.RequestServices.GetRequiredService<IDataProtectionProvider>(); //.GetDataProtectionProvider();
    var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    
    if (authCookie == null) {
        return next();
    }
    
    var protectedPayload = authCookie.Split("=").Last();


    var protector = dp.CreateProtector("auth-cookie");    
    var payload = protector.Unprotect(protectedPayload);
    var parts = payload.Split(':');
    
    var key = parts[0];
    var value = parts[1];
    
    var claim = new Claim(key, value);
    var claims = new List<Claim>() {
        claim
    }; // one claim of a passport is Id
    var identity = new ClaimsIdentity(claims); // is a way to identify you: goverment gives you a passport
    var principal = new ClaimsPrincipal(identity); // principal is just an object, what carries info about who you are as user 
    //one principal can have many identities: passport, driver licence etc. It is a bag 
    
    
    // вместо set-cookie установка principal'а
    context.User = principal;
    
    return next();
});


app.MapGet("/signin", (AuthService auth ) => {
    auth.SignIn();
    return "Signed in";
});


app.MapGet("/user", (HttpContext context, IDataProtectionProvider dp) => {
    return context.User.FindFirst("user")?.Value;
});

app.Run();


public class AuthService {
    private readonly IDataProtectionProvider _idp;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IDataProtectionProvider dataProtectionProvider, IHttpContextAccessor httpContextAccessor) {
        _idp = dataProtectionProvider;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public void SignIn() {
        var protector = _idp.CreateProtector("auth-cookie");
        
        var protectedPayload =  protector.Protect("user:iam");
        _httpContextAccessor.HttpContext.Response.Headers.SetCookie = $"auth={protectedPayload}";
        
    }
}