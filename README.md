# Uçuş Bilgi Sistemi

C# ve SQLite ile geliştirdiğim konsol tabanlı uçuş yönetim uygulaması.

## Özellikler
- Uçuş ekleme, listeleme, şehre göre arama, silme
- Yolcu ekleme ve listeleme
- Koltuk rezervasyonu (aynı koltuk iki kez satılamaz)
- Bir uçuşun yolcu listesini görüntüleme

## Kullanılan Teknolojiler
C#, .NET, SQLite, Microsoft.Data.Sqlite

## Veritabanı
Tablolar: Havalimani, Ucus, Yolcu, Rezervasyon.
Yolcu ve Uçuş arasında Rezervasyon tablosu üzerinden çoka çok ilişki vardır.

## Kurulum
1. `schema.sql` dosyasını "DB Browser for SQLite" uygulaması ile çalıştırıp `ucus_sistemi.db` oluştur.
2. `Program.cs` içindeki `dbYolu` değişkenine dosyanın yolunu yaz.
3. Visual Studio'da projeyi çalıştır.

## Öğrendiklerim
- İlişkisel veritabanı tasarımı, JOIN, foreign key, UNIQUE kısıtı
- Parametreli sorgularla güvenli veri işleme
- try/catch ile hata yönetimi

## Gelecek Planlar
- Web arayüzü (ASP.NET Core)
