using System.ComponentModel.DataAnnotations;
using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace InAction.Validation;

class Program
{
    static void Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);
        builder.Services
            .AddTransient<IFileProvider>(serviceProvider => serviceProvider.GetRequiredService<IHostEnvironment>().ContentRootFileProvider);

        var app = builder.Build();
        app.Run<Program>();
    }

    public void Run([Range(1, 128)]int width, [Range(1, 128)]int height, [Argument][PathExists][PathExistsWithDI] string filePath)
    {
        Console.WriteLine($"Size: {width}x{height}");
        Console.WriteLine($"Path: {filePath}");
    }
}

class PathExistsAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string path && (Directory.Exists(path) || File.Exists(path)))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"The path '{value}' is not found.");
    }
}

class PathExistsWithDIAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var fileSystem = validationContext.GetRequiredService<IFileProvider>();
        if (value is string path && fileSystem.GetFileInfo(path).Exists)
        {
            return ValidationResult.Success;
        }
        
        return new ValidationResult($"The path '{value}' is not found.");
    }
}
