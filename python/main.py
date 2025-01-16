from codebert import create_codebert_model
from unixcoder import create_unixcoder_model

if __name__ == "__main__":
    folder = "data/csharp-training-data/"
    create_unixcoder_model(f"{folder}csharp_train_data.csv", f"{folder}csharp_test_data.csv")
