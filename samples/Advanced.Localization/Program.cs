using System.Globalization;
using CoconaSample.Advanced.Localization;
using WildCoco;
using WildCoco.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

// Simulate ja-jp locale environment.
Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ja-jp");

// Register Microsoft.Extensions.Localization and IWildCocoLocalizer services
// WildCoco uses `IWildCocoLocalizer` to localize command descriptions.
var builder = WildCocoApp.CreateBuilder();
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
builder.Services.TryAddTransient<IWildCocoLocalizer, MicrosoftExtensionLocalizationWildCocoLocalizer>();

var app = builder.Build();
app.AddCommand("hello", ([Argument(Description = "Name")]string name, IStringLocalizer<Program> localizer) =>
    {
        // Get a localized text from Microsoft.Extensions.Localization.IStringLocalizer (same as ASP.NET Core)
        Console.WriteLine(localizer.GetString("Hello {0}!", name));
    })
    .WithDescription("Say Hello");
app.Run();
