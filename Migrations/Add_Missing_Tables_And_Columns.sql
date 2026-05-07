-- =====================================================================
-- Kapsamlı Migrasyon: Eksik Tablo ve Sütunlar
-- Açıklama: Bu script, EF modelinde tanımlanmış ancak henüz veritabanında
--           oluşturulmamış tablo ve sütunları idempotent (IF NOT EXISTS)
--           şekilde ekler. Her dağıtımda güvenle çalıştırılabilir.
--
-- NASIL ÇALIŞTIRILIR:
--   1. SQL Server Management Studio (SSMS) açın.
--   2. Üst menüden Dosya > Aç > Dosya... seçin ve bu .sql dosyasını açın.
--      VEYA bu dosyanın içeriğini kopyalayıp Yeni Sorgu penceresine yapıştırın.
--   3. Sağ üstteki açılır listeden hedef veritabanınızı (cws_ vb.) seçin.
--   4. F5 tuşuna basın veya "Yürüt" düğmesine tıklayın.
-- =====================================================================

-- -----------------------------------------------------------------------
-- 1. SiparisEnBoyAdet tablosuna eksik sütunlar
-- -----------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisEnBoyAdet' AND COLUMN_NAME='SistemId')
    ALTER TABLE dbo.SiparisEnBoyAdet ADD SistemId int NULL;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisEnBoyAdet' AND COLUMN_NAME='AltSistemId')
    ALTER TABLE dbo.SiparisEnBoyAdet ADD AltSistemId int NULL;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisEnBoyAdet' AND COLUMN_NAME='SistemTurId')
    ALTER TABLE dbo.SiparisEnBoyAdet ADD SistemTurId int NULL;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisEnBoyAdet' AND COLUMN_NAME='GirilenKanatAdet')
    ALTER TABLE dbo.SiparisEnBoyAdet ADD GirilenKanatAdet int NULL;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisEnBoyAdet' AND COLUMN_NAME='GirilenEn3')
    ALTER TABLE dbo.SiparisEnBoyAdet ADD GirilenEn3 int NULL;

-- -----------------------------------------------------------------------
-- 2. SiparisCam tablosuna eksik sütunlar
-- -----------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisCam' AND COLUMN_NAME='OnCam')
    ALTER TABLE dbo.SiparisCam ADD OnCam nvarchar(50) NULL;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisCam' AND COLUMN_NAME='AraBosluk')
    ALTER TABLE dbo.SiparisCam ADD AraBosluk nvarchar(50) NULL;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisCam' AND COLUMN_NAME='ArkaCam')
    ALTER TABLE dbo.SiparisCam ADD ArkaCam nvarchar(50) NULL;

-- -----------------------------------------------------------------------
-- 3. CamKombinasyon tablosu (cam kombinasyon fiyatları)
-- -----------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='CamKombinasyon')
BEGIN
    CREATE TABLE dbo.CamKombinasyon (
        Id          int           IDENTITY(1,1) NOT NULL,
        Kombinasyon nvarchar(250) NULL,
        Birim       nvarchar(50)  NULL,
        BirimFiyat  decimal(17,2) NULL,
        CONSTRAINT PK_CamKombinasyon PRIMARY KEY (Id)
    );
END;

-- -----------------------------------------------------------------------
-- 4. SiparisTeklif tablosu (siparis kalemlerine ait maliyet satırları)
-- -----------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisTeklif')
BEGIN
    CREATE TABLE dbo.SiparisTeklif (
        Id                 bigint IDENTITY(1,1) NOT NULL,
        SiparisEnBoyAdetId bigint        NULL,
        Malzeme            nvarchar(250) NULL,
        Birim              nvarchar(50)  NULL,
        Miktar             decimal(17,2) NULL,
        BirimFiyat         decimal(17,2) NULL,
        ToplamTutar        decimal(17,2) NULL,
        KayitTarihi        datetime      NULL,
        CONSTRAINT PK_SiparisTeklif PRIMARY KEY (Id)
    );
END;

-- -----------------------------------------------------------------------
-- 5. SiparisTeklifToplamBilgisi tablosu (siparis kalemi toplam tutarları)
-- -----------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SiparisTeklifToplamBilgisi')
BEGIN
    CREATE TABLE dbo.SiparisTeklifToplamBilgisi (
        Id                 bigint IDENTITY(1,1) NOT NULL,
        SiparisEnBoyAdetId bigint        NULL,
        ToplamMaliyet      decimal(17,2) NULL,
        m2                 decimal(17,2) NULL,
        Teklif             decimal(17,2) NULL,
        CONSTRAINT PK_SiparisTeklifToplamBilgisi PRIMARY KEY (Id)
    );
END;

-- -----------------------------------------------------------------------
-- 6. Sabitler tablosuna maliyet hesaplama için gerekli varsayılan kayıtlar
--    Id=2 -> Alüminyum kg fiyatı (kuruş cinsinden, /100 ile TL'ye çevrilir)
--    Id=8 -> Cam birim fiyatı (kuruş cinsinden, /100 ile TL'ye çevrilir)
--    Id=9 -> Aksesuar seti birim fiyatı (kuruş cinsinden, /100 ile TL'ye çevrilir)
--    Id=3 -> İmalat bedeli (kuruş/m2, /100 ile TL'ye çevrilir)
--    Id=4 -> Sarf malzeme bedeli (kuruş/m2, /100 ile TL'ye çevrilir)
--    Id=5 -> Kar payı yüzdesi
--    Id=10 -> Kar payı birim fiyatı (kuruş cinsinden, /100 ile TL'ye çevrilir)
--    NOT: Varsayılan değerler 0 olarak eklenmiştir. Uygulamanın doğru maliyet
--         hesaplaması yapabilmesi için bu değerlerin uygulama içindeki
--         "Tanımlamalar > Sabitler" ekranından güncellenmesi GEREKMEKTEDİR.
-- -----------------------------------------------------------------------
SET IDENTITY_INSERT dbo.Sabitler ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 2)
    INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (2, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 3)
    INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (3, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 4)
    INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (4, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 5)
    INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (5, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 8)
    INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (8, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 9)
    INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (9, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 10)
    INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (10, 0);

SET IDENTITY_INSERT dbo.Sabitler OFF;
