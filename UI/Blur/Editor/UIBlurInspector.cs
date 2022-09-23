using UnityEditor;

namespace Game.UI.Effects
{
    [CustomEditor(typeof(UIBlur))]
    public class UIBlurInspector : Editor
    {
        private UIBlur m_Target;

        SerializedProperty m_BlurColor;
        SerializedProperty m_BlurSize;
        SerializedProperty m_BlurIteration;
        SerializedProperty m_BlurSpread;
        SerializedProperty m_BlurDownSample;

        void OnEnable()
        {
            m_Target = target as UIBlur;

            m_BlurColor = serializedObject.FindProperty("m_BlurColor");
            m_BlurSize = serializedObject.FindProperty("m_BlurSize");
            m_BlurIteration = serializedObject.FindProperty("m_BlurIteration");
            m_BlurSpread = serializedObject.FindProperty("m_BlurSpread");
            m_BlurDownSample = serializedObject.FindProperty("m_BlurDownSample");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_BlurSize);
            if (EditorGUI.EndChangeCheck())
            {
                m_Target.SetBlurImage();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_BlurIteration);
            if (EditorGUI.EndChangeCheck())
            {
                m_Target.SetBlurImage();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_BlurSpread);
            if (EditorGUI.EndChangeCheck())
            {
                m_Target.SetBlurImage();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_BlurDownSample);
            if (EditorGUI.EndChangeCheck())
            {
                m_Target.SetBlurImage();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}