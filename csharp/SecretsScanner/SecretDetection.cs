using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretsScanner;

public class SecretDetection
{
    public string FileName { get; set; }
    public int LineNumber { get; set; }
    public string CodeSnippet { get; set; }
    public string SeverityLevel { get; set; }
    public string Description { get; set; }
}
