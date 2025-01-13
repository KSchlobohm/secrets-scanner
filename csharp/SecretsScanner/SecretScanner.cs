using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SecretsScanner;

public class SecretScanner
{
    public static List<SecretDetection> ScanForSecrets(string codeFilePath, string modelPath)
    {
        var secrets = new List<SecretDetection>();

        if (!File.Exists(codeFilePath))
        {
            Console.WriteLine($"File not found: {codeFilePath}");
            return secrets;
        }

        string code = File.ReadAllText(codeFilePath);
        using var session = new InferenceSession(modelPath);

        // Tokenize the code
        var tokens = TokenizeCode(code);

        // Convert long[,] to 1D array and get dimensions
        int[] inputIdsDimensions = { tokens.InputIds.GetLength(0), tokens.InputIds.GetLength(1) };
        long[] inputIdsFlat = tokens.InputIds.Cast<long>().ToArray();
        var inputIdsTensor = new DenseTensor<long>(inputIdsFlat, inputIdsDimensions);

        int[] attentionMaskDimensions = { tokens.AttentionMask.GetLength(0), tokens.AttentionMask.GetLength(1) };
        long[] attentionMaskFlat = tokens.AttentionMask.Cast<long>().ToArray();
        var attentionMaskTensor = new DenseTensor<long>(attentionMaskFlat, attentionMaskDimensions);

        // Run the model
        var input = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor)
        };
        using var results = session.Run(input);

        // Process the results
        var fileName = Path.GetFileName(codeFilePath);
        var output = results.First().AsEnumerable<float>().ToArray();
        for (int i = 0; i < output.Length; i++)
        {
            var codeSnippet = GetCodeSnippet(code, i);

            if (!string.IsNullOrEmpty(codeSnippet) // This is valid code
                    && output[i] > 0.5) // And a secret is detected with confidence exceeding this threshold
            {
                secrets.Add(new SecretDetection
                {
                    FileName = fileName,
                    FullFileName = codeFilePath,
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