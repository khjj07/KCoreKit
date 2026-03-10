using System;
using System.Linq;

using UnityEngine;

namespace KCoreKit
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;

    public class RowScriptSelectorDropdown : AdvancedDropdown
    {
        private Action<MonoScript> _onSelected;

        public RowScriptSelectorDropdown(AdvancedDropdownState state, Action<MonoScript> onSelected) : base(state)
        {
            _onSelected = onSelected;
            this.minimumSize = new Vector2(250, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Select Row Script");
            
            var types = TypeCache.GetTypesDerivedFrom<DataTableRowBase>()
                .Where(t => !t.IsAbstract && !t.IsInterface);

            foreach (var type in types)
            {
                var assetGuids = AssetDatabase.FindAssets($"{type.Name} t:MonoScript");
                foreach (var guid in assetGuids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    
                    if (script != null && script.GetClass() == type)
                    {
                        root.AddChild(new ScriptItem(type.Name, script));
                        break; 
                    }
                }
            }

            if (!root.children.Any())
                root.AddChild(new AdvancedDropdownItem("No DataTableRowBase found"));

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is ScriptItem scriptItem)
            {
                _onSelected?.Invoke(scriptItem.Script);
            }
        }
        
        private class ScriptItem : AdvancedDropdownItem
        {
            public MonoScript Script { get; }
            public ScriptItem(string name, MonoScript script) : base(name)
            {
                Script = script;
            }
        }
    }
#endif
}