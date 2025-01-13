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

            // Act
            var secrets = SecretScanner.ScanForSecrets(codeFilePath, modelPath);

            // Assert
            Assert.NotEmpty(secrets);
            Assert.Contains(secrets, s => s.Description == "Hard-coded secret detected");
        }
    }
}
