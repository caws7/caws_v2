using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimizasyon
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("\n#############\n");
            Stok stok = new Stok();
            FireStok fire_stok = new FireStok();

            fire_stok.minDeger = 1000;

            //ihtiyacımız olan profiller

            //SİPARİŞ 1
            Profil p1 = new Profil { Boy = 3295 };
            p1.Adet = 70;
            p1.Profil_Kod = "G-101";
            Profil p2 = new Profil { Boy = 1450 };
            p2.Adet = 280;
            p2.Profil_Kod = "G-101";
            Profil p3 = new Profil { Boy = 3295};
            p3.Adet = 70;
            p3.Profil_Kod = "G-105";
            Profil p4 = new Profil { Boy = 2853 };
            p4.Adet = 140;
            p4.Profil_Kod = "G-107";
            Profil p5 = new Profil { Boy = 2853 };
            p5.Adet = 140;
            p5.Profil_Kod = "G-108";
            Profil p6 = new Profil { Boy = 2853 };
            p6.Adet = 140;
            p6.Profil_Kod = "G-109";
            Profil p7 = new Profil { Boy = 3429 };
            p7.Adet = 70;
            p7.Profil_Kod = "SB-101";
            Profil p8 = new Profil { Boy = 3475 };
            p8.Adet = 70;
            p8.Profil_Kod = "SP-101";
            Profil p9 = new Profil { Boy = 3473};
            p9.Adet = 70;
            p9.Profil_Kod = "SP-102";
            Profil p10 = new Profil { Boy = 978 };
            p10.Adet = 140;
            p10.Profil_Kod = "G-106";
            Profil p11 = new Profil { Boy = 977 };
            p11.Adet = 280;
            p11.Profil_Kod = "G-106";
            Profil p12 = new Profil { Boy = 793 };
            p12.Adet = 140;
            p12.Profil_Kod = "G-110";
            Profil p13 = new Profil { Boy = 1952 };
            p13.Adet = 140;
            p13.Profil_Kod = "G-110";

            

            // //SİPARİŞ 2
            // Profil p7 = new Profil { Boy = 2795 };
            // p7.Adet = 10;
            // p7.Profil_Kod = "A";
            // Profil p8 = new Profil { Boy = 546 };
            // p8.Adet = 14;
            // p8.Profil_Kod = "B";
            // Profil p9 = new Profil { Boy = 900};
            // p9.Adet = 17;
            // p9.Profil_Kod = "C";
            // Profil p10 = new Profil { Boy = 1969 };
            // p10.Adet = 8;
            // p10.Profil_Kod = "D";
            // Profil p11 = new Profil { Boy = 1875 };
            // p11.Adet = 6;
            // p11.Profil_Kod = "E";
            // Profil p12 = new Profil { Boy = 2650 };
            // p12.Adet = 4;
            // p12.Profil_Kod = "F";

            //DB den fire stok bilgisi gelecek
            Profil f1 = new Profil { Boy = 2300 };
            f1.Adet = 3;
            f1.Profil_Kod = "A";
            Profil f2 = new Profil { Boy = 2100 };
            f2.Adet = 1;
            f2.Profil_Kod = "B";
            Profil f3 = new Profil { Boy = 3145 };
            f3.Adet = 2;
            f3.Profil_Kod = "C";
            Profil f4 = new Profil { Boy = 1140 };
            f4.Adet = 4;
            f4.Profil_Kod = "D";
            Profil f5 = new Profil { Boy = 1510 };
            f5.Adet = 5;
            f5.Profil_Kod = "E";
            Profil f6 = new Profil { Boy = 1510 };
            f6.Adet = 5;
            f6.Profil_Kod = "A";
            fire_stok.Fireler = new List<Profil>();
            fire_stok.profilEkle(f1);
            fire_stok.profilEkle(f2);
            fire_stok.profilEkle(f3);
            fire_stok.profilEkle(f4);
            fire_stok.profilEkle(f5);
            fire_stok.profilEkle(f6);

            //Buraya stok bilgileri gelecek 
            //Şimdilik dummy profiller oluşturuyoruz.
            Dictionary<int,int> d1 = new Dictionary<int, int>();
            d1.Add(3000,0);
            d1.Add(4000,0);
            d1.Add(5000,0);
            d1.Add(6000,0);
            d1.Add(6500,5000);
            d1.Add(7000,0);

            // Dictionary<int,int> d2 = new Dictionary<int, int>();
            // d2.Add(5371,2);

            //FİRE STOKTAKİLERİ DE EKLEYEREK DENEYELİM
            // foreach(var p in fire_stok.Fireler)
            // {
                
            // }
            
            stok.Stoktaki_Profiller.Add("G-101",d1);
            stok.Stoktaki_Profiller.Add("G-102",d1);
            stok.Stoktaki_Profiller.Add("G-105",d1);
            stok.Stoktaki_Profiller.Add("G-107",d1);
            stok.Stoktaki_Profiller.Add("G-108",d1);
            stok.Stoktaki_Profiller.Add("G-109",d1);
            stok.Stoktaki_Profiller.Add("SB-101",d1);
            stok.Stoktaki_Profiller.Add("SP-101",d1);
            stok.Stoktaki_Profiller.Add("SP-102",d1);
            stok.Stoktaki_Profiller.Add("G-106",d1);
            stok.Stoktaki_Profiller.Add("G-110",d1);

            

            //Optimizasyon
            

            


            //Gelen siparişler optimizasyon içerisinde sırasıyla optimize edilecek.
            Input input = new Input();

            //şimdilik siparişleri manuel oluşturuyorum. 
            Siparis s1 = new Siparis();
            s1.profiller = new List<Profil>{p1,p2,p3,p4,p5,p6,p7,p8,p9,p10,p11,p12,p13};
            s1.siparis_adet = 1;
            s1.siparis_id = 3;
            
            // Siparis s2 = new Siparis();
            // s2.profiller = new List<Profil>{p7,p8,p9,p10,p11,p12};
            // s2.siparis_adet = 2;
            // s2.siparis_id = 4;

            input.Siparisler = new List<Siparis>{s1};
            input.FireStok = fire_stok;
            input.Stok = stok;

            Optimizer optimizer = new Optimizer(input);

            bool fireStokAktif = false;

            double optimizasyonSuresi = 0;

            optimizer.optimizeEt(fireStokAktif);
            

           Console.WriteLine("Geçen Süre : {0} saniye",optimizasyonSuresi);




        }
    }
}
