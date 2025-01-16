using Microsoft.Extensions.Primitives;
using Microsoft.ML.Tokenizers;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SecretsScanner;

public class SecretScanner
{
    public static List<DetectionResult> ScanForSecrets(string codeFilePath, string modelPath)
    {
        if (!File.Exists(codeFilePath))
        {
            Console.WriteLine($"File not found: {codeFilePath}");
            return default;
        }

        string exampleCode = File.ReadAllText(codeFilePath);

        // Run inference
        return AnalyzeCode(modelPath, exampleCode);
    }

    static List<DetectionResult> AnalyzeCode(string modelPath, string code)
    {
        // Load ONNX model
        using var session = new InferenceSession(modelPath);

        var results = new List<DetectionResult>();
        var lines = code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Skip empty or irrelevant lines
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Tokenize the line (simplified; replace with proper tokenizer logic)
            var tokenizedInput = Tokenize(line);
            if (tokenizedInput == null)
            {
                continue;
            }

            // Create ONNX input tensors
            var inputIdsTensor = new DenseTensor<long>(tokenizedInput.InputIds, new[] { 1, tokenizedInput.InputIds.Length });
            var attentionMaskTensor = new DenseTensor<long>(tokenizedInput.AttentionMask, new[] { 1, tokenizedInput.AttentionMask.Length });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
                NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor)
            };

            // Run inference
            using var inferenceResults = session.Run(inputs);
            var logits = inferenceResults.First().AsEnumerable<float>().ToArray();

            // Apply softmax to get probabilities
            var probabilities = Softmax(logits);
            var predictedLabel = Array.IndexOf(probabilities, probabilities.Max());

            // If the line is predicted to contain a secret, add it to the results
            if (predictedLabel == 1)
            {
                results.Add(new DetectionResult
                {
                    LineNumber = i + 1,
                    Snippet = line,
                    Confidence = probabilities[1] // Confidence for label 1
                });
            }
        }

        return results;
    }

    static TokenizedInput Tokenize(string code)
    {
        var tokenizer = BpeTokenizer.Create("vocab.json", "merges.txt");

        // Tokenize the input code
        var encoding = tokenizer.EncodeToIds(code);

        // Convert tokens to input IDs
        var inputIds = encoding.Select(id => (long)id).ToArray();

        // Create attention mask (1 for actual tokens, 0 for padding)
        var attentionMask = new long[inputIds.Length];
        for (int i = 0; i < inputIds.Length; i++)
        {
            attentionMask[i] = 1;
        }

        return new TokenizedInput
        {
            InputIds = inputIds,
            AttentionMask = attentionMask
        };
    }

    static float[] Softmax(float[] logits)
    {
        var maxLogit = logits.Max();
        var expLogits = logits.Select(logit => MathF.Exp(logit - maxLogit)).ToArray();
        var sumExpLogits = expLogits.Sum();
        return expLogits.Select(expLogit => expLogit / sumExpLogits).ToArray();
    }

    class TokenizedInput
    {
        public long[] InputIds { get; set; }
        public long[] AttentionMask { get; set; }
    }

    public class DetectionResult
    {
        public int LineNumber { get; set; }
        public string Snippet { get; set; }
        public float Confidence { get; set; }
    }
}