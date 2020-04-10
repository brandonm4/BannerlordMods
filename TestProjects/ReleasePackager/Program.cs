using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ReleasePackager
{
    class Program
    {
        private const string c_MODULE_NAME = "BMTournamentXP";
        private const string c_RELEASE_VER = "e1.2.11"; // v is required, major.minor.revision format.
        private const bool c_IS_SINGLEPLAYER_ONLY = true; // if this is false, the module can also be used in multiplayer, leave it false for now.

        private const string c_BIN_DIR = "bin/Win64_Shipping_Client";
        private const string c_SUBMODULE_XML_FILENAME = "SubModule.xml";


        string SourcePath = @"E:\Users\Brandon\OneDrive - Mathis Consulting, LLC\Development\BannerLord\BrandonMods";
        List<string> subModules = new List<string>() { "BMTournamentXP", "BMTournamentPrizes", "NoSpearsInTournaments" };
        static void Main(string[] args)
        {
            
        }


        //Shamelessly stolen from BannerLib
        public string GenerateSubModuleXml()
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = ("    "),
                CloseOutput = true,
                OmitXmlDeclaration = true
            };
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartElement("Module");
                WriteClosedElementWithAttribute(writer, "Name", c_MODULE_NAME);
                WriteClosedElementWithAttribute(writer, "Id", c_MODULE_NAME);
                WriteClosedElementWithAttribute(writer, "Version", c_RELEASE_VER);
                WriteClosedElementWithAttribute(writer, "SingleplayerModule", true.ToString().ToLower());
                WriteClosedElementWithAttribute(writer, "MultiplayerModule", (!c_IS_SINGLEPLAYER_ONLY).ToString().ToLower());
                WriteClosedElement(writer, "DependedModules");
                writer.WriteStartElement("SubModules");
                foreach (var subModDir in subModules)
                {
                    var subModFullName = Path.GetFileName(subModDir);
                    var subModName = subModFullName.Substring(subModFullName.IndexOf('.') + 1);
                    writer.WriteStartElement("SubModule");
                    WriteClosedElementWithAttribute(writer, "Name", subModFullName);
                    WriteClosedElementWithAttribute(writer, "DLLName", $"{subModFullName}.dll");
                    WriteClosedElementWithAttribute(writer, "SubModuleClassType", $"{subModFullName}.{subModName}SubModule");
                    writer.WriteStartElement("Tags");
                    WriteTag(writer, "DedicatedServerType", "none");
                    WriteTag(writer, "IsNoRenderModeElement", "false");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                WriteClosedElement(writer, "Xmls");
                writer.WriteEndDocument();
                writer.Flush();
            }
            // Microsoft insist on XHTML standards for some reason, with no option to change it.
            return sb.ToString().Replace(" />", "/>");
        }
        private void WriteTag(XmlWriter writer, string key, string value)
        {
            writer.WriteStartElement("Tag");
            writer.WriteAttributeString("key", key);
            writer.WriteAttributeString("value", value);
            writer.WriteEndElement();
        }

        private void WriteClosedElementWithAttribute(XmlWriter writer, string elementName, string value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("value", value);
            writer.WriteEndElement();
        }

        private void WriteClosedElement(XmlWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteEndElement();
        }
    }
}
