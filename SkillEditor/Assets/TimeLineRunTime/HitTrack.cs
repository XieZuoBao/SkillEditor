using System.Collections.Generic;
using UnityEngine;

public class HitTrack : PlayableTrack
{
    [Header("范围检测基础物体")]
    public GameObject go;
#if UNITY_EDITOR
    public override string name { get { return "Hit Track"; } }

    public override void AddPlayableClip(Rect rect)
    {
        if (clips == null)
        {
            clips = new List<PlayableClip>();
        }
        HitPlayableClip playableClip = new HitPlayableClip();
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
            GUI.Box(rect, go.name + "(Hit)");
        }
    }
#endif
}