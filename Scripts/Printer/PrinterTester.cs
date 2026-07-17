
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    
    public class PrinterTester : MonoBehaviour
    {
        [SerializeField] private string text;
 
        private Printer _printer;
        
        [Button]
        public void Run()
        {
            _printer = GetComponent<Printer>();
            _printer.Stop();
            _printer.Setup(text);
            _printer.Print(0,() => { Debug.Log("end"); });
        }
    }
}