using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Cocona;

namespace InAction.Validation
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void Run([Range(1, 128)]int width, [Range(1, 128)]int height, [Argument][PathExists]string filePath)
        {
            Console.WriteLine($"Size: {width}x{height}");
            Console.WriteLine($"Path: {filePath}");
        }
    }

    class PathExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string path && (Directory.Exists(path) || Directory.Exists(path)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"The path '{value}' is not found.");
        }
    }
}
