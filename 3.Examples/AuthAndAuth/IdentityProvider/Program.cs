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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization();

var users = new ConcurrentDictionary<string, string>();
builder.Services.AddSingleton(users);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", async context =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        await context.Response.WriteAsync($"Hello, {context.User.Identity.Name}!");
    }
    else
    {
        await context.Response.WriteAsync("Hello, guest!");
    }
});

app.MapGet("/Account/Login", async context =>
{
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

app.MapPost("/Account/Login", async context =>
{
    var form = context.Request.Form;
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    if (users.TryGetValue(username, out var storedPassword) && storedPassword == password)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);

        context.Response.Redirect("/");
    }
    else
    {
        await context.Response.WriteAsync("Invalid credentials");
    }
});

app.MapGet("/Account/Register", async context =>
{
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

app.MapPost("/Account/Register", async context =>
{
    var form = context.Request.Form;
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    if (users.TryAdd(username, password))
    {
        await context.Response.WriteAsync("User registered successfully");
    }
    else
    {
        await context.Response.WriteAsync("User already exists");
    }
});

app.MapGet("/Account/Logout", async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/");
});

app.Run();
