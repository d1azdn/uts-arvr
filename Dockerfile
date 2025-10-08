# Dockerfile
FROM python:3.9-slim

# Install library sistem yang dibutuhkan untuk AVIF/HEIF
RUN apt-get update && apt-get install -y libheif-dev

WORKDIR /code
COPY requirements.txt .
RUN pip install --no-cache-dir --upgrade -r requirements.txt
COPY . .
EXPOSE 7860
CMD ["python", "app.py"]