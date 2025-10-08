# test_api.py
import requests

# --- KONFIGURASI ---
# Ganti dengan URL Space Anda. Pastikan ada '/predict' di akhir.
# Contoh: "https://nama-anda-nama-space-anda.hf.space/predict"
API_URL = "https://diazdn-face-detection.hf.space/predict" 

# Ganti dengan path ke file gambar yang ingin Anda tes di komputer Anda.
# Contoh: "images/wanita1.jpg" atau "C:/Users/Andi/Pictures/pria.png"
IMAGE_PATH = "face-detection/contoh-wanita.jpg"
# --------------------

def predict_gender(api_url, image_path):
    """
    Mengirim gambar ke API dan mencetak hasil prediksi.
    """
    print(f"Mengirim gambar: {image_path} ke API: {api_url}")

    try:
        # Buka file gambar dalam mode binary ('rb' = read binary)
        with open(image_path, "rb") as image_file:
            # Siapkan file untuk dikirim.
            # Kunci ('image') harus sama dengan yang diharapkan oleh server Flask.
            # di app.py kita menggunakan: request.files['image']
            files_to_send = {'image': image_file}

            # Kirim POST request dengan file dan timeout 15 detik
            response = requests.post(api_url, files=files_to_send, timeout=15)

            # Cek jika ada error dari sisi server (spt 4xx atau 5xx)
            response.raise_for_status() 

            # Jika berhasil, server akan mengembalikan JSON. Kita parse JSON tersebut.
            prediction = response.json()
            
            print("\n--- Hasil Prediksi ---")
            for gender, confidence in prediction.items():
                # Tampilkan dalam format persentase yang rapi
                print(f"{gender.capitalize()}: {confidence:.2%}")
            print("----------------------")

    except FileNotFoundError:
        print(f"Error: File tidak ditemukan di '{image_path}'")
    except requests.exceptions.RequestException as e:
        print(f"Error saat melakukan request ke API: {e}")
    except Exception as e:
        print(f"Terjadi error yang tidak terduga: {e}")

# Panggil fungsi untuk memulai proses
if __name__ == "__main__":
    if "NAMA-ANDA" in API_URL or "path/ke/gambar" in IMAGE_PATH:
        print("!!! PENTING: Harap ubah variabel API_URL dan IMAGE_PATH di dalam skrip terlebih dahulu.")
    else:
        predict_gender(API_URL, IMAGE_PATH)