using UnityEngine;
using UnityEditor;

public class AudioPlayableClip : PlayableClip
{
    public AudioClip audioClip;

    [HideInInspector]
    public string audioName;

    public override void Init(PlayableTrack track)
    {
        base.Init(track);

        if (Application.isPlaying)
        {
            //TODO:通过资源管理器去加载出物体 一旦加载出来了 就自动放入对象池 不用临时物体
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (audioName != null)
            {
                audioClip =  AssetDatabase.LoadAssetAtPath<AudioClip>(ResPathTools.audio + audioName+".wav");
            }

        }
#endif
    }

    public override void BeginPlay()
    {
        base.BeginPlay();
        if(audioClip != null)
        {
            
        }
    }
#if UNITY_EDITOR
    public override void Draw(float y, float xOffset)
    {
        if(audioClip != null)
        {
            audioName = audioClip.name;
        }
        base.Draw(y, xOffset);
    }
#endif
}