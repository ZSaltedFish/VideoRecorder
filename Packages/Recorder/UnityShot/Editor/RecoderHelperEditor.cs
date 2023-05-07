using UnityEditor;
using UnityEngine;
using ZKnight.VideoRecorder.Runtime;

namespace ZKnight.VideoRecorder.Editor
{
    [CustomEditor(typeof(RecoderHelper))]
    public class RecoderHelperEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            string _folder = (target as RecoderHelper).SrcPath;
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            _folder = EditorGUILayout.TextField("Save Folder", _folder);
            if (GUILayout.Button(">>", GUILayout.MaxWidth(30)))
            {
                string path = EditorUtility.SaveFolderPanel("Save Folder", _folder, "");
                if (!string.IsNullOrEmpty(path))
                {
                    _folder = path; 
                    (target as RecoderHelper).SrcPath = _folder;
                }
            }

            EditorGUILayout.EndHorizontal();
            base.OnInspectorGUI();
        }
    }
}