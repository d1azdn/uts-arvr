# app.py
import gradio as gr
import tensorflow as tf
import numpy as np
from PIL import Image

# 1. Muat model TFLite (tidak ada perubahan)
try:
    interpreter = tf.lite.Interpreter(model_path="model.tflite")
    interpreter.allocate_tensors()
    input_details = interpreter.get_input_details()
    output_details = interpreter.get_output_details()
    print("TFLite model loaded successfully.")
except Exception as e:
    print(f"Error loading TFLite model: {e}")

# 2. Muat label dari file labels.txt (sekarang akan membaca 'female', 'male')
with open("labels.txt", "r") as f:
    labels = [line.strip() for line in f.readlines()]
print(f"Labels loaded: {labels}")

# 3. Fungsi untuk prediksi (tidak ada perubahan)
def predict(inp_image):
    input_shape = input_details[0]['shape']
    height = input_shape[1]
    width = input_shape[2]

    image = inp_image.resize((width, height))
    image_array = np.array(image, dtype=np.float32)
    image_array = image_array / 255.0
    image_array = np.expand_dims(image_array, axis=0)

    interpreter.set_tensor(input_details[0]['index'], image_array)
    interpreter.invoke()
    output_data = interpreter.get_tensor(output_details[0]['index'])
    
    confidences = {labels[i]: float(output_data[0][i]) for i in range(len(labels))}
    
    return confidences

# 4. Bangun Antarmuka Gradio (Judul dan Deskripsi diperbarui ke Bahasa Inggris)
iface = gr.Interface(
    fn=predict,
    inputs=gr.Image(type="pil", label="Upload Face Image"),
    outputs=gr.Label(num_top_classes=2, label="Prediction Results"),
    title="Female / Male Gender Detection",
    description="Upload a facial image to be classified by the model. This app uses a TFLite model and automatically provides a UI and a REST API.",
    examples=[
        ["contoh-pria.jpg"],
        ["contoh-wanita.jpg"]
    ]
)

# 5. Luncurkan aplikasi
iface.launch()