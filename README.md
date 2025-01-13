# Secrets Scanner

## Introduction

This project aims to develop a command-line tool to scan C# code for hard-coded secrets using a fine-tuned ONNX model of CodeBERT. The tool will help identify potential security risks in the code by detecting hard-coded secrets such as API keys, passwords, and connection strings.

## Prerequisites

- Python 3.6 or higher
- .NET Core SDK 3.1 or higher
- ONNX Runtime
- Transformers library
- Torch library
- Newtonsoft.Json library

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/githubnext/workspace-blank.git
   cd workspace-blank
   ```

2. Install the required Python libraries:
   ```
   pip install onnx transformers torch
   ```

3. Install the required .NET libraries:
   ```
   dotnet add package Microsoft.ML.OnnxRuntime
   dotnet add package Newtonsoft.Json
   ```

## Usage

1. Fine-tune the CodeBERT model and convert it to ONNX format:
   ```
   python python/main.py
   ```

2. Run the Secrets Scanner tool on a C# code file:
   ```
   dotnet run --project csharp/SecretsScanner/SecretsScanner.csproj <path_to_code_file>
   ```

   Example command:
   ```
   dotnet run --project csharp/SecretsScanner/SecretsScanner.csproj data/SampleApp/Program.cs
   ```

## Data Directory

The `data` directory contains a simple console app written in C# with a hard-coded secret used when sending an email. This directory is used to validate the functionality of the Secrets Scanner tool.

## Tests

To run the tests project and validate the tool's functionality:
```
dotnet test tests/SecretsScannerTests/SecretsScannerTests.csproj
```

## Output Format

The output of the scan is in JSON format and includes the following information:
- File name and path of the scanned file
- Line number(s) where the hard-coded secret is detected
- A snippet of the code around the detected secret for context
- A severity level indicating the potential risk of the detected secret
- A message or description explaining why the detected code is considered a hard-coded secret

## Contributing

We welcome contributions to the project. Please follow these guidelines:
- Fork the repository and create a new branch for your feature or bugfix.
- Write clear and concise commit messages.
- Ensure that your code follows the project's coding standards and passes all tests.
- Submit a pull request with a detailed description of your changes.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more information.
