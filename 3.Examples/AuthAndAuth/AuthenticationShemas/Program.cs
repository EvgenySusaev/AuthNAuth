using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Step 1: Configure Authentication
builder.Services.AddAuthentication()
    .AddScheme<CookieAuthenticationOptions, VisitorAuthenticationHandler>(
        "visitor", // Scheme name for visitor authentication
        options => { } // Configuration options (none in this example)
    )
    .AddCookie("local"); // Scheme name for local cookie-based authentication

// Step 2: Configure Authorization
builder.Services.AddAuthorization(options => {
    options.AddPolicy("customer", policy => {
        policy.AddAuthenticationSchemes("local", "visitor")
            .RequireAuthenticatedUser(); // Requires the user to be authenticated
    });
});

var app = builder.Build();

// Step 3: Use Authentication and Authorization Middleware
app.UseAuthentication(); // Adds authentication middleware to the request pipeline
app.UseAuthorization(); // Adds authorization middleware to the request pipeline

// Step 4: Define Endpoints

// Protected endpoint that requires 'customer' policy
app.MapGet("/", async () => "Hello World!")
    .RequireAuthorization("customer");

// Login endpoint to sign in the user
app.MapGet("/login", async (context) => {
    // Create user claims and identity for 'local' authentication
    var claims = new List<Claim> { new Claim("user", "Alice") };
    var identity = new ClaimsIdentity(claims, "local");
    var userPrincipal = new ClaimsPrincipal(identity); // who you are as a user

    // Sign in the user with the 'local' authentication scheme
    await context.SignInAsync("local", userPrincipal);
});

app.Run();

// Custom Authentication Handler for 'visitor' scheme
public class VisitorAuthenticationHandler : CookieAuthenticationHandler {
    public VisitorAuthenticationHandler(
        IOptionsMonitor<CookieAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        // Attempt to authenticate using the base cookie handler
        var result = await base.HandleAuthenticateAsync();

        if (result.Succeeded) {
            // Return success if authentication succeeds
            return result;
        }

        // If authentication fails, create a default visitor identity
        var claims = new List<Claim> { new Claim("user", "Visitor") };
        var identity = new ClaimsIdentity(claims, "visitor");
        var userPrincipal = new ClaimsPrincipal(identity);

        // Sign in the user as a visitor with the 'visitor' scheme
        await Context.SignInAsync("visitor", userPrincipal);

        // Return success with the visitor authentication ticket
        return AuthenticateResult.Success(new AuthenticationTicket(userPrincipal, "visitor"));
    }
}