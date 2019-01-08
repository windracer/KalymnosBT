using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KalymnosBT.MVVMBase;
using KalymnosBT.Settings;
using Newtonsoft.Json;

namespace KalymnosBT.Models
{
    static class KalymnosDbHelper
    {
        private static int LastDbRevission => 1;

        private static string DbFolder { get; } = DesktopSettings.Default.GetStoringFolder("data", true);
        public static string DbFile => Path.Combine(DbFolder, "kalymnos.bt");
        private static string DbBackupFile => Path.Combine(DbFolder, "kalymnos.bak");

        public static void Save(KalymnosDb db)
        {
            var json = JsonConvert.SerializeObject(db, 
                Formatting.Indented, 
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });
            
            if (!Directory.Exists(DbFolder))
            {
                Directory.CreateDirectory(DbFolder);
            }

            if (File.Exists(DbFile))
            {
                //------------------------------------
                // Make a permanent backup once a day:
                //------------------------------------
                var backupFolder = Path.Combine(DbFolder, "backups");
                if (!Directory.Exists((backupFolder)))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                //-----------------------------------------------
                // If backup file does not exists yet, create it:
                //-----------------------------------------------
                var backupFile = Path.Combine(backupFolder, $"kalymnos-{DateTime.Now:yyyy-MM-dd}.bt");
                if (!File.Exists(backupFile))
                {
                    File.Copy(DbFile, backupFile, false);
                }

                if (File.Exists(DbBackupFile))
                {
                    File.Delete(DbBackupFile);
                }
                File.Move(DbFile, DbBackupFile);
            }

            using (var stream = File.Open(DbFile, FileMode.Create))
            {
                using (var archive = new GZipStream(stream, CompressionLevel.Optimal))
                {
                    var buf = Encoding.UTF8.GetBytes(json);
                    archive.Write(buf, 0, buf.Length);
                }
            }
        }

        public static KalymnosDb Load()
        {
            if (!File.Exists(DbFile))
            {
                return DefaultDb();
            }

            KalymnosDb db = null;

            try
            {
                using (var stream = File.Open(DbFile, FileMode.Open))
                {
                    using (var archive = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        using (var memory = new MemoryStream())
                        {
                            archive.CopyTo(memory);
                            db = JsonConvert.DeserializeObject<KalymnosDb>(Encoding.UTF8.GetString(memory.GetBuffer()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}\n\nUsing new default DB instead");
                db = DefaultDb();
            }

            return db;
        }

        public static KalymnosDb DefaultDb()
        {
            var db = new KalymnosDb{Revission = 1};
            var projects = new List<Project>
            {
                new Project
                {
                    Name = "Android App",
                    Prefix = "ANDR",
                    LastIssueNumber = 703,
                    Issues =
                    {
                        new Issue {IssueId = 42, Title = "Make a smooth navigation between pages", Details = "Read about animation", Starred = true},
                        new Issue {IssueId = 703, Title = "Add About page", Status = IssueStatus.Closed},
                        new Issue
                        {
                            IssueId = 225,
                            Important = true,
                            Title = "Hangs on saving to file",
                            Details = "Sometimes writing to the file keeps up to 30 seconds",
                            Comments = { new Comment {Text = "I could repeat that on emulator an"}},
                            Votes = { new Vote {Name = "MR", Email = "mr@yahoo.com", When = DateTime.UtcNow}}
                        }
                    }
                },
                new Project
                {
                    Name = "iPhone App",
                    Prefix = "IOS",
                    LastIssueNumber = 0
                }
            };
            projects.ForEach(p =>
            {
                foreach (var issue in p.Issues)
                {
                    issue.Project = p;
                }
                db.Projects.Add(p);
            });
            return db;
        }


    }

    public class KalymnosDb : ObservableObject
    {
        private int _revision;
        public int Revission { get => _revision; set => SetProperty(ref _revision, value); }
        public ObservableCollection<Project> Projects { get; } = new ObservableCollection<Project>();
        public ObservableCollection<Tuple<Project, DateTime>> Trash { get; } = new ObservableCollection<Tuple<Project, DateTime>>();
    }
}
