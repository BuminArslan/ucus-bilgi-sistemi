--Uçuş Bilgi Sistemi: SQLite kullanmaktadır, VS'nin verdiği hatalar normaldir. Veriler örnek amaçlıdır.

CREATE TABLE IF NOT EXISTS Havalimani (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Ad TEXT NOT NULL,
    Sehir TEXT NOT NULL,
    KodIATA TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Ucus (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UcusNo TEXT NOT NULL,
    KalkisHavalimaniId INTEGER NOT NULL,
    VarisHavalimaniId INTEGER NOT NULL,
    KalkisSaati TEXT NOT NULL,
    FOREIGN KEY (KalkisHavalimaniId) REFERENCES Havalimani(Id),
    FOREIGN KEY (VarisHavalimaniId) REFERENCES Havalimani(Id)
);

CREATE TABLE IF NOT EXISTS Yolcu (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Ad TEXT NOT NULL,
    Soyad TEXT NOT NULL,
    Eposta TEXT
);

CREATE TABLE IF NOT EXISTS Rezervasyon (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    YolcuId INTEGER NOT NULL,
    UcusId INTEGER NOT NULL,
    KoltukNo TEXT NOT NULL,
    RezervasyonTarihi TEXT NOT NULL,
    FOREIGN KEY (YolcuId) REFERENCES Yolcu(Id),
    FOREIGN KEY (UcusId) REFERENCES Ucus(Id),
    UNIQUE (UcusId, KoltukNo)
);

INSERT INTO Havalimani (Ad, Sehir, KodIATA) VALUES
('Istanbul Havalimani', 'Istanbul', 'IST'),
('Esenboga Havalimani', 'Ankara', 'ESB'),
('Adnan Menderes Havalimani', 'Izmir', 'ADB');

INSERT INTO Ucus (UcusNo, KalkisHavalimaniId, VarisHavalimaniId, KalkisSaati) VALUES
('TK2100', 1, 2, '2026-12-01 09:30'),
('TK2314', 1, 3, '2026-12-01 14:15');

INSERT INTO Yolcu (Ad, Soyad, Eposta) VALUES
('Ali', 'Yilmaz', 'ali@ornek.com'),
('Ayse', 'Demir', 'ayse@ornek.com');