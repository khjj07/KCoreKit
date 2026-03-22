using UnityEngine;

namespace KCoreKit
{
    public class PrinterTest : MonoBehaviour
    {
        [BigHeader("테스트용 텍스트")]
        public string text;

        [Button("텍스트 재생")]
        public void Play()
        {
            var printer = GetComponent<Printer>();
            printer.PreLoad(text);
            printer.Print();
        }
    }
}