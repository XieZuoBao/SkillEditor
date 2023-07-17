using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飞行物轨道 用来甩剑气的
/// </summary>
public class FlyItemTrack : PlayableTrack
{
    [Header("基础物体范围")]
    public GameObject go;

    [HideInInspector]
    public string goName;

    public override void Init(Sequnce parent)
    {
        parentSequnce = parent;
        //编辑下
        if (parent.mUnit == null && !Application.isPlaying)
        {
            GameObject temp = GameObject.Find(goName);
            if (temp != null)
            {
                go = temp;
            }
        }
        else
        {
            if (parent.mUnit.gameObject.name == goName)
            {
                go = parent.mUnit.gameObject;
            }
            else
            {
                GameObject temp = GameObject.Find(goName);
                if (temp != null)
                {
                    go = temp;
                }
                else
                {
                    Debug.LogWarning($"状态{parentSequnce.name}中存在FlyItemTrack基础物体s 为NULL！请检查配置!!!");
                }
            }
        }

        foreach (var clip in clips)
        {
            clip.Init(this);
        }
    }

#if UNITY_EDITOR
    public override string name
    { get { return "FlyItem Track"; } }

    public override void AddPlayableClip(Rect rect)
    {
        if (clips == null)
        {
            clips = new List<PlayableClip>();
        }
        FlyItemPlayableClip playableClip = new FlyItemPlayableClip();
        playableClip.rectRange = rect;
        playableClip.startTime = rect.x / scale;
        playableClip.endTime = (rect.x + rect.width) / scale;
        playableClip.parentTrack = this;
        clips.Add(playableClip);
    }

    public override void DrawHeader(Rect rect)
    {
        rectHeader = rect;
        if (go == null)
        {
            GUI.Box(rect, name + "(None)");
        }
        else
        {
            goName = go.name;
            GUI.Box(rect, go.name + "(Effect)");
        }
    }
#endif
}