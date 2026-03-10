#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    [CustomEditor(typeof(PrinterEditor))]
    public class PrinterEditorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
                var editor = (PrinterEditor)target;
                if (GUILayout.Button("Run Printer"))
                {
                    editor.Run();
                }
            }
        }
    }
    
    public class PrinterEditor : MonoBehaviour
    {
        [SerializeField] private string text;

        [SerializeField] private Button playButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private TMP_InputField textInputField;

        private Printer _printer;

        public void Start()
        {
            playButton.onClick.AddListener(OnPlayButtonClick);
            stopButton.onClick.AddListener(OnStopButtonClick);
            textInputField.onValueChanged.AddListener(OnTextChanged);
            textInputField.text = text;
        }

        public void OnPlayButtonClick()
        {
            Run();
        }

        public void OnTextChanged(string input)
        {
            text = input;
        }

        public void OnStopButtonClick()
        {
            _printer.Stop();
        }

        public void Run()
        {
            _printer = GetComponent<Printer>();
            _printer.Stop();
            _printer.PreLoad(text);
            _printer.Print(() => { UnityEngine.Debug.Log("end"); });
        }
    }
}
#endif