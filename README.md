# UTS Augmented Reality & Virtual Reality - Kelompok 3
## Deskripsi Proyek
Aplikasi ini merupakan proyek Ujian Tengah Semester untuk mata kuliah Augmented Reality & Virtual Reality. Dokumen ini menjelaskan mengenai proyek pengembangan aplikasi filter wajah AR berbasis kecerdasan buatan (AI) yang dirancang untuk mendeteksi wajah pengguna secara real-time, mengklasifikasikan sesuai dengan gender pengguna, dan menerapkan filter visual yang sesuai. Fokus utama proyek ini adalah mengimplementasikan dan mengoptimalkan model computer vision secara efisien agar dapat berjalan sepenuhnya di perangkat pengguna (on-device) tanpa ketergantungan pada koneksi internet.
## Anggota Kelompok

| NIM           | Nama Lengkap   |
| ------------- | -------------- |
| 2210511087    | Diaz Saputra   |
| [NIM temanmu] | [Nama temanmu] |
| [NIM temanmu] | [Nama temanmu] |
| [NIM temanmu] | [Nama temanmu] |
## Prasyarat
Untuk prasyarat pengerjaan proyek ini, pastikan perangkat anda memiliki : 
- Unity Hub
- Unity Editor versi yang disepakati tim. Menggunakan versi yang sama sangat penting untuk menghindari konflik.
- Modul Android Build Support (diinstal melalui Unity Hub).
- git
## Cara Menjalankan Aplikasi
Berikut adalah langkah-langkah untuk menjalankan proyek ini:

1.  **Clone Repositori**
    Buka terminal atau Git Bash dan jalankan perintah berikut:
    ```
    git clone https://github.com/d1azdn/uts-arvr.git
    ```

2.  **Buka dengan Unity Hub**
    - Buka Unity Hub.
    - Klik "Open" atau "Add project from disk".
    - Arahkan ke folder tempat kamu melakukan *clone* repositori ini, lalu buka.

3.  **Verifikasi packages**
	- Pada bagian Windows > Package Manager, pastikan telah terinstall
		- ARFoundation

4. **Setting profile**
	- Buka tab file > Project Settings
	- Pada tab XR Plugin Management, checkbox bagian ARCore

5.  **Install APK (Untuk Pengguna Android)**
    - Unduh file `.apk` yang ada di folder `Builds`
    - Pindahkan file `.apk` ke perangkat Android kamu.
    - Instal aplikasi dan jalankan.
## Teknologi yang Digunakan
-   **Engine:** Unity 6000.2
-   **AR SDK:** AR Foundation
-   **3D Modeling:** Prefab from Unity
## Dokumentasi
Berikut merupakan beberapa gambar dokumentasi sprint dan hasil testing file Tensorflow Lite
