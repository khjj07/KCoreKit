using UnityEngine;

namespace KCoreKit
{
    public class PanelCanvas : Singleton<PanelCanvas>
    {

        private static PanelWidget _currentPanel;
        private static Transform _previousPanelParent;

        public static void Open(PanelWidget widget)
        {
            _currentPanel?.Hide();
            _currentPanel?.transform.SetParent(_previousPanelParent, true);
            _currentPanel?.OnClose();
            _currentPanel = widget;
            _previousPanelParent = _currentPanel.transform.parent;
            _currentPanel?.Show();
            _currentPanel?.transform.SetParent(GetInstance().transform, true);
            _currentPanel?.OnOpen();
        }

        public static void Close()
        {
            _currentPanel?.Hide();
            _currentPanel?.transform.SetParent(_previousPanelParent, true);
            _currentPanel?.OnClose();
            _currentPanel = null;
        }
    }
}