"""
CSV dosyasından veri okuyup çizgi, sütun ve pasta grafikleri oluşturan script.
"""

import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

# Türkçe karakter desteği ve görsel stil
plt.rcParams["font.family"] = "DejaVu Sans"
sns.set_theme(style="whitegrid")

CSV_DOSYASI = "veri.csv"


def veriyi_oku(dosya_yolu: str) -> pd.DataFrame:
    """CSV dosyasını pandas ile okur."""
    df = pd.read_csv(dosya_yolu)
    print("Veri önizleme:")
    print(df.head())
    print(f"\nToplam satır: {len(df)}")
    print(f"Sütunlar: {list(df.columns)}")
    return df


def cizgi_grafik(df: pd.DataFrame) -> None:
    """Aylık toplam satış miktarını çizgi grafik olarak çizer."""
    aylik = df.groupby("Ay", as_index=False)["Satis"].sum()
    ay_sirasi = ["Ocak", "Subat", "Mart", "Nisan", "Mayis", "Haziran"]
    aylik["Ay"] = pd.Categorical(aylik["Ay"], categories=ay_sirasi, ordered=True)
    aylik = aylik.sort_values("Ay")

    plt.figure(figsize=(10, 5))
    plt.plot(aylik["Ay"], aylik["Satis"], marker="o", linewidth=2, color="#2E86AB")
    plt.title("Aylık Toplam Satış Miktarı", fontsize=14, fontweight="bold")
    plt.xlabel("Ay")
    plt.ylabel("Satış Adedi")
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.savefig("grafik_cizgi.png", dpi=150)
    plt.show()
    print("Çizgi grafik kaydedildi: grafik_cizgi.png")


def sutun_grafik(df: pd.DataFrame) -> None:
    """Ürün bazında toplam geliri sütun grafik olarak çizer."""
    urun_gelir = df.groupby("Urun", as_index=False)["Gelir"].sum()

    plt.figure(figsize=(8, 5))
    sns.barplot(data=urun_gelir, x="Urun", y="Gelir", palette="viridis")
    plt.title("Ürün Bazında Toplam Gelir", fontsize=14, fontweight="bold")
    plt.xlabel("Ürün")
    plt.ylabel("Gelir (TL)")
    plt.tight_layout()
    plt.savefig("grafik_sutun.png", dpi=150)
    plt.show()
    print("Sütun grafik kaydedildi: grafik_sutun.png")


def pasta_grafik(df: pd.DataFrame) -> None:
    """Ürünlerin satış payını pasta grafik olarak çizer."""
    urun_satis = df.groupby("Urun")["Satis"].sum()

    plt.figure(figsize=(8, 8))
    colors = ["#F18F01", "#2E86AB", "#A23B72"]
    plt.pie(
        urun_satis.values,
        labels=urun_satis.index,
        autopct="%1.1f%%",
        startangle=90,
        colors=colors,
        explode=[0.02] * len(urun_satis),
    )
    plt.title("Ürün Bazında Satış Payı", fontsize=14, fontweight="bold")
    plt.tight_layout()
    plt.savefig("grafik_pasta.png", dpi=150)
    plt.show()
    print("Pasta grafik kaydedildi: grafik_pasta.png")


def main() -> None:
    df = veriyi_oku(CSV_DOSYASI)

    print("\n--- Grafikler oluşturuluyor ---\n")
    cizgi_grafik(df)
    sutun_grafik(df)
    pasta_grafik(df)

    print("\nTüm grafikler başarıyla oluşturuldu.")


if __name__ == "__main__":
    main()
