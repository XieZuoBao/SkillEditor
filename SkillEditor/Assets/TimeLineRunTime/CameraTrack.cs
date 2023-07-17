using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : PlayableTrack
{
    [Tooltip("使用此摄像机去播放轨道的CameraClip")]
    [Header("摄像机")]
    public Camera camera;

    public override string name { get { return "CameraTrack"; } }

    public override void AddPlayableClip(Rect rect)
    {
        if (clips == null)
        {
            clips = new List<PlayableClip>();
        }
        PlayableClip playableClip = new CameraPlayableClip();
        playableClip.rectRange = rect;
        playableClip.startTime = rect.x / scale;
        playableClip.endTime = (rect.x + rect.width) / scale;
        playableClip.parentTrack = this;
        clips.Add(playableClip);
    }

    public override void DrawHeader(Rect rect)
    {
        rectHeader = rect;
        if (camera == null)
        {
            GUI.Box(rect, name + "(None)");
        }
        else
        {
            GUI.Box(rect, camera.name);
        }
    }
}