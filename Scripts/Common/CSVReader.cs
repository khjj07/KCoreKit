using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KCoreKit
{
    public class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

        public static List<Dictionary<string, string>> Read(TextAsset data, char trim = ',')
        {
            var list = new List<Dictionary<string, string>>();
            
            string content = data.text; 

            var lines = Regex.Split(content, LINE_SPLIT_RE);
            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            
            // 모든 헤더 요소에서 공백 및 BOM 문자 제거
            for (int h = 0; h < header.Length; h++)
            {
                header[h] = header[h].Trim().Trim(new char[] { '\uFEFF', '\u200B' });
            }

            for (var i = 1; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

                var entry = new Dictionary<string, string>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.Trim('\"').Replace("\\", ""); 
                    entry[header[j]] = value;
                }
                list.Add(entry);
            }
            return list;
        }
      

    }
}