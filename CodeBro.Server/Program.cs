using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CodeBro.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Permitem CORS pentru ca aplicația client să se poată conecta la server
builder.Services.AddCors();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors(cors => cors
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin());

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<CodeHub>("/codehub");
});

app.Run();
