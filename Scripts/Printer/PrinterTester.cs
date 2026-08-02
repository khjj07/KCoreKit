
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
        public void Setup()
        {
            _printer = GetComponent<Printer>();
            _printer.Setup(text);
        }  
        
        
        [Button]
        public void Print()
        {
            _printer.Print(0,() => { Debug.Log("end"); });
        }

        [Button]
        public void Stop()
        {
            _printer.Stop();
        }
    }
}