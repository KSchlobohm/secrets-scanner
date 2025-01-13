import onnx
import torch
from transformers import RobertaModel, RobertaTokenizer

def load_pretrained_codebert_model():
    model_name = "microsoft/codebert-base"
    model = RobertaModel.from_pretrained(model_name)
    tokenizer = RobertaTokenizer.from_pretrained(model_name)
    return model, tokenizer

def convert_model_to_onnx(model, tokenizer, output_path):
    dummy_input = tokenizer.encode_plus("int dummy input", return_tensors="pt")
    torch.onnx.export(model, (dummy_input['input_ids'], dummy_input['attention_mask']), output_path, 
                      input_names=['input_ids', 'attention_mask'], 
                      output_names=['output'], 
                      dynamic_axes={'input_ids': {0: 'batch_size'}, 'attention_mask': {0: 'batch_size'}, 'output': {0: 'batch_size'}})

def save_onnx_model(model, output_path):
    onnx.save(model, output_path)
