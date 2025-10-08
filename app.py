# app.py
from flask import Flask, request, jsonify
import tensorflow as tf
import numpy as np
from PIL import Image
import io
import os
import base64
from io import BytesIO

# === Tambahan penting untuk dukung AVIF ===
import pillow_avif  # harus diimport sebelum Image.open
# ==========================================

# Inisialisasi Aplikasi Flask
app = Flask(__name__)

# Muat model dan label
try:
    interpreter = tf.lite.Interpreter(model_path="model.tflite")
    interpreter.allocate_tensors()
    input_details = interpreter.get_input_details()
    output_details = interpreter.get_output_details()
    print("✅ TFLite model loaded successfully.")
except Exception as e:
    print(f"❌ Error loading TFLite model: {e}")
    interpreter = None

try:
    with open("labels.txt", "r") as f:
        labels = [line.strip() for line in f.readlines()]
    print(f"✅ Labels loaded: {labels}")
except Exception as e:
    print(f"❌ Error loading labels.txt: {e}")
    labels = []

def predict(inp_image):
    input_shape = input_details[0]['shape']
    height, width = input_shape[1], input_shape[2]
    
    image = inp_image.convert('RGB').resize((width, height))
    image_array = np.array(image, dtype=np.float32) / 255.0
    image_array = np.expand_dims(image_array, axis=0)

    interpreter.set_tensor(input_details[0]['index'], image_array)
    interpreter.invoke()
    output_data = interpreter.get_tensor(output_details[0]['index'])
    
    confidences = {labels[i]: float(output_data[0][i]) for i in range(len(labels))}
    return confidences

# === Endpoint API ===
@app.route('/predict', methods=['POST'])
def handle_prediction():
    if interpreter is None:
        return jsonify({"error": "Model is not loaded"}), 500

    if 'image' not in request.files:
        return jsonify({"error": "No image file provided in the 'image' field"}), 400

    file = request.files['image']
    if file.filename == '':
        return jsonify({"error": "No selected file"}), 400

    try:
        print("\n--- DATA REQUEST DITERIMA ---")
        print(f"Nama File: {file.filename}")
        print(f"Content-Type dari file: {file.content_type}")

        image_bytes = file.read()
        print(f"Ukuran data (bytes): {len(image_bytes)}")
        print(f"50 byte pertama: {image_bytes[:50]}")
        print("---------------------------\n")

        # Buka gambar (sekarang bisa baca AVIF juga)
        pil_image = Image.open(io.BytesIO(image_bytes))
        prediction_results = predict(pil_image)
        return jsonify(prediction_results)

    except Exception as e:
        print(f"🔥 ERROR SAAT MEMPROSES GAMBAR: {str(e)}")
        return jsonify({"error": f"An error occurred: {str(e)}"}), 500

@app.route('/', methods=['GET'])
def index():
    return jsonify({"status": "API is running!"})

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 7860)) 
    app.run(host='0.0.0.0', port=port)
