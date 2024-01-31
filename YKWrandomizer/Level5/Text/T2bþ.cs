using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YKWrandomizer.Level5.Binary;
using YKWrandomizer.Level5.Text.Logic;
using YKWrandomizer.Level5.Binary.Logic;

namespace YKWrandomizer.Level5.Text
{
    public class T2bþ : CfgBin
    {
        public Dictionary<int, TextConfig> Texts;

        public Dictionary<int, TextConfig> Nouns;

        public T2bþ()
        {
            Texts = new Dictionary<int, TextConfig>();
            Nouns = new Dictionary<int, TextConfig>();
        }

        public T2bþ(Stream stream)
        {
            Open(stream);

            // Get faces
            int[] faces = Entries
                .Where(x => x.GetName() == "TEXT_WASHA_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => Convert.ToInt32(x.Variables[1].Value))
                .ToArray();

            // Get faces configs
            Dictionary<int, int> facesConfig = Entries
                .Where(x => x.GetName() == "TEXT_CONFIG_BEGIN")
                .SelectMany(x => x.Children)
                .ToDictionary(x => Convert.ToInt32(x.Variables[0].Value), y => Convert.ToInt32(y.Variables[2].Value));

            // Get Texts
            Texts = Entries
                .Where(x => x.GetName() == "TEXT_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .GroupBy(
                    x => Convert.ToInt32(x.Variables[0].Value),
                    y =>
                    {
                        int variable0Value = Convert.ToInt32(y.Variables[0].Value);
                        int washaID = -1;
                        List<StringLevel5> strings = new List<StringLevel5>();

                        if (facesConfig.ContainsKey(variable0Value))
                        {
                            int configValue = facesConfig[variable0Value];
                            if (configValue != -1 && configValue < faces.Length)
                            {
                                washaID = faces[configValue];
                            }
                        }

                        strings.Add(new StringLevel5(
                            Convert.ToInt32(y.Variables[1].Value),
                            (y.Variables[2].Value as OffsetTextPair).Text
                        ));

                        return new TextConfig(strings, washaID);
                    }
                )
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var mergedStrings = group.SelectMany(item => item.Strings).ToList();
                        return new TextConfig(mergedStrings, group.First().WashaID);
                    }
                );

            // Get Nouns
            Nouns = Entries
                .Where(x => x.GetName() == "NOUN_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .GroupBy(
                    x => Convert.ToInt32(x.Variables[0].Value),
                    y =>
                    {
                        int variable0Value = Convert.ToInt32(y.Variables[0].Value);
                        int washaID = -1; // Default value
                        List<StringLevel5> strings = new List<StringLevel5>
                        {
                            new StringLevel5(
                                Convert.ToInt32(y.Variables[1].Value),
                                (y.Variables[5].Value as OffsetTextPair).Text
                            )
                        };

                        return new TextConfig(strings, washaID);
                    }
                )
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var mergedStrings = group.SelectMany(item => item.Strings).ToList();
                        return new TextConfig(mergedStrings, group.First().WashaID);
                    }
                );
        }

        public T2bþ(byte[] data)
        {
            Open(data);

            // Get faces
            int[] faces = Entries
                .Where(x => x.GetName() == "TEXT_WASHA_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => Convert.ToInt32(x.Variables[1].Value))
                .ToArray();

            // Get faces configs
            Dictionary<int, int> facesConfig = Entries
                .Where(x => x.GetName() == "TEXT_CONFIG_BEGIN")
                .SelectMany(x => x.Children)
                .ToDictionary(x => Convert.ToInt32(x.Variables[0].Value), y => Convert.ToInt32(y.Variables[2].Value));

            // Get Texts
            Texts = Entries
                .Where(x => x.GetName() == "TEXT_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .GroupBy(
                    x => Convert.ToInt32(x.Variables[0].Value),
                    y =>
                    {
                        int variable0Value = Convert.ToInt32(y.Variables[0].Value);
                        int washaID = -1;
                        List<StringLevel5> strings = new List<StringLevel5>();

                        if (facesConfig.ContainsKey(variable0Value))
                        {
                            int configValue = facesConfig[variable0Value];
                            if (configValue != -1 && configValue < faces.Length)
                            {
                                washaID = faces[configValue];
                            }
                        }

                        strings.Add(new StringLevel5(
                            Convert.ToInt32(y.Variables[1].Value),
                            (y.Variables[2].Value as OffsetTextPair).Text
                        ));

                        return new TextConfig(strings, washaID);
                    }
                )
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var mergedStrings = group.SelectMany(item => item.Strings).ToList();
                        return new TextConfig(mergedStrings, group.First().WashaID);
                    }
                );

            // Get Nouns
            Nouns = Entries
                .Where(x => x.GetName() == "NOUN_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .GroupBy(
                    x => Convert.ToInt32(x.Variables[0].Value),
                    y =>
                    {
                        int variable0Value = Convert.ToInt32(y.Variables[0].Value);
                        int washaID = -1; // Default value
                        List<StringLevel5> strings = new List<StringLevel5>
                        {
                            new StringLevel5(
                                Convert.ToInt32(y.Variables[1].Value),
                                (y.Variables[5].Value as OffsetTextPair).Text
                            )
                        };

                        return new TextConfig(strings, washaID);
                    }
                )
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var mergedStrings = group.SelectMany(item => item.Strings).ToList();
                        return new TextConfig(mergedStrings, group.First().WashaID);
                    }
                );
        }

        public T2bþ(string xmlData) : base()
        {
            Texts = new Dictionary<int, TextConfig>();
            Nouns = new Dictionary<int, TextConfig>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            XmlNodeList textNodes = xmlDoc.SelectNodes("/Texts/TextConfig");

            foreach (XmlNode textNode in textNodes)
            {
                int crc32 = int.Parse(textNode.Attributes.GetNamedItem("crc32").Value.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                int washa = int.Parse(textNode.Attributes.GetNamedItem("washa").Value.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);

                XmlNodeList stringNodes = textNode.SelectNodes("String");

                List<StringLevel5> strings = new List<StringLevel5>();

                for (int i = 0; i < stringNodes.Count; i++)
                {
                    strings.Add(new StringLevel5(i, stringNodes[i].Attributes.GetNamedItem("value").Value));
                }

                // Determine whether it's Texts or Nouns and set washaID accordingly
                if (textNode.ParentNode.Name == "Texts")
                {
                    TextConfig textConfig = new TextConfig(strings, washa);
                    Texts[crc32] = textConfig;
                }
                else if (textNode.ParentNode.Name == "Nouns")
                {
                    // For Nouns, set washaID to -1
                    TextConfig textConfig = new TextConfig(strings, -1);
                    Nouns[crc32] = textConfig;
                }
            }
        }

        public T2bþ(string[] lines)
        {
            Texts = new Dictionary<int, TextConfig>();
            Nouns = new Dictionary<int, TextConfig>();

            int currentIndex = 0;
            TextConfig currentTextConfig = null;

            foreach (string line in lines)
            {
                if (IsRegularFormat(line))
                {
                    Match match = Regex.Match(line, @"\[(\w+)/0x([A-Fa-f0-9]+)/0x([A-Fa-f0-9]+)\]");

                    if (match.Success)
                    {
                        string type = match.Groups[1].Value;
                        int crc32 = int.Parse(match.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
                        int washa = int.Parse(match.Groups[3].Value, System.Globalization.NumberStyles.HexNumber);

                        currentTextConfig = new TextConfig(new List<StringLevel5>(), washa);

                        if (type == "Texts")
                        {
                            Texts[crc32] = currentTextConfig;
                        }
                        else if (type == "Nouns")
                        {
                            Nouns[crc32] = currentTextConfig;
                        }
                    }
                    else
                    {
                        Texts.Add(currentIndex, new TextConfig(new List<StringLevel5>() { new StringLevel5(0, line) }, -1));
                        currentTextConfig = null;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    if (currentTextConfig != null)
                    {
                        currentTextConfig.Strings.Add(new StringLevel5(currentTextConfig.Strings.Count, line));
                    }
                    else
                    {
                        Texts.Add(currentIndex, new TextConfig(new List<StringLevel5>() { new StringLevel5(0, line) }, -1));
                        currentTextConfig = null;
                    }
                }

                currentIndex++;
            }
        }

        private bool IsRegularFormat(string line)
        {
            return Regex.IsMatch(line, @"\[(\w+)/0x([A-Fa-f0-9]+)/0x([A-Fa-f0-9]+)\]");
        }

        private string[] GetStrings()
        {
            string[] allTexts = Texts.Values
                            .SelectMany(textList => textList.Strings)
                            .Select(textValue => textValue.Text)
                            .Distinct()
                            .ToArray();

            string[] allNouns = Nouns.Values
                .SelectMany(textList => textList.Strings)
                .Select(textValue => textValue.Text)
                .Distinct()
                .ToArray();

            return allTexts.Union(allNouns).ToArray();
        }

        private Dictionary<int, string> GetStringsTable()
        {
            Dictionary<int, string> output = new Dictionary<int, string>();

            int offset = 0;
            string[] textsAndNouns = GetStrings();

            foreach (string text in textsAndNouns)
            {
                if (text != null)
                {
                    output.Add(offset, text);
                    offset += Encoding.GetBytes(text).Length + 1;
                }
            }

            return output;
        }

        private Entry GetTextEntry(Dictionary<int, string> strings)
        {
            Entry textEntry = new Entry("TEXT_INFO_BEGIN_0", new List<Variable>() { new Variable(Binary.Logic.Type.Int, Texts.Values.Sum(textList => textList.Strings.Count)) }, Encoding, true);

            foreach (KeyValuePair<int, TextConfig> textItem in Texts)
            {
                for (int i = 0; i < textItem.Value.Strings.Count; i++)
                {
                    StringLevel5 textValue = textItem.Value.Strings[i];

                    Entry textItemEntry = new Entry("TEXT_INFO_" + i, new List<Variable>()
                        {
                            new Variable(Binary.Logic.Type.Int, textItem.Key),
                            new Variable(Binary.Logic.Type.Int, i),
                            new Variable(Binary.Logic.Type.String, new OffsetTextPair(strings.FirstOrDefault(x => x.Value == textValue.Text).Key, textValue.Text)),
                            new Variable(Binary.Logic.Type.Int, 0),
                        }, Encoding
                    );

                    textEntry.Children.Add(textItemEntry);
                }
            }

            return textEntry;
        }

        private Entry GetTextConfigEntry()
        {
            int index = 0;
            List<int> washas = Texts.Where(x => x.Value.WashaID != -1).Select(x => x.Value.WashaID).ToList();
            Entry textConfigEntry = new Entry("TEXT_CONFIG_BEGIN_0", new List<Variable>() { new Variable(Binary.Logic.Type.Int, Texts.Count) }, Encoding, true);

            foreach (KeyValuePair<int, TextConfig> textItem in Texts)
            {
                Entry textConfigItemEntry = new Entry("TEXT_CONFIG_" + index, new List<Variable>()
                        {
                            new Variable(Binary.Logic.Type.Int, textItem.Key),
                            new Variable(Binary.Logic.Type.Int, textItem.Value.Strings.Count),
                            new Variable(Binary.Logic.Type.Int, washas.IndexOf(textItem.Value.WashaID)),
                        }, Encoding
                );

                textConfigEntry.Children.Add(textConfigItemEntry);
                index++;
            }

            return textConfigEntry;
        }

        private Entry GetTextWashaEntry()
        {
            int[] washas = Texts.Where(x => x.Value.WashaID != -1).Select(x => x.Value.WashaID).ToArray();
            Entry textWashaEntry = new Entry("TEXT_WASHA_BEGIN_0", new List<Variable>() { new Variable(Binary.Logic.Type.Int, washas.Length) }, Encoding, true);

            for (int i = 0; i < washas.Length; i++)
            {
                Entry textWashaItem = new Entry("TEXT_WASHA_" + i, new List<Variable>()
                        {
                            new Variable(Binary.Logic.Type.Int, i),
                            new Variable(Binary.Logic.Type.Int, washas[i]),
                        }, Encoding
                );

                textWashaEntry.Children.Add(textWashaItem);
            }

            return textWashaEntry;
        }

        private Entry GetNounEntry(Dictionary<int, string> strings)
        {
            Entry nounEntry = new Entry("NOUN_INFO_BEGIN_0", new List<Variable>() { new Variable(Binary.Logic.Type.Int, Nouns.Values.Sum(textList => textList.Strings.Count)) }, Encoding, true);

            foreach (KeyValuePair<int, TextConfig> nounItem in Nouns)
            {
                for (int i = 0; i < nounItem.Value.Strings.Count; i++)
                {
                    StringLevel5 textValue = nounItem.Value.Strings[i];

                    Entry textItemEntry = new Entry("NOUN_INFO_" + i, new List<Variable>()
                        {
                            new Variable(Binary.Logic.Type.Int, nounItem.Key),
                            new Variable(Binary.Logic.Type.Int, i),
                            new Variable(Binary.Logic.Type.String,  new OffsetTextPair(-1, null)),
                            new Variable(Binary.Logic.Type.String,  new OffsetTextPair(-1, null)),
                            new Variable(Binary.Logic.Type.String,  new OffsetTextPair(-1, null)),
                            new Variable(Binary.Logic.Type.String, new OffsetTextPair(strings.FirstOrDefault(x => x.Value == textValue.Text).Key, textValue.Text)),
                            new Variable(Binary.Logic.Type.String,  new OffsetTextPair(-1, null)),
                            new Variable(Binary.Logic.Type.String,  new OffsetTextPair(-1, null)),
                            new Variable(Binary.Logic.Type.String,  new OffsetTextPair(-1, null)),
                            new Variable(Binary.Logic.Type.String,  new OffsetTextPair(-1, null)),
                            new Variable(Binary.Logic.Type.Int, 0),
                            new Variable(Binary.Logic.Type.Int, 0),
                            new Variable(Binary.Logic.Type.Int, 0),
                            new Variable(Binary.Logic.Type.Int, 0),
                        }, Encoding
                    );

                    nounEntry.Children.Add(textItemEntry);
                }
            }

            return nounEntry;
        }

        public void Save(string fileName, bool iego)
        {
            Dictionary<int, string> strings = GetStringsTable();

            if (Texts.Count > 0)
            {
                Entry textEntry = GetTextEntry(strings);
                ReplaceEntry("TEXT_INFO_BEGIN", textEntry);

                if (iego)
                {
                    Entry configEntry = GetTextConfigEntry();
                    Entry washaEntry = GetTextWashaEntry();

                    ReplaceEntry("TEXT_CONFIG_BEGIN", configEntry);
                    ReplaceEntry("TEXT_WASHA_BEGIN", washaEntry);
                }
            }

            if (Nouns.Count > 0)
            {
                Entry nounEntry = GetNounEntry(strings);
                ReplaceEntry("NOUN_INFO_BEGIN", nounEntry);
            }

            Strings = strings;
            Save(fileName);
        }

        public byte[] Save(bool iego)
        {
            Dictionary<int, string> strings = GetStringsTable();

            if (Texts.Count > 0)
            {
                Entry textEntry = GetTextEntry(strings);
                ReplaceEntry("TEXT_INFO_BEGIN", textEntry);

                if (iego)
                {
                    Entry configEntry = GetTextConfigEntry();
                    Entry washaEntry = GetTextWashaEntry();

                    ReplaceEntry("TEXT_CONFIG_BEGIN", configEntry);
                    ReplaceEntry("TEXT_WASHA_BEGIN", washaEntry);
                }
            }

            if (Nouns.Count > 0)
            {
                Entry nounEntry = GetNounEntry(strings);
                ReplaceEntry("NOUN_INFO_BEGIN", nounEntry);
            }

            Strings = strings;
            return Save();
        }

        public string[] ConvertToXml(Dictionary<int, TextConfig> texts, string baliseName)
        {
            List<string> xmlStrings = new List<string>();

            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.AppendLine("<" + baliseName + ">");

            foreach (var kvp in texts)
            {
                int crc32 = kvp.Key;
                string washa = "0x" + kvp.Value.WashaID.ToString("X8");

                xmlBuilder.AppendLine($" <TextConfig crc32=\"0x{crc32.ToString("X8")}\" washa=\"{washa}\">");

                foreach (var stringLevel5 in kvp.Value.Strings)
                {
                    xmlBuilder.AppendLine($"  <String value=\"{stringLevel5.Text.Replace("<", "&lt;").Replace(">", "V&gt;").Replace("\"", "&quot;")}\" />");
                }

                xmlBuilder.AppendLine(" </TextConfig>");
            }

            xmlBuilder.AppendLine("</" + baliseName + ">");
            xmlStrings.Add(xmlBuilder.ToString());

            return xmlStrings.ToArray();
        }

        public string[] ExportToXML()
        {
            List<string> xmlStrings = new List<string>();

            xmlStrings.Add("<?xml version=\"1.0\"?>");

            if (Texts.Count > 1)
            {
                xmlStrings.AddRange(ConvertToXml(Texts, "Texts"));
            }

            if (Nouns.Count > 1)
            {
                xmlStrings.AddRange(ConvertToXml(Nouns, "Nouns"));
            }

            return xmlStrings.ToArray();
        }

        public string[] ExportToTxt()
        {
            List<string> txtStrings = new List<string>();

            foreach (var kvp in Texts)
            {
                int crc32 = kvp.Key;
                string washa = "0x" + kvp.Value.WashaID.ToString("X8");

                StringBuilder textBuilder = new StringBuilder();

                textBuilder.AppendFormat("[Texts/0x{0:X8}/{1}] {2}", crc32, washa, Environment.NewLine);

                foreach (var stringLevel5 in kvp.Value.Strings)
                {
                    textBuilder.AppendLine(stringLevel5.Text);
                }

                txtStrings.Add(textBuilder.ToString());
            }

            foreach (var kvp in Nouns)
            {
                int crc32 = kvp.Key;

                StringBuilder textBuilder = new StringBuilder();

                textBuilder.AppendFormat("[Nouns/0x{0:X8}/-1] {1}", crc32, Environment.NewLine);

                foreach (var stringLevel5 in kvp.Value.Strings)
                {
                    textBuilder.AppendLine(stringLevel5.Text);
                }

                txtStrings.Add(textBuilder.ToString());
            }

            return txtStrings.ToArray();
        }
    }
}
