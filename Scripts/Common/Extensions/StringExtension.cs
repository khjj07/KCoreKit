using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KCoreKit
{
    public static class StringExtension
    {
        public static List<string> ParseStringList(this string input, char separator = '|')
        {
            Debug.Log(input);
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            if (!input.Contains(separator))
            {
                var list = new List<string> { input };
                return list;
            }
            
            return input.Split(separator)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }
        
        
        // 문자열 최대 바이트 수에 맞게 자르기
        public static string TruncateByBytes(this string text, int maxBytes)
        {
            if (string.IsNullOrEmpty(text) || maxBytes <= 0)
                return string.Empty;    
            
            int currentBytes = 0;
            int validLength = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int charBytes = (c >= 0 && c <= 127 ) ? 1 : 2;
                
                if (currentBytes + charBytes > maxBytes)
                    break;
                
                currentBytes += charBytes;
                validLength++;
            }
            
            return text.Substring(0, validLength);
        }
    }
}