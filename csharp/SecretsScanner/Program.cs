using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Newtonsoft.Json;

namespace SecretsScanner
{
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
            if (!File.Exists(codeFilePath))
            {
                Console.WriteLine($"File not found: {codeFilePath}");
                return;
            }

            string code = File.ReadAllText(codeFilePath);
            var secrets = ScanForSecrets(code);

            string output = JsonConvert.SerializeObject(secrets, Formatting.Indented);
            Console.WriteLine(output);
        }

        static List<Secret> ScanForSecrets(string code)
        {
            var secrets = new List<Secret>();

            // Load the ONNX model
            using var session = new InferenceSession("fine_tuned_codebert_model.onnx");

            // Tokenize the code
            var tokens = TokenizeCode(code);

            // Run the model
            var input = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_ids", tokens.InputIds),
                NamedOnnxValue.CreateFromTensor("attention_mask", tokens.AttentionMask)
            };
            using var results = session.Run(input);

            // Process the results
            var output = results.First().AsEnumerable<float>().ToArray();
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] > 0.5) // Threshold for detecting a secret
                {
                    secrets.Add(new Secret
                    {
                        FileName = "example.cs",
                        LineNumber = i + 1,
                        CodeSnippet = GetCodeSnippet(code, i),
                        SeverityLevel = "High",
                        Description = "Hard-coded secret detected"
                    });
                }
            }

            return secrets;
        }

        static Tokens TokenizeCode(string code)
        {
            // Implement tokenization logic here
            // This is a placeholder implementation
            return new Tokens
            {
                InputIds = new long[1, 1],
                AttentionMask = new long[1, 1]
            };
        }

        static string GetCodeSnippet(string code, int lineNumber)
        {
            var lines = code.Split('\n');
            int start = Math.Max(0, lineNumber - 2);
            int end = Math.Min(lines.Length - 1, lineNumber + 2);
            return string.Join('\n', lines.Skip(start).Take(end - start + 1));
        }
    }

    class Secret
    {
        public string FileName { get; set; }
        public int LineNumber { get; set; }
        public string CodeSnippet { get; set; }
        public string SeverityLevel { get; set; }
        public string Description { get; set; }
    }

    class Tokens
    {
        public long[,] InputIds { get; set; }
        public long[,] AttentionMask { get; set; }
    }
}
