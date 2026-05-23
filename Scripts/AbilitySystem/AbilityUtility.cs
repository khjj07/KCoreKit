namespace KCoreKit
{
    public static class AbilityUtility
    {
        public static string ParseValue(AbilityEffect effect, string key)
        {
            if (effect.provider.HasProperty(key))
            {
                return effect.provider.GetProperty(key);
            }
            return key;
        }
    }
}