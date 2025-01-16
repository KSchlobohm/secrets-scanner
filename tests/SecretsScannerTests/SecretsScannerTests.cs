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
            string modelPath = "../../../../../unixcoder_hardcoded_secrets.onnx";

            // Act
            var secrets = SecretScanner.ScanForSecrets(codeFilePath, modelPath);

            // Assert
            Assert.NotEmpty(secrets);
        }
    }
}
