using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace CamSistemWebArayuz
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RunDatabaseMigrations();
        }

        protected void Application_BeginRequest()
        {
            if (Context?.Response != null)
            {
                Context.Response.ContentEncoding = Utf8NoBom;
                Context.Response.Charset = "utf-8";
            }

            if (Context?.Request != null && ShouldForceUtf8(Context.Request.ContentType))
            {
                Context.Request.ContentEncoding = Utf8NoBom;
            }
        }

        protected void Application_PreSendRequestHeaders()
        {
            var response = Context?.Response;
            if (response == null || !ShouldForceUtf8(response.ContentType))
                return;

            response.ContentEncoding = Utf8NoBom;
            response.Charset = "utf-8";

            var contentType = response.ContentType ?? string.Empty;
            if (!HasCharsetParameter(contentType))
            {
                response.ContentType = string.IsNullOrWhiteSpace(contentType)
                    ? "text/html; charset=utf-8"
                    : contentType + "; charset=utf-8";
            }
        }

        private static bool ShouldForceUtf8(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                return false;

            var mediaType = contentType.Split(';')[0].Trim();
            if (mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                return true;

            return mediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase)
                   || mediaType.Equals("application/javascript", StringComparison.OrdinalIgnoreCase)
                   || mediaType.Equals("application/xml", StringComparison.OrdinalIgnoreCase)
                   || mediaType.Equals("application/xhtml+xml", StringComparison.OrdinalIgnoreCase)
                   || mediaType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasCharsetParameter(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                return false;

            var parts = contentType.Split(';');
            for (int i = 1; i < parts.Length; i++)
            {
                var parameter = parts[i].Trim();
                if (parameter.StartsWith("charset=", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private void RunDatabaseMigrations()
        {
            try
            {
                var efConnectionString = ConfigurationManager.ConnectionStrings["CamSistemModel"]?.ConnectionString;
                if (string.IsNullOrEmpty(efConnectionString)) return;

                var builder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(efConnectionString);
                var sqlConnectionString = builder.ProviderConnectionString;

                var migrations = new[]
                {
                    // SiparisEnBoyAdet – per-row system columns
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'SistemId') ALTER TABLE dbo.SiparisEnBoyAdet ADD SistemId int NULL",
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'AltSistemId') ALTER TABLE dbo.SiparisEnBoyAdet ADD AltSistemId int NULL",
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'SistemTurId') ALTER TABLE dbo.SiparisEnBoyAdet ADD SistemTurId int NULL",
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'GirilenKanatAdet') ALTER TABLE dbo.SiparisEnBoyAdet ADD GirilenKanatAdet int NULL",
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'GirilenEn3') ALTER TABLE dbo.SiparisEnBoyAdet ADD GirilenEn3 int NULL",

                    // SiparisCam – glass combination columns
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisCam' AND COLUMN_NAME = 'OnCam') ALTER TABLE dbo.SiparisCam ADD OnCam nvarchar(50) NULL",
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisCam' AND COLUMN_NAME = 'AraBosluk') ALTER TABLE dbo.SiparisCam ADD AraBosluk nvarchar(50) NULL",
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisCam' AND COLUMN_NAME = 'ArkaCam') ALTER TABLE dbo.SiparisCam ADD ArkaCam nvarchar(50) NULL",

                    // CamKombinasyon table
                    @"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CamKombinasyon')
                      CREATE TABLE dbo.CamKombinasyon (
                          Id         int           IDENTITY(1,1) NOT NULL,
                          Kombinasyon nvarchar(250) NULL,
                          Birim      nvarchar(50)  NULL,
                          BirimFiyat decimal(17,2) NULL,
                          CONSTRAINT PK_CamKombinasyon PRIMARY KEY (Id)
                      )",

                    // SiparisTeklif table
                    @"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisTeklif')
                      CREATE TABLE dbo.SiparisTeklif (
                          Id                 bigint        IDENTITY(1,1) NOT NULL,
                          SiparisEnBoyAdetId bigint        NULL,
                          Malzeme            nvarchar(250) NULL,
                          Birim              nvarchar(50)  NULL,
                          Miktar             decimal(17,2) NULL,
                          BirimFiyat         decimal(17,2) NULL,
                          ToplamTutar        decimal(17,2) NULL,
                          KayitTarihi        datetime      NULL,
                          CONSTRAINT PK_SiparisTeklif PRIMARY KEY (Id)
                      )",

                    // SiparisTeklifToplamBilgisi table
                    @"IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisTeklifToplamBilgisi')
                      CREATE TABLE dbo.SiparisTeklifToplamBilgisi (
                          Id                 bigint        IDENTITY(1,1) NOT NULL,
                          SiparisEnBoyAdetId bigint        NULL,
                          ToplamMaliyet      decimal(17,2) NULL,
                          m2                 decimal(17,2) NULL,
                          Teklif             decimal(17,2) NULL,
                          CONSTRAINT PK_SiparisTeklifToplamBilgisi PRIMARY KEY (Id)
                      )",

                    // Sabitler – default cost constants (Id=2..5), combined in one batch
                    @"SET IDENTITY_INSERT dbo.Sabitler ON;
                      IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 2) INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (2, 0);
                      IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 3) INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (3, 0);
                      IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 4) INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (4, 0);
                      IF NOT EXISTS (SELECT 1 FROM dbo.Sabitler WHERE Id = 5) INSERT INTO dbo.Sabitler (Id, SabitDeger) VALUES (5, 0);
                      SET IDENTITY_INSERT dbo.Sabitler OFF;",
                };

                using (var conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();
                    foreach (var sql in migrations)
                    {
                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[DbMigration] Hata: " + ex.Message);
            }
        }
    }
}
