using UnityEngine;
using Sirenix.OdinInspector.Editor;

public class InSpectorWindow : OdinEditorWindow
{
    public static InSpectorWindow win;
    public static void OpenWindow()
    {
        win = GetWindow<InSpectorWindow>();
        win.titleContent = new GUIContent("数据窗口");
    }

    protected override object GetTarget()
    {
        TimeLineWindow.willRepaint = true;
        return TimeLineWindow.select;
    }
}
