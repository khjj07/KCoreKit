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
    
            // 이 부분이 핵심입니다!
            // data.bytes를 직접 쓰지 말고, 유니티가 이미 파싱해둔 .text를 사용하세요.
            string content = data.text; 

            var lines = Regex.Split(content, LINE_SPLIT_RE);
            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
    
            // 첫 번째 헤더의 첫 글자에 BOM이 남아있을 경우를 대비한 방어 코드
            if (header.Length > 0) {
                header[0] = header[0].Trim(new char[] { '\uFEFF', '\u200B' });
            }

            for (var i = 1; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

                var entry = new Dictionary<string, string>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    // trim 매개변수가 ','(쉼표)일 경우 데이터 내 문자가 손상될 수 있으니 주의하세요.
                    value = value.Trim('\"').Replace("\\", ""); 
                    entry[header[j]] = value;
                }
                list.Add(entry);
            }
            return list;
        }
      

    }
}
