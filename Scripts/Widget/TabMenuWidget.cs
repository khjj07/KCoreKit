using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KCoreKit
{
    public class TabMenuWidget : PanelWidget
    {
        [Serializable]
        public class TabBind
        {
            public string action;
            public Button button;   
            public TabWidget widget;
        }
        
        private TabWidget currentTab;
        
        [SerializeField]
        public List<TabBind> tabWidgetBinds = new List<TabBind>();
        private bool switchable = true;
       

        public void Open(TabWidget widget)
        {
            currentTab = widget;
            currentTab?.Show();
        }
        public void Close()
        {
            currentTab?.Hide();
            currentTab = null;
        }
        
        public void OnOpenAction(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                foreach (var bind in tabWidgetBinds)
                {
                    if (context.action.name == bind.action)
                    {
                        PanelCanvas.Open(this);
                        Open(bind.widget);
                        break;
                    }
                }
            }
        }
     
        public bool IsTapOpened()
        {
            return currentTab;
        }


        public override void OnOpen()
        {
            
        }

        public override void OnClose()
        {
            
        }
    }
}