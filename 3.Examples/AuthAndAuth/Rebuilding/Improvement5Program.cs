using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Весь предыдущий функционал из AuthService заменяет
builder.Services
    .AddAuthentication("cookie") // scheme - who identifies you? government identifies me? policeman? job center? 
    .AddCookie("cookie"); // cookie responsible for loading up and validating, checking format. All stuff from middleware.


var app = builder.Build();


// Заберём все данныые из куки, используюя отдельный middleware, который объявим до всего остального 
app.UseAuthentication();


app.MapGet("/signin", (HttpContext context) => {
    // auth.SignIn();
    var claim = new Claim("user", "iam");
    var claims = new List<Claim>() {
        claim
    }; 
    var identity = new ClaimsIdentity(claims);
    var principal =
        new ClaimsPrincipal(identity); 
    
    context.SignInAsync("cookie", principal);
    return "Signed in";
});


app.MapGet("/user",
    (HttpContext context, IDataProtectionProvider dp) => { return context.User.FindFirst("user")?.Value; });

app.Run();