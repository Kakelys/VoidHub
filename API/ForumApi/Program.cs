using System.Text.Json.Serialization;
using ForumApi.Data.Repository;
using ForumApi.Utils.Extensions;
using ForumApi.Utils.Middlewares;
using ForumApi.Options;
using ForumApi.Hubs;
using ForumApi.Utils.Background;

//need to be checked before create builder
if (!Directory.Exists("wwwroot"))
{
  Directory.CreateDirectory("wwwroot");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddAppOptions(builder.Configuration);

var imageSettings = builder.Configuration
  .GetSection(ImageOptions.Image)
  .Get<ImageOptions>() ?? throw new ArgumentNullException("ImageOptions");

//check for folders
if(!Directory.Exists($"{imageSettings.Folder}/{imageSettings.AvatarFolder}"))
{
  Directory.CreateDirectory($"{imageSettings.Folder}/{imageSettings.AvatarFolder}");
}

if(!Directory.Exists($"{imageSettings.Folder}/{imageSettings.PostImageFolder}"))
{
  Directory.CreateDirectory($"{imageSettings.Folder}/{imageSettings.PostImageFolder}");
}

if(!Directory.Exists($"{imageSettings.Folder}/{imageSettings.ForumFolder}"))
{
  Directory.CreateDirectory($"{imageSettings.Folder}/{imageSettings.ForumFolder}");
}

builder.Services.AddRepository(builder.Configuration);
builder.Services.AddAppServices();

builder.Services.AddControllers()
  .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSwagger();

builder.Services.ConfigureAutoMapper();
builder.Services.AddValidators();
builder.Services.AddJwtAuth(builder.Configuration);

builder.Services.AddSignalR();

var frontCorsPolicy = "frontCorsPolicy";
builder.Services.AddCors(options =>
{
  options.AddPolicy(
    name : frontCorsPolicy,
    policy =>
    {
      var clients = builder.Configuration.GetSection("Clients").Get<List<string>>();

      if(clients?.Count != 0)
      {
        policy.WithOrigins([..clients]);
      }

      // TODO: strange origin works with tunnels
      policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddHostedService<GarbageFileService>();
builder.Services.AddHostedService<OnlineStatsUpdateService>();

builder.Services.ConfigureLocalization();

var app = builder.Build();

app.UseRequestLocalization();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<QueryTokenMiddleware>();

//check for default avatar
if(!File.Exists($"{imageSettings.Folder}/{imageSettings.AvatarDefault}"))
{
  app.Logger.LogWarning($"Default avatar in {imageSettings.Folder}/{imageSettings.AvatarDefault} not found.");
}

//check for default forum
if(!File.Exists($"{imageSettings.Folder}/{imageSettings.ForumDefault}"))
{
  app.Logger.LogWarning($"Default forum in {imageSettings.Folder}/{imageSettings.ForumDefault} not found.");
}

app.UseStaticFiles();

app.UseRouting();
app.UseCors(frontCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI( options => {
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
  options.RoutePrefix = "";
});

app.MapControllers();
app.MapHub<MainHub>("/api/v1/signalr");

app.Run();
