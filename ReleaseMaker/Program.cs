using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ReleaseMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "130":
                        CreateVersion130();
                        break;
                    case "120":
                        CreateVersion120();
                        break;
                }
            }
        }

        static void CreateVersion130()
        {
            using var fs = File.Open("TournamentsXPanded-130.zip", FileMode.Create);
            using var za = new ZipArchive(fs, ZipArchiveMode.Update, true, Encoding.UTF8);
            var TXPPath = @"E:\Users\Brandon\OneDrive - Mathis Consulting, LLC\Development\BannerLord\Bannerlord\Modules\TournamentsXPanded";
            var dir = Path.Combine(TXPPath, "bin", "Win64_Shipping_Client");
            za.CreateEntryFromFile(Path.Combine(TXPPath, "SubModule.xml"), "Modules/TournamentsXPanded/SubModule.xml");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;
                if (fi.Name.EndsWith(".exe") || fi.Name.EndsWith(".dll"))
                {
                    using var file = fi.OpenRead();
                    var ze = za.CreateEntry("Modules/TournamentsXPanded/bin/Win64_Shipping_Client/" + fi.Name, CompressionLevel.Optimal);
                    using var zs = ze.Open();
                    file.CopyTo(zs);
                }
            }
            dir = Path.Combine(TXPPath, "ModuleData", "Languages");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;

                using var file = fi.OpenRead();
                var ze = za.CreateEntry("Modules/TournamentsXPanded/ModuleData/Languages/" + fi.Name, CompressionLevel.Optimal);
                using var zs = ze.Open();
                file.CopyTo(zs);
            }
            dir = Path.Combine(TXPPath, "ModuleData", "Languages", "CN");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;

                using var file = fi.OpenRead();
                var ze = za.CreateEntry("Modules/TournamentsXPanded/ModuleData/Languages/CN/" + fi.Name, CompressionLevel.Optimal);
                using var zs = ze.Open();
                file.CopyTo(zs);
            }
            dir = Path.Combine(TXPPath, "ModuleData", "Languages", "RU");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;

                using var file = fi.OpenRead();
                var ze = za.CreateEntry("Modules/TournamentsXPanded/ModuleData/Languages/RU/" + fi.Name, CompressionLevel.Optimal);
                using var zs = ze.Open();
                file.CopyTo(zs);
            }

            //Now for ModLib
            TXPPath = @"E:\Users\Brandon\OneDrive - Mathis Consulting, LLC\Development\BannerLord\Bannerlord\Modules\TournamentsXP.Addon.ModLib";
            dir = Path.Combine(TXPPath, "bin", "Win64_Shipping_Client");
            za.CreateEntryFromFile(Path.Combine(TXPPath, "SubModule.xml"), "Modules/TournamentsXP.Addon.ModLib/SubModule.xml");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;
                if (fi.Name.EndsWith(".exe") || fi.Name.EndsWith(".dll"))
                {
                    using var file = fi.OpenRead();
                    var ze = za.CreateEntry("Modules/TournamentsXP.Addon.ModLib/bin/Win64_Shipping_Client/" + fi.Name, CompressionLevel.Optimal);
                    using var zs = ze.Open();
                    file.CopyTo(zs);
                }
            }
        }
        static void CreateVersion120()
        {
            using var fs = File.Open("TournamentsXPanded-121.zip", FileMode.Create);
            using var za = new ZipArchive(fs, ZipArchiveMode.Update, true, Encoding.UTF8);
            var TXPPath = @"E:\Users\Brandon\OneDrive - Mathis Consulting, LLC\Development\BannerLord\Bannerlord\Modules\TournamentsXPanded";
            var dir = @"E:\Users\Brandon\OneDrive - Mathis Consulting, LLC\Development\BannerLord\BrandonMods\TournamentsXPanded2\bin\x64\RELEASEV121\";
            za.CreateEntryFromFile(Path.Combine(TXPPath, "SubModule.xml"), "Modules/TournamentsXPanded/SubModule.xml");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;

                using var file = fi.OpenRead();
                var ze = za.CreateEntry("Modules/TournamentsXPanded/bin/Win64_Shipping_Client/" + fi.Name, CompressionLevel.Optimal);
                using var zs = ze.Open();
                file.CopyTo(zs);
            }
            dir = Path.Combine(TXPPath, "ModuleData", "Languages");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;

                using var file = fi.OpenRead();
                var ze = za.CreateEntry("Modules/TournamentsXPanded/ModuleData/Languages/" + fi.Name, CompressionLevel.Optimal);
                using var zs = ze.Open();
                file.CopyTo(zs);
            }
            dir = Path.Combine(TXPPath, "ModuleData", "Languages", "CN");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;

                using var file = fi.OpenRead();
                var ze = za.CreateEntry("Modules/TournamentsXPanded/ModuleData/Languages/CN/" + fi.Name, CompressionLevel.Optimal);
                using var zs = ze.Open();
                file.CopyTo(zs);
            }
            dir = Path.Combine(TXPPath, "ModuleData", "Languages", "RU");
            foreach (var filePath in Directory.EnumerateFiles(dir))
            {
                var fi = new FileInfo(filePath);
                if (fi.Name[0] == '.') continue;

                using var file = fi.OpenRead();
                var ze = za.CreateEntry("Modules/TournamentsXPanded/ModuleData/Languages/RU/" + fi.Name, CompressionLevel.Optimal);
                using var zs = ze.Open();
                file.CopyTo(zs);
            }
        }
    }
}
