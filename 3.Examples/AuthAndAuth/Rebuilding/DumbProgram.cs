// var builder = WebApplication.CreateBuilder(args);
//
// var app = builder.Build();
//
//
// // 1. просим браузер устанавливать куки. set cookie, please
// app.MapGet("/login", (HttpContext context ) => {
//
// #region SignIn
//     context.Response.Headers.SetCookie = "auth=user:iam";
//     return "Logged in";
// #endregion
//
// });
//
// // 2. Теперь браузер по нашей просьбе устанавливает куки, которые мы может читать. Он будет это делать в каждом запросе
// app.MapGet("/user", (HttpContext context ) => {
//     var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
//     var key = authCookie.Split('=')[0];
//     var value = authCookie.Split('=')[1];
//     
//     return value;
// });
//
// app.Run();