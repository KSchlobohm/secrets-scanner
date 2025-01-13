from transformers import AutoModel, AutoTokenizer
import torch

# Assumption 1: The file 'python/dotnet_code_snippets.txt' contains at least two code snippets separated by "\n---\n"
# Assumption 2: The model expects inputs with keys 'input_ids' and 'attention_mask'
# Assumption 3: The snippets are located in the same directory as this script
# Assumption 4: This script is located in the 'python' directory

def load_code_snippets(file_path):
    with open(file_path, 'r') as file:
        return file.read().split("\n---\n")

def main():
    # Load the pretrained CodeBERT model and tokenizer
    model = AutoModel.from_pretrained("microsoft/codebert-base")
    tokenizer = AutoTokenizer.from_pretrained("microsoft/codebert-base")

    # Load example code snippets from a text file
    # assumes that the snippets are located in the same directory as this script,
    # and that this script is located in the python directory
    example_code_snippets = load_code_snippets("python/dotnet_code_snippets.txt")

    all_inputs = []
    for i, code_snippet in enumerate(example_code_snippets):
        # Tokenize the example input
        tokenized_input = tokenizer(
            code_snippet,
            return_tensors="pt",       # PyTorch tensors
            max_length=512,            # Truncate or pad to model's max sequence length
            padding="max_length",      # Ensure input length matches max_length
            truncation=True            # Truncate sequences longer than max_length
        )

        # Verify input structure
        print(f"Tokenized Input Keys for snippet {i+1}:", tokenized_input.keys())
        # Should include keys like 'input_ids', 'attention_mask', and optionally 'token_type_ids'
        all_inputs.append(tokenized_input)

    # Combine all tokenized inputs
    combined_input = {}
    for key in all_inputs[0].keys():
        combined_input[key] = torch.cat([inp[key] for inp in all_inputs], dim=0)

    # Export the model to ONNX format
    torch.onnx.export(
        model,                                          # Model to export
        tuple(combined_input.values()),                 # Combined input tuple
        "codebert_with_secrets.onnx",                   # Single output file name
        input_names=list(combined_input.keys()),        # Names of the input tensors
        output_names=["output"],                        # Names of the output tensors
        dynamic_axes={                                  # Allow variable-length sequences
            key: {0: "batch_size", 1: "seq_len"} for key in combined_input
        },
        opset_version=14,                               # ONNX opset version (14 or later)
        do_constant_folding=True                        # Enable constant folding for optimization
    )

if __name__ == "__main__":
    main()
