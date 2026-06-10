# 9. Basit Not Paylaşımı

Kullanıcıların yazdığı notları cihazın paylaşım özelliğiyle (e-posta, mesajlaşma uygulamaları vb.) paylaşabileceği bir Android not uygulaması.

## Nasıl Yaptım?

1. **Android Studio** ile kullanıcı arayüzünü oluşturdum (`activity_main.xml`, `item_not.xml`).
2. Notları **SQLite** veritabanında tuttum (`DatabaseHelper.java`).
3. Paylaşım için **Android Intent** kullandım (`Intent.ACTION_SEND`).

## Özellikler

- Yeni not ekleme (başlık + içerik)
- Notları listeleme
- Notu silme
- Notu paylaşma (WhatsApp, Gmail, SMS vb.)

## Paylaşım Kodu

```java
Intent intent = new Intent(Intent.ACTION_SEND);
intent.setType("text/plain");
intent.putExtra(Intent.EXTRA_SUBJECT, not.getBaslik());
intent.putExtra(Intent.EXTRA_TEXT, notMetni);
startActivity(Intent.createChooser(intent, "Notu paylaş"));
```

## Proje Yapısı

```
app/src/main/java/com/example/notpaylasimi/
├── MainActivity.java      # Ana ekran ve paylaşım mantığı
├── DatabaseHelper.java    # SQLite veritabanı işlemleri
├── Not.java               # Not model sınıfı
└── NotAdapter.java        # RecyclerView adaptörü
```

## Çalıştırma

1. Android Studio'da **File → Open** ile bu klasörü açın.
2. Gradle senkronizasyonunun bitmesini bekleyin.
3. Emülatör veya fiziksel cihaz seçip **Run** (▶) tuşuna basın.

## Gereksinimler

- Android Studio Hedgehog veya üzeri
- Minimum SDK: 24 (Android 7.0)
- Target SDK: 34
