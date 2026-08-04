namespace KCoreKit
{
    
    public enum StatModifyType
    {
        Flat,         // 고정 값 가산 (예: 공격력 +10)
        PercentAdd,   // 퍼센트 합연산 (예: 공격력 +10%, +20% -> +30%)
        PercentMult   // 퍼센트 곱연산 (예: 최종 데미지 * 1.1)
    }
    
    public class StatModifier
    {
        public readonly float value;
        public readonly StatModifyType type;
        public readonly int order;
        public readonly object source; // 어떤 아이템/버프에서 왔는지 추적용

        public StatModifier(float value, StatModifyType type, int order, object source)
        {
            this.value = value;
            this.type = type;
            this.order = order;
            this.source = source;
        }

        // 편의를 위한 생성자 오버로드
        public StatModifier(float value, StatModifyType type) : this(value, type, (int)type, null)
        {
        }

        public StatModifier(float value, StatModifyType type, object source) : this(value, type, (int)type, source)
        {
        }
    }
}