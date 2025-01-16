from datasets import load_dataset
from transformers import RobertaTokenizer, RobertaForSequenceClassification, Trainer, TrainingArguments
import torch

# Assumption 1: The file training and test data are located in 'data/csharp-training-data/' folder by default

def create_unixcoder_model(train_file_path, test_file_path):
    # set default paths for training and testing data
    if not train_file_path:
        train_file_path = "data/csharp-training-data/csharp_train_data.csv"
    
    if not test_file_path:
        test_file_path = "data/csharp-training-data/csharp_test_data.csv"

    # Load dataset
    dataset = load_dataset("csv", data_files={"train": train_file_path, "test": test_file_path})

    # Load UniXcoder tokenizer and model
    model_name = "microsoft/unixcoder-base"
    tokenizer = RobertaTokenizer.from_pretrained(model_name)
    model = RobertaForSequenceClassification.from_pretrained(model_name, num_labels=2)

    # Tokenize the dataset
    def preprocess_function(examples):
        return tokenizer(examples["text"], truncation=True, padding="max_length", max_length=512)

    tokenized_dataset = dataset.map(preprocess_function, batched=True)

    # Fine-tuning arguments
    training_args = TrainingArguments(
        output_dir="./results",
        evaluation_strategy="epoch",
        save_strategy="epoch",
        learning_rate=2e-5,
        per_device_train_batch_size=16,
        per_device_eval_batch_size=16,
        num_train_epochs=3,
        weight_decay=0.01,
        logging_dir="./logs",
        logging_steps=10,
        load_best_model_at_end=True,
        save_total_limit=2
    )

    # Initialize Trainer
    trainer = Trainer(
        model=model,
        args=training_args,
        train_dataset=tokenized_dataset["train"],
        eval_dataset=tokenized_dataset["test"]
    )

    # Fine-tune the model
    trainer.train()

    # Move the model to CPU
    model.cpu()

    # Save the fine-tuned model
    model.save_pretrained("./fine_tuned_unixcoder")
    tokenizer.save_pretrained("./fine_tuned_unixcoder")

    # Example input for dummy input
    dummy_input = tokenizer(
        "public const string API_KEY = \"AKIA123456789EXAMPLE\";",
        return_tensors="pt",
        padding="max_length",
        max_length=512,
        truncation=True
    )
    dummy_input = {k: v.cpu() for k, v in dummy_input.items()}

    # Export to ONNX
    torch.onnx.export(
        model,
        (dummy_input["input_ids"], dummy_input["attention_mask"]),
        "unixcoder_hardcoded_secrets.onnx",
        input_names=["input_ids", "attention_mask"],
        output_names=["logits"],
        dynamic_axes={
            "input_ids": {0: "batch_size", 1: "sequence_length"},
            "attention_mask": {0: "batch_size", 1: "sequence_length"},
            "logits": {0: "batch_size", 1: "num_labels"}
        },
        opset_version=14
    )