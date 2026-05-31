using System;
using UnityEngine;

namespace Lockpick
{
    [Serializable]
    public struct BigNumber : IComparable<BigNumber>, IEquatable<BigNumber>
    {
        // 유효숫자 (항상 1.0 이상 1000.0 미만으로 유지됨, 단 0 제외)
        public double mantissa; 
    
        // 지수 (1000 단위인 K, M, B 문자에 대응하기 위해 3의 배수로 관리하면 편리합니다)
        public int exponent;

        // 단위 표기용 배열
        private static readonly string[] Units = { 
            "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", 
            "UDc", "DDc", "TDc", "QaDc", "QiDc", "SxDc", "SpDc", "ODc", "NDc", "Vg" 
        };

        #region Constructors
        public BigNumber(double value)
        {
            this.mantissa = value;
            this.exponent = 0;
            Normalize();
        }

        public BigNumber(double mantissa, int exponent)
        {
            this.mantissa = mantissa;
            this.exponent = exponent;
            Normalize();
        }
        #endregion

        // 자릿수를 정형화하는 핵심 메서드
        private void Normalize()
        {
            if (mantissa == 0)
            {
                exponent = 0;
                return;
            }

            // 유효숫자가 1000 이상일 때 (예: 1250 -> 1.25K)
            while (Math.Abs(mantissa) >= 1000.0)
            {
                mantissa /= 1000.0;
                exponent += 3;
            }

            // 유효숫자가 1 미만일 때 (예: 0.005K -> 5)
            while (Math.Abs(mantissa) < 1.0 && mantissa != 0)
            {
                mantissa *= 1000.0;
                exponent -= 3;
            }
        }

        #region Mathematical Operators
        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            if (a.mantissa == 0) return b;
            if (b.mantissa == 0) return a;

            if (a.exponent < b.exponent)
            {
                int expDiff = b.exponent - a.exponent;
                if (expDiff > 15) return b; // 정밀도 한계를 넘어서면 작은 수 무시
                return new BigNumber(a.mantissa / Math.Pow(10, expDiff) + b.mantissa, b.exponent);
            }
            else
            {
                int expDiff = a.exponent - b.exponent;
                if (expDiff > 15) return a; // 정밀도 한계를 넘어서면 작은 수 무시
                return new BigNumber(a.mantissa + b.mantissa / Math.Pow(10, expDiff), a.exponent);
            }
        }

        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            return a + new BigNumber(-b.mantissa, b.exponent);
        }

        public static BigNumber operator *(BigNumber a, BigNumber b)
        {
            return new BigNumber(a.mantissa * b.mantissa, a.exponent + b.exponent);
        }

        public static BigNumber operator /(BigNumber a, BigNumber b)
        {
            if (b.mantissa == 0) throw new DivideByZeroException();
            return new BigNumber(a.mantissa / b.mantissa, a.exponent - b.exponent);
        }
        #endregion

        #region Comparison Operators
        public int CompareTo(BigNumber other)
        {
            if (this.exponent != other.exponent)
                return this.exponent.CompareTo(other.exponent);
            return this.mantissa.CompareTo(other.mantissa);
        }

        public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
        public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
        public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;
        public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;
        public static bool operator ==(BigNumber a, BigNumber b) => a.Equals(b);
        public static bool operator !=(BigNumber a, BigNumber b) => !a.Equals(b);

        public bool Equals(BigNumber other) => this.exponent == other.exponent && Mathf.Approximately((float)this.mantissa, (float)other.mantissa);
        public override bool Equals(object obj) => obj is BigNumber other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(mantissa, exponent);
        #endregion

        #region Conversion & String
        // 기본 double이나 int에서 BigDouble로 편하게 대입하기 위한 암시적 형변환
        public static implicit operator BigNumber(double value) => new BigNumber(value);
        public static implicit operator BigNumber(int value) => new BigNumber(value);

        // UI 출력용 포맷팅
        public override string ToString()
        {
            int unitIndex = exponent / 3;

            if (unitIndex < 0) return "0";

            if (unitIndex < Units.Length)
            {
                // 천 단위 기호가 존재할 때 (예: 123.45M)
                return exponent == 0 ? $"{mantissa:F0}" : $"{mantissa:F2}{Units[unitIndex]}";
            }
            else
            {
                // 준비된 단위를 넘어설 경우 과학적 표기법 전환 (예: 1.23e+72)
                return $"{mantissa:F2}e+{exponent}";
            }
        }
        #endregion
        
        #region String Parsing
    /// <summary>
    /// 단위를 포함한 문자열("1.5K", "1.23e+45" 등)을 BigDouble로 변환합니다.
    /// </summary>
    public static BigNumber Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new BigNumber(0);

        input = input.Trim().Replace(",", ""); // 공백 및 천단위 콤마 제거

        // 1. 과학적 표기법 처리 (예: 1.23e+45 또는 1.23E45)
        if (input.Contains("e") || input.Contains("E"))
        {
            string[] parts = input.Split(new[] { 'e', 'E' });
            if (parts.Length == 2 && double.TryParse(parts[0], out double m))
            {
                // 지수 부분에 '+' 기호가 있다면 제거 후 파싱
                string expStr = parts[1].Replace("+", "");
                if (int.TryParse(expStr, out int e))
                {
                    // 우리가 만든 구조체는 3의 배수 지수(K, M, B...)를 기본으로 하므로 
                    // 정규화(Normalize)를 위해 일반 값으로 환산 후 생성
                    // 단, 지수가 너무 크면 자릿수 보존을 위해 직접 계산 유도 가능
                    return new BigNumber(m * Math.Pow(10, e % 3), e - (e % 3));
                }
            }
            throw new FormatException($"지수 형태의 문자열 파싱 실패: {input}");
        }

        // 2. 일반 알파벳 단위 표기법 처리 (예: 123.45M)
        // 뒤에서부터 문자가 어디까지 단위 배열(Units)에 속하는지 확인
        int letterCount = 0;
        for (int i = input.Length - 1; i >= 0; i--)
        {
            if (char.IsLetter(input[i])) letterCount++;
            else break;
        }

        if (letterCount > 0)
        {
            string numberPart = input.Substring(0, input.Length - letterCount);
            string unitPart = input.Substring(input.Length - letterCount);

            if (double.TryParse(numberPart, out double m))
            {
                // 단위 배열에서 일치하는 인덱스 검색
                int unitIndex = Array.IndexOf(Units, unitPart);
                if (unitIndex != -1)
                {
                    return new BigNumber(m, unitIndex * 3);
                }
                throw new FormatException($"정의되지 않은 단위 기호입니다: {unitPart}");
            }
        }
        else
        {
            // 3. 단위가 없는 순수 숫자 문자열 처리 (예: 1234567)
            if (double.TryParse(input, out double m))
            {
                return new BigNumber(m);
            }
        }

        throw new FormatException($"BigDouble 형식을 파싱할 수 없습니다: {input}");
    }

    /// <summary>
    /// 문자열 변환을 시도하고 성공 여부를 반환합니다. (예외 발생 방지)
    /// </summary>
    public static bool TryParse(string input, out BigNumber result)
    {
        try
        {
            result = Parse(input);
            return true;
        }
        catch
        {
            result = new BigNumber(0);
            return false;
        }
    }
    #endregion
    }
}