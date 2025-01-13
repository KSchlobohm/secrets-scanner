using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace SecretsScanner;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: SecretsScanner <path_to_code_file>");
            return;
        }

        string codeFilePath = args[0];

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        string modelPath = configuration["OnnxModelPath"] ?? "codebert_with_secrets.onnx";
        if (!File.Exists(modelPath))
        {
            if (Path.IsPathRooted(modelPath) == false)
            {
                Console.WriteLine($"Onnx model file not found in directory {Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}");
            }
            else
            {
                Console.WriteLine($"Model file not found: {modelPath}");
            }

            return;
        }

        var secrets = SecretScanner.ScanForSecrets(codeFilePath, modelPath);

        string output = JsonSerializer.Serialize(secrets, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(output);
    }
}
