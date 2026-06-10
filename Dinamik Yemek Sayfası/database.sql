CREATE DATABASE IF NOT EXISTS yemek_tarifleri CHARACTER SET utf8mb4 COLLATE utf8mb4_turkish_ci;
USE yemek_tarifleri;

CREATE TABLE IF NOT EXISTS tarifler (
    id INT AUTO_INCREMENT PRIMARY KEY,
    baslik VARCHAR(150) NOT NULL,
    aciklama TEXT NOT NULL,
    malzemeler TEXT NOT NULL,
    hazirlanis TEXT NOT NULL,
    sure VARCHAR(50) NOT NULL,
    porsiyon VARCHAR(50) NOT NULL,
    resim VARCHAR(255) DEFAULT NULL
);

INSERT INTO tarifler (baslik, aciklama, malzemeler, hazirlanis, sure, porsiyon) VALUES
(
    'Mercimek Çorbası',
    'Klasik Türk mutfağının vazgeçilmez, doyurucu ve besleyici çorbası.',
    '1 su bardağı kırmızı mercimek\n1 adet soğan\n1 adet havuç\n2 yemek kaşığı sıvı yağ\n1 yemek kaşığı un\n6 su bardağı su\nTuz, karabiber\nLimon (servis için)',
    '1. Soğan ve havucu küp doğrayıp yağda kavurun.\n2. Unu ekleyip 1 dakika kavurmaya devam edin.\n3. Yıkanmış mercimeği ve suyu ekleyin.\n4. Sebzeler yumuşayana kadar pişirin.\n5. Blenderdan geçirip tuz ve karabiberle tatlandırın.\n6. Limonla servis edin.',
    '40 dakika',
    '4 kişilik'
),
(
    'Menemen',
    'Kahvaltı sofralarının en sevilen, pratik ve lezzetli yemeği.',
    '3 adet yumurta\n2 adet domates\n2 adet yeşil biber\n2 yemek kaşığı sıvı yağ\nTuz, karabiber, pul biber',
    '1. Biberleri ince doğrayıp yağda kavurun.\n2. Rendelenmiş domatesleri ekleyip suyunu çekene kadar pişirin.\n3. Yumurtaları kırıp karıştırın.\n4. İstenilen kıvama gelene kadar pişirin.\n5. Sıcak servis edin.',
    '15 dakika',
    '2 kişilik'
),
(
    'Karnıyarık',
    'Patlıcan ve kıymalı harçla hazırlanan geleneksel bir ana yemek.',
    '4 adet patlıcan\n300 g kıyma\n2 adet soğan\n2 adet domates\n2 adet yeşil biber\nSıvı yağ, tuz, karabiber',
    '1. Patlıcanları alacalı soyup kızartın.\n2. Soğan ve biberi kavurup kıymayı ekleyin.\n3. Domates ve baharatlarla harcı hazırlayın.\n4. Patlıcanları ortadan yarın, harçla doldurun.\n5. Fırında veya tencerede pişirip servis edin.',
    '60 dakika',
    '4 kişilik'
);
