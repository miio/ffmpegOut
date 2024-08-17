using UnityEngine;
using UnityEditor;

namespace FFmpegOut
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraCapture))]
    public class CameraCaptureEditor : Editor
    {
        SerializedProperty _setResolution;
        SerializedProperty _width;
        SerializedProperty _height;
        SerializedProperty _frameRate;
        SerializedProperty _allowSlowDown;
        SerializedProperty _codec;
        SerializedProperty _startTime;
        SerializedProperty _recordLength;

        static GUIContent [] _codecLabels = {
            new GUIContent("ProRes (QuickTime)"),
            new GUIContent("H.264 (MP4)"),
            new GUIContent("VP8 (WebM)"),
            new GUIContent("h264_nvenc (MP4)"),
            new GUIContent("hevc_nvenc (MP4)"),
            new GUIContent("h264_videotoolbox (MP4)"),
            new GUIContent("hevc_videotoolbox (MP4)"),
            new GUIContent("UDP localhost:5000 h264_nvenc"),
            new GUIContent("UDP localhost:5000 hevc_nvenc"),
            new GUIContent("UDP localhost:5000 h264_videotoolbox"),
            new GUIContent("UDP localhost:5000 hevc_videotoolbox")
        };

        static int [] _codecOptions = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        void OnEnable()
        {
            _setResolution = serializedObject.FindProperty("_setResolution");
            _width = serializedObject.FindProperty("_width");
            _height = serializedObject.FindProperty("_height");
            _frameRate = serializedObject.FindProperty("_frameRate");
            _allowSlowDown = serializedObject.FindProperty("_allowSlowDown");
            _codec = serializedObject.FindProperty("_codec");
            _startTime = serializedObject.FindProperty("_startTime");
            _recordLength = serializedObject.FindProperty("_recordLength");
        }

        public override void OnInspectorGUI()
        {
            // Show the editor controls.
            serializedObject.Update();

            EditorGUILayout.PropertyField(_setResolution);

            if (_setResolution.hasMultipleDifferentValues || _setResolution.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_width);
                EditorGUILayout.PropertyField(_height);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(_frameRate);
            EditorGUILayout.PropertyField(_allowSlowDown);
            EditorGUILayout.IntPopup(_codec, _codecLabels, _codecOptions);
            EditorGUILayout.PropertyField(_startTime);
            EditorGUILayout.PropertyField(_recordLength);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
