using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Routing;

namespace CamSistemWebArayuz
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RunDatabaseMigrations();
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
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'SistemId') ALTER TABLE dbo.SiparisEnBoyAdet ADD SistemId int NULL",
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'AltSistemId') ALTER TABLE dbo.SiparisEnBoyAdet ADD AltSistemId int NULL",
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SiparisEnBoyAdet' AND COLUMN_NAME = 'SistemTurId') ALTER TABLE dbo.SiparisEnBoyAdet ADD SistemTurId int NULL"
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