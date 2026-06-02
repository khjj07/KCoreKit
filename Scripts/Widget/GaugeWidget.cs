using DG.Tweening;

namespace KCoreKit
{
    public class GaugeWidget : ImageWidget
    {
        public float maxValue;
        public float currentValue;
        public float changeDuration = 0.1f;

        public void Setup(float maxValue, float currentValue = 0)
        {
            this.maxValue = maxValue;
            this.currentValue = currentValue;
            image.fillAmount = currentValue / maxValue;
        }

        public void OnChange(float value)
        {
            currentValue = value;
            DOTween.To(() => image.fillAmount, x => image.fillAmount = x, currentValue / maxValue, changeDuration);
        }
    }
}