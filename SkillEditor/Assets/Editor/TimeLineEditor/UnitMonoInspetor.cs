using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitMono))]
public class UnitMonoInspetor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("配置角色状态表"))
        {
            UnitMono unit = target as UnitMono;
            if(unit.cfg == null)
            {
                unit.stateMap = new UnitStateMap();
                UnitStateWindow.OpenWindow(unit.stateMap);
            }
            else
            {
                unit.stateMap = Sirenix.Serialization.SerializationUtility.DeserializeValue<UnitStateMap>(unit.cfg.bytes, Sirenix.Serialization.DataFormat.JSON);
                UnitStateWindow.OpenWindow(unit.stateMap);
            }
        }
    }
}