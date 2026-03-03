using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
    // 이 어트리뷰트가 붙은 필드는 PropertyDrawer가 모든 내용을 표시하도록 합니다.
public class ShowChildrenAttribute : PropertyAttribute 
{
    // 추가적인 로직이 필요 없다면 비워둘 수 있습니다.
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowChildrenAttribute))]
public class ShowChildrenDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 1. 레이블과 인덴트 시작
        // 기본 레이블을 사용하여 필드 이름을 표시합니다.
        EditorGUI.LabelField(position, label);

        // 자식 필드들을 안쪽으로 밀어 넣어 시각적인 구분을 줍니다.
        EditorGUI.indentLevel++; 

        // 2. 자식 필드 순회 및 그리기
        
        // NextVisible(true)를 사용하여 첫 번째 자식 필드를 가져옵니다.
        // (property는 ItemData 타입 자체를 가리킵니다)
        SerializedProperty childProperty = property.Copy();
        bool enterChildren = true;
        
        // property.NextVisible(true)를 사용하여 자식 필드만 순회합니다.
        while (childProperty.NextVisible(enterChildren))
        {
            enterChildren = false;

            // 현재 프로퍼티가 부모 프로퍼티의 자식인지 확인합니다.
            // 즉, 루프가 ItemData 내부의 필드들을 순회하는 동안만 그려야 합니다.
            if (SerializedProperty.EqualContents(childProperty, property))
            {
                // 부모 자체는 건너뛰고 자식 필드부터 시작
                continue; 
            }

            // 부모 프로퍼티의 범위를 벗어나면 루프를 종료합니다.
            if (childProperty.depth > property.depth)
            {
                // 다음 필드가 ItemData 내부 필드라면 그립니다.
                position.y += EditorGUIUtility.singleLineHeight + 10;
                
                // PropertyField를 사용하여 필드를 그립니다. true는 재귀적으로 자식 요소를 표시합니다.
                EditorGUI.PropertyField(position, childProperty, true); 
            }
            else
            {
                // ItemData의 필드가 아닌 다른 필드로 넘어갔다면 루프 종료
                break;
            }
        }
        
        // 3. 인덴트 복원
        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 1. 레이블 높이 (1줄)
        float totalHeight = EditorGUIUtility.singleLineHeight;

        // 2. 모든 자식 필드의 높이를 합산합니다.
        SerializedProperty childProperty = property.Copy();
        bool enterChildren = true;

        while (childProperty.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (SerializedProperty.EqualContents(childProperty, property))
            {
                continue;
            }
            
            if (childProperty.depth > property.depth)
            {
                // 각 자식 필드의 높이를 더합니다.
                totalHeight += EditorGUI.GetPropertyHeight(childProperty, true) + 10;
            }
            else
            {
                break;
            }
        }

        return totalHeight;
    }
}
#endif
}
