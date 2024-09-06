using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Добавим шифрование куки
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();



app.MapGet("/signin", (AuthService auth ) => {
    auth.SignIn();
    return "Signed in";
});


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