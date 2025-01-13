using System;
using System.IO;
using Xunit;
using SecretsScanner;

namespace SecretsScannerTests
{
    public class SecretsScannerTests
    {
        [Fact]
        public void TestScanForSecrets()
        {
            // Arrange
            string codeFilePath = "../../data/SampleApp/Program.cs";
            string code = File.ReadAllText(codeFilePath);

            // Act
            var secrets = Program.ScanForSecrets(code);

            // Assert
            Assert.NotEmpty(secrets);
            Assert.Contains(secrets, s => s.Description == "Hard-coded secret detected");
        }
    }
}
