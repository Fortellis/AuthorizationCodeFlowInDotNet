using Newtonsoft.Json;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapPost("/token", async (HttpRequest request) => {
    string body = String.Empty;
    using (StreamReader reader = new StreamReader(request.Body))
    {
        body = await reader.ReadToEndAsync();
    }
    Console.WriteLine("This is the body of the request: " + body);
    var url = new Uri("https://identity.fortellis.io/oauth2/aus1p1ixy7YL8cMq02p7/v1/token");
    var handler = new HttpClientHandler();
    using var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", "Basic {yourAPIKey:APISecret}");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    HttpContent data = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        new KeyValuePair<string, string>("redirect_uri", "http://localhost:{yourPort}/"),
        new KeyValuePair<string, string>("code", body)
    });

    Console.WriteLine($"This is what's in the data: {data.ToString}");

    var res = await client.PostAsync(url, data);

    var content = await res.Content.ReadAsStringAsync();

    Console.WriteLine("This is the content of the response: " + content);

    var tokenResponse = JsonConvert.DeserializeObject(content);
    Console.WriteLine("This is the token response parsed: " + tokenResponse);
    return content;
});
app.Run();
