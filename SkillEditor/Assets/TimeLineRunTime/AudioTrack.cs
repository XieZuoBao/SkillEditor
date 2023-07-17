using System.Collections.Generic;
using UnityEngine;

public class AudioTrack : PlayableTrack
{
    [Tooltip("选择音源")]
    public AudioSource audioSource;

    [HideInInspector]
    public string audioName;

    public override void Init(Sequnce parent)
    {
        parentSequnce = parent;
        //编辑下
        if (parent.mUnit == null && !Application.isPlaying)
        {
            GameObject temp = GameObject.Find(audioName);
            if (temp != null)
            {
                audioSource = temp.GetComponent<AudioSource>();
            }
        }
        else
        {
            if (parent.mUnit.gameObject.name == audioName)
            {
                audioSource = parent.mUnit.gameObject.GetComponent<AudioSource>();
            }
            else
            {
                GameObject temp = GameObject.Find(audioName);
                if (temp != null)
                {
                    audioSource = temp.GetComponent<AudioSource>();
                }
                else
                {
                    Debug.LogWarning($"状态{parentSequnce.name}中存在AudioTrack基础物体AudioSource为NULL！请检查配置!!!");
                }
            }
        }

        foreach (var clip in clips)
        {
            clip.Init(this);
        }
    }
#if UNITY_EDITOR

    public override void DrawHeader(Rect rect)
    {
        rectHeader = rect;
        if (audioSource == null)
        {
            GUI.Box(rect, name + "(None)");
        }
        else
        {
            audioName = audioSource.name;
            GUI.Box(rect, audioSource.name + "(Audio)");
        }
    }
    public override string name { get { return "AudioTrack"; } }

    public override void AddPlayableClip(Rect rect)
    {
        if (clips == null)
        {
            clips = new List<PlayableClip>();
        }
        AudioPlayableClip playableClip = new AudioPlayableClip();
        playableClip.rectRange = rect;
        playableClip.startTime = rect.x / scale;
        playableClip.endTime = (rect.x + rect.width) / scale;
        playableClip.parentTrack = this;
        clips.Add(playableClip);
    }
#endif
}