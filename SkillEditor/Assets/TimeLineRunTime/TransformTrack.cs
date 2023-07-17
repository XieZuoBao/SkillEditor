using System.Collections.Generic;
using UnityEngine;

public class TransformTrack : PlayableTrack
{
    [Header("位移的基础物体")]
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
                    Debug.LogWarning($"状态{parentSequnce.name}中存在TransformTrack基础物体s 为NULL！请检查配置!!!");
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
    { get { return "Transform Track"; } }

    public override void AddPlayableClip(Rect rect)
    {
        if (clips == null)
        {
            clips = new List<PlayableClip>();
        }
        TransformPlayableClip playableClip = new TransformPlayableClip();
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
            GUI.Box(rect, go.name + "(Transform)");
        }
    }
#endif
}