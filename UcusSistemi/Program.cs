using Microsoft.Data.Sqlite;
using System.Security.Cryptography.X509Certificates;

string dbYolu = @"C:\Users\Bumin\Desktop\ucus_sistemi.db";

using var baglanti = new SqliteConnection($"Data Source={dbYolu}");
baglanti.Open();

using var pragmaKomut = new SqliteCommand("PRAGMA foreign_keys = ON;", baglanti);
pragmaKomut.ExecuteNonQuery();

bool devam = true;
while (devam)
{
    Console.WriteLine();
    Console.WriteLine("=== UCUS SISTEMI ===");
    Console.WriteLine("1 - Ucuslari listele");
    Console.WriteLine("2 - Ucus ekle");
    Console.WriteLine("3 - Ucus ara (şehre göre)");
    Console.WriteLine("4 - Ucus Sil");
    Console.WriteLine("5 - Yolcu Ekle");
    Console.WriteLine("6 - Yolcuları Listele");
    Console.WriteLine("7 - Rezervasyon Yap");
    Console.WriteLine("8 - Secilen ucusun yolcu listesi");
    Console.WriteLine("9 - Cikis");
    Console.Write("Seciminiz: ");
    string secim = Console.ReadLine() ?? "";

    switch (secim)
    {
        case "1":
            Console.Clear();
            UcuslariListele(baglanti);           
            break;
        case "2":
            Console.Clear();
            UcusEkle(baglanti);
            break;
        case "3":
            Console.Clear();
            UcusAra(baglanti);
            break;
        case "4":
            Console.Clear();
            UcusSil(baglanti);
            break;
        case "5":
            Console.Clear();
            YolcuEkle(baglanti);
            break;
        case "6":
            Console.Clear();
            YolculariListele(baglanti);
            break;
        case "7":
            Console.Clear();
            RezervasyonYap(baglanti);
            break;
        case "8":
            Console.Clear();
            UcusYolculari(baglanti);
            break;
        case "9":
            Console.Clear();
            devam = false;
            break;
        default:
            Console.Clear();
            Console.WriteLine("Gecersiz secim!");
            break;
    }
}

Console.WriteLine();
Console.WriteLine("Oturum Sona Erdi..");

static int SatirlariYazdir(SqliteDataReader okuyucu)
{
    int sayac = 0;
    Console.WriteLine();
    while( okuyucu.Read())
    {
        Console.WriteLine($"{okuyucu.GetString(0)}: {okuyucu.GetString(1)} -> {okuyucu.GetString(2)} | {okuyucu.GetString(3)} ");
        sayac++;
    }
    return sayac;
}

static void UcuslariListele(SqliteConnection baglanti)
{
    string sql = @"SELECT u.UcusNo, k.Sehir, v.Sehir, u.KalkisSaati
                   FROM Ucus u
                   JOIN Havalimani k ON u.KalkisHavalimaniId = k.Id
                   JOIN Havalimani v ON u.VarisHavalimaniId = v.Id";

    using var komut = new SqliteCommand(sql, baglanti);
    using var okuyucu = komut.ExecuteReader();

    Console.WriteLine();
    while (okuyucu.Read())
    {
        Console.WriteLine($"{okuyucu.GetString(0)}: {okuyucu.GetString(1)} -> {okuyucu.GetString(2)} | {okuyucu.GetString(3)}");
    }
}

static void UcusAra(SqliteConnection baglanti)
{
    Console.Write("Aranacak Şehir (Türkçe Harf Kullanma):");
    string sehir = Console.ReadLine() ?? "";

    string sql = @"SELECT u.UcusNo, k.Sehir, v.Sehir, u.KalkisSaati
                    FROM Ucus u
                    JOIN Havalimani k ON u.KalkisHavalimaniId = k.Id
                    JOIN Havalimani v ON u.VarisHavalimaniId = v.Id
                    WHERE k.Sehir LIKE @sehir OR v.Sehir LIKE @sehir";

    using var komut = new SqliteCommand(sql,baglanti);
    komut.Parameters.AddWithValue("@sehir", "%" + sehir + "%");
    using var okuyucu =komut.ExecuteReader();

    int bulunan = SatirlariYazdir(okuyucu);
    if(bulunan==0)
    {
        Console.WriteLine("Bu şehirle ilgili uçuş bulunamadı.");
    }
}

static void UcusEkle(SqliteConnection baglanti)
{
    Console.Write("Yeni ucus no (ornek TK1234): ");
    string ucusNo = Console.ReadLine() ?? "";

    int kalkisId = SayiOku("Kalkis havalimani Id (1=IST, 2=ESB, 3=ADB): ");
    int varisId = SayiOku("Varis havalimani Id: ");

    Console.Write("Kalkis saati (ornek 2026-12-05 18:00): ");
    string saat = Console.ReadLine() ?? "";

    using var komut = new SqliteCommand(
        "INSERT INTO Ucus (UcusNo, KalkisHavalimaniId, VarisHavalimaniId, KalkisSaati) VALUES (@no, @kalkis, @varis, @saat)",
        baglanti);
    komut.Parameters.AddWithValue("@no", ucusNo);
    komut.Parameters.AddWithValue("@kalkis", kalkisId);
    komut.Parameters.AddWithValue("@varis", varisId);
    komut.Parameters.AddWithValue("@saat", saat);
    komut.ExecuteNonQuery();

    Console.WriteLine("Ucus eklendi!");
}

static void UcusSil(SqliteConnection baglanti)
{
    Console.Write("Silinicek Ucus No: ");
    string ucusno = Console.ReadLine() ?? "";

    Console.Write($"{ucusno} Silinsinmi? (Evet/Hayır): ");
    string onay = Console.ReadLine() ?? "";
    if(onay.ToLower() != "evet")
    {
        Console.WriteLine("İptal Edildi.");
        return;
    }

    try
    {
        using var komut = new SqliteCommand("DELETE FROM Ucus WHERE UcusNo = @no", baglanti);
        komut.Parameters.AddWithValue("@no", ucusno);
        int silinen = komut.ExecuteNonQuery();

        if (silinen > 0)
        {
            Console.WriteLine($"{silinen} ucus silindi.");
        }
        else
        {
            Console.WriteLine("Bu numarayla bir ucus bulunamadi.");
        }
    }
    catch (SqliteException)
    {
        Console.WriteLine("Bu uçuşta rezervasyon var, once rezervasyonlar silinmeli. Ucus silinmedi.");
    }
}

static void YolcuEkle(SqliteConnection baglanti)
{
    Console.Write("Adi: ");
    string ad = Console.ReadLine() ?? "";
    Console.Write("Soyad: ");
    string soyad = Console.ReadLine() ?? "";
    Console.Write("E-posta: ");
    string eposta = Console.ReadLine() ?? "";

    using var komut = new SqliteCommand(
            "INSERT INTO Yolcu (Ad,Soyad,Eposta) VALUES (@ad, @soyad, @eposta)", baglanti);

    komut.Parameters.AddWithValue("@ad", ad);
    komut.Parameters.AddWithValue("@soyad", soyad);
    komut.Parameters.AddWithValue("@eposta", eposta);
    komut.ExecuteNonQuery();

}

static void YolculariListele(SqliteConnection baglanti)
{
    using var komut = new SqliteCommand("SELECT Id, Ad, Soyad FROM Yolcu", baglanti);
    using var okuyucu = komut.ExecuteReader();

    Console.WriteLine();

    while(okuyucu.Read())
    {
        Console.WriteLine($"{okuyucu.GetInt32(0)} - {okuyucu.GetString(1)} {okuyucu.GetString(2)}");
    }
}

static void RezervasyonYap(SqliteConnection baglanti)
{
    YolculariListele(baglanti);
    int yolcuId = SayiOku("Yolcu Id: ");

    using var YolcuKontrol = new SqliteCommand("SELECT COUNT(*) FROM Yolcu WHERE Id = @id", baglanti);
    YolcuKontrol.Parameters.AddWithValue("@id", yolcuId);
    long yolcuSayisi = (long)(YolcuKontrol.ExecuteScalar() ?? 0L);
    if(yolcuSayisi == 0)
    {
        Console.WriteLine("Bu Id'li yolcu yok.");
        return;
    }

    UcuslariListele(baglanti);
    Console.Write("Ucus No: ");
    string ucusNo = Console.ReadLine() ?? "";

    using var UcusBul = new SqliteCommand("SELECT Id FROM Ucus WHERE UcusNo = @no", baglanti);
    UcusBul.Parameters.AddWithValue("@no", ucusNo);
    object? sonuc = UcusBul.ExecuteScalar();
    if(sonuc == null)
    {
        Console.WriteLine("Bu Numaralı bir ucus yok.");
        return;
    }
    long ucusId = (long)sonuc;

    Console.Write("Koltuk Secimi (Örnek: 12A): ");
    string koltuk = (Console.ReadLine() ?? "").ToUpper();

    try
    {
        using var ekle = new SqliteCommand("INSERT INTO Rezervasyon (YolcuId, UcusId, KoltukNo, RezervasyonTarihi) VALUES (@yolcu, @ucus, @koltuk, @tarih)", baglanti);

        ekle.Parameters.AddWithValue("@yolcu", yolcuId);
        ekle.Parameters.AddWithValue("@ucus", ucusId);
        ekle.Parameters.AddWithValue("@koltuk", koltuk);
        ekle.Parameters.AddWithValue("@tarih", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        ekle.ExecuteNonQuery();

        Console.WriteLine("Rezervasyonunuz yapıldı!");
    }
    catch (SqliteException)
    {
        Console.WriteLine("Bu koltuk dolu.");
        
    }

}

static void UcusYolculari(SqliteConnection baglanti)
{
    Console.Write("Ucus No: ");
    string UcusNo = Console.ReadLine() ?? "";

    string sql = @"SELECT r.KoltukNo, y.Ad, y.Soyad
                    FROM Rezervasyon r
                    JOIN Yolcu y ON r.YolcuId = y.Id
                    JOIN Ucus u ON r.UcusId = u.Id
                    WHERE u.UcusNo = @no
                    ORDER BY r.KoltukNo";

    using var komut = new SqliteCommand(sql, baglanti);
    komut.Parameters.AddWithValue("@no", UcusNo);
    using var okuyucu = komut.ExecuteReader();

    int sayac = 0;
    Console.WriteLine();
    
    while( okuyucu.Read())
    {
        Console.WriteLine($"Koltuk {okuyucu.GetString(0)}: {okuyucu.GetString(1)} {okuyucu.GetString(2)}");
        sayac++;
    }

    if (sayac == 0)
    {
        Console.WriteLine("Bu Ucus boş.");
    }
}

static int SayiOku(string mesaj)
{
    while (true)
    {
        Console.Write(mesaj);
        if (int.TryParse(Console.ReadLine(), out int sayi))
        {
            return sayi;
        }
        Console.WriteLine("Lutfen gecerli bir sayi gir!");
    }
}