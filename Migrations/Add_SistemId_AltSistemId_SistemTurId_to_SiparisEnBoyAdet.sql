-- Migration: Add per-row system columns to SiparisEnBoyAdet
-- Run once on the target database

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'SistemId')
    ALTER TABLE dbo.SiparisEnBoyAdet ADD SistemId int NULL;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'AltSistemId')
    ALTER TABLE dbo.SiparisEnBoyAdet ADD AltSistemId int NULL;

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'SistemTurId')
    ALTER TABLE dbo.SiparisEnBoyAdet ADD SistemTurId int NULL;
