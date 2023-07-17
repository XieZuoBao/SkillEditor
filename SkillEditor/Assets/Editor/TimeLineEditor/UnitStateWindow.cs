using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using System.IO;

public class UnitStateWindow : OdinEditorWindow
{
    public static UnitStateWindow win;
    private static UnitStateMap stateMap;
    public static void OpenWindow(UnitStateMap cfg)
    {
        win = GetWindow<UnitStateWindow>();
        win.titleContent = new GUIContent("角色状态数据配置");
        stateMap = cfg;
    }

    protected override void OnGUI()
    {
        base.OnGUI();
        if (GUILayout.Button("保存"))
        {
            byte[] bytes = Sirenix.Serialization.SerializationUtility.SerializeValue(stateMap, DataFormat.JSON);
            string temp = Application.streamingAssetsPath.Replace("StreamingAssets", "") + stateMap.unitName + ".txt";
            File.WriteAllBytes(temp, bytes);
            AssetDatabase.Refresh();
        }
    }

    protected override object GetTarget()
    {
        return stateMap;
    }
}