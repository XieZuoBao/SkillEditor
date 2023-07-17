using UnityEngine;
using Sirenix.OdinInspector;

public enum CameraClipType 
{ 
    Shake,//屏幕震动
    PostProcess,//后处理
}

public class CameraPlayableClip : PlayableClip
{
    [Header("摄像机处理类型")]
    public CameraClipType clipType;

    [ShowIf("@clipType==CameraClipType.Shake")]
    [Header("震动幅度")]
    public float shakeAmount;

    [FilePath]
    [ShowIf("@clipType==CameraClipType.PostProcess")]
    [Header("shader名字")]
    public string shaderName;

    public override void Init(PlayableTrack track)
    {
        base.Init(track);
    }
#if UNITY_EDITOR

#endif
}