using System.Text.RegularExpressions;
using Domain.Services.SpaPrerendererService;
using MintPlayer.AspNetCore.Hsts;
using MintPlayer.AspNetCore.SpaServices.Extensions;
using MintPlayer.AspNetCore.SpaServices.Prerendering;
using MintPlayer.AspNetCore.SpaServices.Routing;
using Presentation;
using WebMarkupMin.AspNetCore8;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSpaStaticFilesImproved(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

builder.Services.AddDependencyGroup();
builder.Services.AddSpaPrerenderingService<SpaPrerenderingService>();
builder.Services.AddWebMarkupMin().AddHttpCompression().AddHtmlMinification();

builder.Services
    .Configure<WebMarkupMinOptions>(options =>
    {
        options.DisablePoweredByHttpHeaders = true;
        options.AllowMinificationInDevelopmentEnvironment = true;
        options.AllowCompressionInDevelopmentEnvironment = true;
        options.DisablePoweredByHttpHeaders = false;
    })
    .Configure<HtmlMinificationOptions>(options =>
    {
        options.MinificationSettings.RemoveEmptyAttributes = true;
        options.MinificationSettings.RemoveRedundantAttributes = true;
        options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
        options.MinificationSettings.RemoveHttpsProtocolFromAttributes = false;
        options.MinificationSettings.MinifyInlineJsCode = true;
        options.MinificationSettings.MinifyEmbeddedJsCode = true;
        options.MinificationSettings.MinifyEmbeddedJsonData = true;
        options.MinificationSettings.WhitespaceMinificationMode = WebMarkupMin.Core.WhitespaceMinificationMode.Aggressive;
    });

var app = builder.Build();

app.UseImprovedHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

if (!builder.Environment.IsDevelopment())
{
    app.UseSpaStaticFilesImproved();
}

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.UseWebMarkupMin();

app.UseSpaImproved(spa =>
{
    spa.Options.SourcePath = "ClientApp";
    // For angular 17
    spa.Options.CliRegexes = [new Regex(@"Local\:\s+(?<openbrowser>https?\:\/\/(.+))")];
    spa.UseSpaPrerendering(options =>
    {
        options.BootModuleBuilder = builder.Environment.IsDevelopment() ? new AngularPrerendererBuilder(npmScript: "build:ssr") : null;
        options.BootModulePath = $"{spa.Options.SourcePath}/dist/server/main.js";
        options.ExcludeUrls = ["/sockjs-node"];
    });
    app.UseWebMarkupMin();
    if (builder.Environment.IsDevelopment())
    {
        spa.UseAngularCliServer(npmScript: "start");
    }
});

app.Run();