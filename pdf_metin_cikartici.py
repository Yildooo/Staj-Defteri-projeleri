
import sys
from PyPDF2 import PdfReader

def extract_text(pdf_path, output_path):
    reader = PdfReader(pdf_path)
    text = []
    for i, page in enumerate(reader.pages):
        text.append(f"--- Sayfa {i+1} ---\n")
        page_text = page.extract_text()
        if page_text:
            text.append(page_text)
        text.append("\n")
    with open(output_path, "w", encoding="utf-8") as f:
        f.writelines(text)

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Kullanım: python pdf_metin_cikartici.py input.pdf output.txt")
        sys.exit(1)

    pdf_path = sys.argv[1]
    output_path = sys.argv[2]
    extract_text(pdf_path, output_path)
    print("Metin başarıyla çıkarıldı.")
