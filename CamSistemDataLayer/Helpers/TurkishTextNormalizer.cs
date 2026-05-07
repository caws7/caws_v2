using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CamSistemDataLayer.Helpers
{
    public static class TurkishTextNormalizer
    {
        private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "siparis listesi", "Sipariş Listesi" },
            { "siparis", "Sipariş" },
            { "cagri", "Çağrı" },
            { "imalat", "İmalat" },
            { "maliyet analiz", "Maliyet Analiz" },
            { "maliyet", "Maliyet" },
            { "kullanici", "Kullanıcı" },
            { "musteri", "Müşteri" },
            { "tedarikci", "Tedarikçi" },
            { "tedarik", "Tedarik" },
            { "yonetimi", "Yönetimi" },
            { "yonetim", "Yönetim" },
            { "olculeri", "Ölçüleri" },
            { "olculer", "Ölçüler" },
            { "olcu", "Ölçü" },
            { "optimizasyon", "Optimizasyon" },
            { "fiyatlandirma", "Fiyatlandırma" },
            { "tanimlamalar", "Tanımlamalar" },
            { "tanimlama", "Tanımlama" },
            { "goruntuleme", "Görüntüleme" },
            { "guncelleme", "Güncelleme" },
            { "duzenleme", "Düzenleme" },
            { "yenikayit", "Yeni Kayıt" },
            { "silme", "Silme" },
            { "sevkiyat", "Sevkiyat" },
            { "gonderim", "Gönderim" },
            { "gonderildi", "Gönderildi" },
            { "hazir", "Hazır" },
            { "camli", "Camlı" },
            { "isi", "Isı" },
            { "aluminyum", "Alüminyum" },
            { "aksesuar seti", "Aksesuar Seti" },
            { "sarf malzeme bedeli", "Sarf Malzeme Bedeli" },
            { "rol adi", "Rol Adı" },
            { "rol", "Rol" },
            { "admin", "Admin" }
        };

        public static string NormalizeDisplayText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var normalized = FoldForMatching(text);
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();

            if (ShouldHumanize(normalized))
            {
                normalized = HumanizeWords(normalized);
            }

            foreach (var replacement in Replacements.OrderByDescending(x => x.Key.Length))
            {
                normalized = Regex.Replace(
                    normalized,
                    $@"(?<!\p{{L}}){Regex.Escape(replacement.Key)}(?!\p{{L}})",
                    replacement.Value,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }

            return normalized;
        }

        private static bool ShouldHumanize(string text)
        {
            if (text.IndexOf('_') >= 0)
            {
                return true;
            }

            var letters = text.Where(char.IsLetter).ToList();
            if (!letters.Any())
            {
                return false;
            }

            return letters.Count(char.IsUpper) >= Math.Ceiling(letters.Count * 0.6m);
        }

        private static string FoldForMatching(string text)
        {
            var decomposed = text.Replace("_", " ").Normalize(NormalizationForm.FormD);
            var chars = decomposed
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Select(NormalizeLegacyTurkishChar);

            return new string(chars.ToArray()).Normalize(NormalizationForm.FormC);
        }

        private static string HumanizeWords(string text)
        {
            var builder = new StringBuilder(text.Length);
            var startOfWord = true;

            foreach (var character in text)
            {
                if (!char.IsLetter(character))
                {
                    builder.Append(character);
                    startOfWord = true;
                    continue;
                }

                builder.Append(startOfWord ? char.ToUpperInvariant(character) : char.ToLowerInvariant(character));
                startOfWord = false;
            }

            return builder.ToString();
        }

        private static char NormalizeLegacyTurkishChar(char character)
        {
            if (character == 'ı')
            {
                return 'i';
            }

            if (character == 'İ')
            {
                return 'I';
            }

            return character;
        }
    }
}
