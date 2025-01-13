using SecretsScanner;
using System.IO;
using Xunit;

namespace SecretsScannerTests
{
    public class SecretsScannerTests
    {
        [Fact]
        public void TestScanForSecrets()
        {
            // Arrange
            string codeFilePath = "../../../../../data/SampleApp/Program.cs";
            string modelPath = "../../../../../codebert_with_secrets.onnx";
            string code = File.ReadAllText(codeFilePath);

            // Act
            var secrets = SecretScanner.ScanForSecrets(code, modelPath);

            // Assert
            Assert.NotEmpty(secrets);
            Assert.Contains(secrets, s => s.Description == "Hard-coded secret detected");
        }
    }
}
