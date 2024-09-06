using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// --------
// --------
// --------

builder.Services.Configure<IdentityOptions>(options => {
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options => {
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.Configure<IdentityOptions>(options => {
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 6;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 3;

    options.SignIn.RequireConfirmedEmail = true;

    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.Cookie.Expiration = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
});

// --------
// --------
// --------

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";

        options.Events = new CookieAuthenticationEvents {
            OnValidatePrincipal = context => {
                var claims = new List<Claim>();

                // Transform 'email' to 'user_email'
                var email = context.Principal.FindFirst("email")?.Value;
                if (email != null) {
                    claims.Add(new Claim("user_email", email));
                }

                // Combine 'given_name' and 'family_name' into 'full_name'
                var givenName = context.Principal.FindFirst("given_name")?.Value;
                var familyName = context.Principal.FindFirst("family_name")?.Value;
                if (givenName != null && familyName != null) {
                    claims.Add(new Claim("full_name", $"{givenName} {familyName}"));
                }

                // Transform 'groups' into application-specific 'roles'
                var groups = context.Principal.FindAll("groups");
                foreach (var group in groups) {
                    if (group.Value == "d84a6b59-1bce-4a13-9f35-5f9a3b1e4f91") // Example Group ID
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    // Add more group-to-role mappings as needed
                }

                var appIdentity = new ClaimsIdentity(claims);
                context.Principal.AddIdentity(appIdentity);

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

var users = new ConcurrentDictionary<string, string>();
builder.Services.AddSingleton(users);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", async context => {
    if (context.User.Identity?.IsAuthenticated == true) {
        await context.Response.WriteAsync($"Hello, {context.User.Identity.Name}!");
    }
    else {
        await context.Response.WriteAsync("Hello, guest!");
    }
});

app.MapGet("/Account/Login", async context => {
    context.Response.ContentType = "text/html";
    var response = @"
    <html>
    <body>
        <form method='post' action='/Account/Login'>
            <input type='text' name='username' placeholder='Username' required />
            <input type='password' name='password' placeholder='Password' required />
            <button type='submit'>Login</button>
        </form>
    </body>
    </html>";
    await context.Response.WriteAsync(response);
});

app.MapPost("/Account/Login", async context => {
    var form = context.Request.Form;
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    if (users.TryGetValue(username, out var storedPassword) && storedPassword == password) {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, "user@example.com"),
            new Claim(ClaimTypes.Role, "Admin") // Assigning an Admin role
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);


        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);

        context.Response.Redirect("/");
    }
    else {
        await context.Response.WriteAsync("Invalid credentials");
    }
});

app.MapGet("/Account/Register", async context => {
    context.Response.ContentType = "text/html";
    var response = @"
    <html>
    <body>
        <form method='post' action='/Account/Register'>
            <input type='text' name='username' placeholder='Username' required />
            <input type='password' name='password' placeholder='Password' required />
            <button type='submit'>Register</button>
        </form>
    </body>
    </html>";
    await context.Response.WriteAsync(response);
});

app.MapPost("/Account/Register", async context => {
    var form = context.Request.Form;
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    if (users.TryAdd(username, password)) {
        await context.Response.WriteAsync("User registered successfully");
    }
    else {
        await context.Response.WriteAsync("User already exists");
    }
});

app.MapGet("/Account/Logout", async context => {
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/");
});

app.Run();


public class MyClaimsTransformation : IClaimsTransformation {
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal) {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();
        var claimType = "myNewClaim";

        if (!principal.HasClaim(claim => claim.Type == claimType)) {
            claimsIdentity.AddClaim(new Claim(claimType, "myClaimValue"));
        }

        principal.AddIdentity(claimsIdentity);

        return Task.FromResult(principal);
    }
}