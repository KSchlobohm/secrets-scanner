# Secrets Scanner

## Introduction

This project aims to develop a command-line tool to scan C# code for hard-coded secrets using a fine-tuned ONNX model of CodeBERT. The tool will help identify potential security risks in the code by detecting hard-coded secrets such as API keys, passwords, and connection strings.

> **Note:** This is a learning project built to explore the use of AI tools for generating code and solving business problems such as scanning for hard-coded secrets in code. This project is not under maintenance and it is not recommended for production.

## Prerequisites

- Python 3.6 or higher
- .NET Core SDK 3.1 or higher
- ONNX Runtime
- Transformers library
- Torch library (version 1.9.0 or higher)
- Newtonsoft.Json library

## Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/githubnext/workspace-blank.git
   cd workspace-blank
   ```

2. Open the project in a devcontainer:
   - Ensure you have Docker installed and running.
   - Open the project folder in Visual Studio Code.
   - When prompted, reopen the project in a devcontainer.

3. Create a Venv
   ```sh
   python -m venv venv
   source venv/bin/activate  # On Windows use `venv\Scripts\activate`
   ```

4. Install the required Python libraries:
   
   > **Note:** The required Python libraries were installed using the following command:
   > ```sh
   > pip install onnx transformers torch==1.9.0
   > ```
   
   ```sh
   pip install -r requirements.txt
   ```

## Usage

1. Fine-tune the CodeBERT model and convert it to ONNX format:
   ```sh
   python python/main.py
   ```

2. Run the Secrets Scanner tool on a C# code file:
   ```sh
   dotnet run --project csharp/SecretsScanner/SecretsScanner.csproj <path_to_code_file>
   ```

   Example command:
   ```sh
   dotnet run --project csharp/SecretsScanner/SecretsScanner.csproj data/SampleApp/Program.cs
   ```

## Data Directory

The `data` directory contains a simple console app written in C# with a hard-coded secret used when sending an email. This directory is used to validate the functionality of the Secrets Scanner tool.

## Tests

To run the tests project and validate the tool's functionality:
```sh
dotnet test tests/SecretsScannerTests/SecretsScannerTests.csproj
```

## Output Format

The output of the scan is in JSON format and includes the following information:
- File name and path of the scanned file
- Line number(s) where the hard-coded secret is detected
- A snippet of the code around the detected secret for context
- A severity level indicating the potential risk of the detected secret
- A message or description explaining why the detected code is considered a hard-coded secret

## License

This project is licensed under the MIT License. See the `LICENSE` file for more information.
