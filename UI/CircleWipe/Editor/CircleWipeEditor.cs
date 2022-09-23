using UnityEngine;
using UnityEditor;

namespace Game.UI.Effects
{
    [CustomEditor(typeof(CircleWipe))]
    public class CircleWipeEditor : Editor
    {
        private CircleWipe m_Target;

        void Awake()
        {
            m_Target = target as CircleWipe;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Please execute in Play Mode", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Fade In", GUILayout.Width(120)))
                {
                    m_Target.FadeIn();
                }

                if (GUILayout.Button("Fade Out", GUILayout.Width(120)))
                {
                    m_Target.FadeOut();
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}