using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KalymnosBT.Settings
{
    public class LocalSettings
    {
        private const string SettingsTag = "settings";
        private const string OptionTag = "option";
        private const string NameTag = "name";
        private const string ArrayTag = "array";

        private const string PortableTag = "portable";
        private const string SettingsFolder = "settings";
        private string SettingsFile => $"settings-{Environment.MachineName}.conf";

        protected virtual string AppName { get { return "TheApp"; } }
        protected virtual string CompanyName { get { return "OrangeCat Software"; } }

        public string this[string key]
        {
            get
            {
                return GetValue<string>(key);
            }
            set
            {
                SetValue(value, key);
            }
        }

        protected readonly Dictionary<string, string> _settings = new Dictionary<string, string>();

        private string _filename = string.Empty;

        public bool IsPortable { get; private set; }

        public static string GetAppFullPath()
        {
            var p = Process.GetCurrentProcess();
            return p.MainModule.FileName;
        }

        public static string GetAppFileName()
        {
            return Path.GetFileName(GetAppFullPath());
        }

        public string GetStoringFolder(string subdir, bool shouldCreate)
        {
            var p = Process.GetCurrentProcess();
            string fullPath = GetAppFullPath();
            string fileName = GetAppFileName();


            IsPortable = false;
            if (fileName.IndexOf(PortableTag, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                IsPortable = true;
            }

            string pathToFolder;

            if (IsPortable)
            {
                pathToFolder = Path.GetDirectoryName(fullPath);
            }
            else
            {
                //Combine the path with the app name
                pathToFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    CompanyName, AppName);
            }

            Debug.Assert(pathToFolder != null);

            var dir = Path.Combine(pathToFolder ?? string.Empty, subdir);

            if (shouldCreate)
            {
                Directory.CreateDirectory(dir);
            }

            return dir;

        }

        private string GetSettingsFolder()
        {
            var localDir = GetStoringFolder(SettingsFolder, true);
            return Path.Combine(localDir, SettingsFile);
        }

        public LocalSettings()
        {
            _filename = GetSettingsFolder();
        }

        public bool Load()
        {
            bool result = false;
            try
            {

                if (!File.Exists(_filename))
                {
                    // Load default values
                    return true;
                }

                //-----------------------------
                // Open XML file with settings:
                //-----------------------------
                XmlDocument doc = new XmlDocument();
                doc.Load(_filename);

                var nodes = doc.GetElementsByTagName("*");
                foreach (XmlNode node in nodes)
                {
                    var prop = node.Attributes.GetNamedItem(NameTag);
                    var val = node.InnerText;

                    if (prop != null && val != null)
                    {
                        var propStr = prop.InnerText;
                        SetValue(val, propStr);
                    }
                }


                result = true;
            }
            catch (Exception ex)
            {

                result = false;
                Debug.WriteLine("An exception has occured while trying to load setting:\n" + ex.Message);
            }
            return result;
        }

        public bool Save()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                XmlNode root = doc.AppendChild(doc.CreateNode(XmlNodeType.Element, SettingsTag, null));

                foreach (var entry in _settings)
                {
                    if (!string.IsNullOrEmpty(entry.Value))
                    {
                        var option = doc.CreateElement(OptionTag);
                        option.SetAttribute(NameTag, entry.Key);
                        option.InnerText = entry.Value;
                        root.AppendChild(option);
                    }
                }
                doc.Save(_filename);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An exception has occured while trying to convert value:\n" + ex.Message);
            }
            return false;

        }

        public void SetValue<T>(T value, [CallerMemberName] string key = null)
        {
            _settings[key] = Convert.ToString(value);
        }

        public T GetValue<T>(T defaultValue = default(T), [CallerMemberName] string key = null)
        {
            try
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(_settings[key]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An exception has occured while trying to convert value:\n" + ex.Message);
            }

            return defaultValue;
        }

        public void SetIntArray<T>(List<T> array, string key)
        {
            _settings[key] = Convert.ToString(string.Join(";", array));
        }

        public List<T> GetIntArray<T>(string key)
        {
            string value;
            if (!_settings.TryGetValue(key, out value))
            {
                return new List<T>();
            }

            return value.Split(';').Select(val => (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(val)).ToList();
        }
    }
}
