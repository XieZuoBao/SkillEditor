using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeLine))]
public class TimeLineInseptor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Open Editor"))
        {
            TimeLine temp = target as TimeLine;
            TimeLineWindow.Open(temp, temp.textAsset);
        }
    }
}