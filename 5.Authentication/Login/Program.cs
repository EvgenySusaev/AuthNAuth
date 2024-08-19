using System;
using System.Text;

const string cookiesDir = "./cookies";



Console.WriteLine("Input user name: ");
var userName = Console.ReadLine();

Console.WriteLine("Input password: ");
var password = Console.ReadLine();

var isAuthenticated = Authenticate(userName, password);
var message = isAuthenticated ? $"Login successfull" : "Login failed";
Console.WriteLine(message);

if (isAuthenticated) {
    var cookie = GeneratePlainTextCookie(userName);
    Console.WriteLine($"Plain text cookie: { cookie }");
    var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(cookie));
    Console.WriteLine($"Base 64 cookie: { b64 }");
}




// -----------------------------------------------------------------------------------
//
//
//
// -----------------------------------------------------------------------------------
//
static bool Authenticate(string userName, string password) {
    return (userName == "admin" && password == "admin");
}

static string GeneratePlainTextCookie(string userName) {
    var sb = new StringBuilder();
    sb.Append($"user={userName}");
    sb.Append("; ");
    sb.Append("auth=true");
    sb.Append("; ");
    sb.Append($"expires={ DateTime.Now.AddHours(1).ToString("R") }");

    return sb.ToString();
}


static string LoadCookie(string path) {
    if (File.Exists(path)) {
        return File.ReadAllText(path);
    } else {
        return null;
    }
}
