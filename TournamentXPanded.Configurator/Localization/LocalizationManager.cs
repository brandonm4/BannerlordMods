using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using static TournamentXPanded.Configurator.Localization.CommandLineFunctionality;

namespace TournamentXPanded.Configurator.Localization
{
    public static class LocalizedTextManager
    {
        public const string DefaultEnglishLanguageId = "English";

        public static List<string> LanguageIds;

        private static Dictionary<string, LocalizedTextManager.LocalizedText> _gameTextDictionary;

        static LocalizedTextManager()
        {
            LocalizedTextManager.LanguageIds = new List<string>();
            LocalizedTextManager._gameTextDictionary = new Dictionary<string, LocalizedTextManager.LocalizedText>();
        }

        [CommandLineArgumentFunction("check_for_erros", "localization")]
        public static string CheckValidity(List<string> strings)
        {
            if (File.Exists("faulty_translation_lines.txt"))
            {
                File.Delete("faulty_translation_lines.txt");
            }
            bool flag = false;
            foreach (KeyValuePair<string, LocalizedTextManager.LocalizedText> keyValuePair in LocalizedTextManager._gameTextDictionary)
            {
                string key = keyValuePair.Key;
                flag = keyValuePair.Value.CheckValidity(key) | flag;
            }
            if (!flag)
            {
                return "No errors are found.";
            }
            return "Errors are written into 'faulty_translation_lines.txt' file in the binary folder.";
        }

        internal static void Deserilaize(XmlNode node, string languageId)
        {
            if (node.Attributes == null)
            {
                throw new Exception("Node attributes are null!");
            }
            string value = node.Attributes["id"].Value;
            string str = node.Attributes["text"].Value;
            if (!LocalizedTextManager._gameTextDictionary.ContainsKey(value))
            {
                LocalizedTextManager.LocalizedText localizedText = new LocalizedTextManager.LocalizedText();
                LocalizedTextManager._gameTextDictionary.Add(value, localizedText);
            }
            LocalizedTextManager.LocalizedText.AddTranslation(LocalizedTextManager._gameTextDictionary[value], languageId, str);
        }

        public static string GetTranslatedText(string languageId, string id)
        {
            LocalizedTextManager.LocalizedText localizedText;
            if (!LocalizedTextManager._gameTextDictionary.TryGetValue(id, out localizedText))
            {
                return null;
            }
            return LocalizedTextManager.LocalizedText.GetTranslatedText(languageId, localizedText);
        }

        private static void LoadFromXml(XmlDocument doc)
        {
            //Debug.Print("Loading localized text xml.", 0, Debug.DebugColor.White, 17592186044416L);
            if (doc.ChildNodes.Count <= 1 || doc.ChildNodes[1].Name != "base" || doc.ChildNodes[1].ChildNodes[0].Name != "tags" || doc.ChildNodes[1].ChildNodes[1].Name != "strings")
            {
                throw new Exception("Incorrect XML document format!");
            }
            string value = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].Attributes["language"].Value;
            if (LocalizedTextManager.LanguageIds.IndexOf(value) < 0)
            {
                LocalizedTextManager.LanguageIds.Add(value);
            }
            XmlNode itemOf = null;
            if (doc.ChildNodes[1].ChildNodes[1].Name == "strings")
            {
                itemOf = doc.ChildNodes[1].ChildNodes[1].ChildNodes[0];
            }
            while (itemOf != null)
            {
                if (itemOf.Name == "string" && itemOf.NodeType != XmlNodeType.Comment)
                {
                    LocalizedTextManager.Deserilaize(itemOf, value);
                }
                itemOf = itemOf.NextSibling;
            }
        }

        public static void LoadLocalizationXmls(string basepath)
        {
            LocalizedTextManager.LanguageIds.Clear();
            LocalizedTextManager.LanguageIds.Add("English");
            string[] directories = Directory.GetDirectories(System.IO.Path.Combine(basepath, "Modules", "TournamentsXPanded"), "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < (int)directories.Length; i++)
            {
                string str = String.Concat(directories[i], "/Languages");
                if (Directory.Exists(str))
                {
                    string[] files = Directory.GetFiles(str, "*.xml", SearchOption.AllDirectories);
                    for (int j = 0; j < (int)files.Length; j++)
                    {
                        LocalizedTextManager.LoadLocalizedTexts(files[j]);
                    }
                }
            }
        }

        private static void LoadLocalizedTexts(string xmlPath)
        {
            XmlDocument xmlDocument = LocalizedTextManager.LoadXmlFile(xmlPath);
            if (xmlDocument != null)
            {
                LocalizedTextManager.LoadFromXml(xmlDocument);
            }
        }

        private static XmlDocument LoadXmlFile(string path)
        {
            XmlDocument xmlDocument;
            try
            {
                //Debug.Print(String.Concat("opening ", path), 0, Debug.DebugColor.White, 17592186044416L);
                XmlDocument xmlDocument1 = new XmlDocument();
                xmlDocument1.LoadXml((new StreamReader(path)).ReadToEnd());
                xmlDocument = xmlDocument1;
            }
            catch
            {
                return null;
            }
            return xmlDocument;
        }

        public static string GetStringId(string text)
        {
            if (text.Contains("}"))
            {
                text = text.Substring(2, text.IndexOf('}') - 2);
            }
            return text;
        }
        public static string FindStringId(string languageId, string localizedText)
        {
            string id = localizedText;
            foreach(var key in _gameTextDictionary.Keys)
            {
                if (((LocalizedText)_gameTextDictionary[key])._localizedTextDictionary["English"] == localizedText)
                {
                    id = "{=" + key + "}";
                }
            }
            return id;
        }

        private class LocalizedText
        {
            internal readonly Dictionary<string, string> _localizedTextDictionary;

            public LocalizedText()
            {
                this._localizedTextDictionary = new Dictionary<string, string>();
            }

            public static void AddTranslation(LocalizedTextManager.LocalizedText text, string language, string translation)
            {
                if (text._localizedTextDictionary.ContainsKey(language))
                {
                    return;
                }
                text._localizedTextDictionary.Add(language, translation);
            }

            public bool CheckValidity(string id)
            {
                bool flag = false;
                foreach (KeyValuePair<string, string> keyValuePair in this._localizedTextDictionary)
                {
                    string value = keyValuePair.Value;
                    int num = 0;
                    int num1 = 0;
                    bool flag1 = false;
                    string str = value;
                    for (int i = 0; i < str.Length; i++)
                    {
                        char chr = str[i];
                        if (chr == '{')
                        {
                            num++;
                        }
                        else if (chr == '}')
                        {
                            num1++;
                        }
                        if (num > num1 && chr == ' ')
                        {
                            flag1 = true;
                        }
                    }
                    if (flag1)
                    {
                        string str1 = String.Concat(new String[] { "Text_id:", id, " within language ", keyValuePair.Key, " has empty space inside left and right brackets '{ , }'. Faulty string: ", value, "\n\n" });
                        File.AppendAllText("faulty_translation_lines.txt", str1, Encoding.Unicode);
                        flag = true;
                    }
                    int num2 = 0;
                    int num3 = 0;
                    string str2 = value;
                    while (true)
                    {
                        int num4 = str2.IndexOf("{?");
                        if (num4 == -1)
                        {
                            break;
                        }
                        num4 = Math.Min(num4 + 1, str2.Length - 1);
                        str2 = str2.Substring(num4);
                        if (str2.Length > 2 && str2[1] != '}')
                        {
                            num2++;
                        }
                    }
                    string str3 = value;
                    while (true)
                    {
                        int num5 = str3.IndexOf("{\\?}");
                        if (num5 == -1)
                        {
                            break;
                        }
                        num3++;
                        num5 = Math.Min(num5 + 1, value.Length - 1);
                        str3 = str3.Substring(num5);
                    }
                    if (num != num1)
                    {
                        string str4 = String.Concat(new String[] { "Text_id:", id, " within language ", keyValuePair.Key, " does not have a matching number of left-right brackets. Faulty string: ", value, "\n\n" });
                        File.AppendAllText("faulty_translation_lines.txt", str4, Encoding.Unicode);
                        flag = true;
                    }
                    if (num2 == num3)
                    {
                        continue;
                    }
                    string str5 = String.Concat(new String[] { "Text_id:", id, " within language ", keyValuePair.Key, " have not-matching number of condition starters and enders. Faulty string: ", value, "\n\n" });
                    File.AppendAllText("faulty_translation_lines.txt", str5, Encoding.Unicode);
                    flag = true;
                }
                return flag;
            }

            public static string GetTranslatedText(string languageId, LocalizedTextManager.LocalizedText localizedText)
            {
                string str;
                if (localizedText._localizedTextDictionary.TryGetValue(languageId, out str))
                {
                    return str;
                }
                if (localizedText._localizedTextDictionary.TryGetValue("English", out str))
                {
                    return str;
                }
                return "";
            }
            
           
        }
    }




}