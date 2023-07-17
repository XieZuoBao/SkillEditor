using System.Collections.Generic;
using UnityEngine;

public class AnimationTrack : PlayableTrack
{
    [Tooltip("RunTime下会使用此动画状态机去播放轨道上的animationClip,\n编辑器模式下使用animationClip采样播放")]
    [Header("动画状态机")]
    public Animator animatorOwn;

    [HideInInspector]
    public string animatorName;

    public override void Init(Sequnce parent)
    {
        parentSequnce = parent;
        if(parent.mUnit == null && !Application.isPlaying)
        {
            GameObject temp = GameObject.Find(animatorName);
            if(temp != null)
            {
                animatorOwn = temp.GetComponent<Animator>();
            }          
        }
        else
        {
            if(parent.mUnit.gameObject.name == animatorName)
            {
                animatorOwn = parent.mUnit.GetComponent<Animator>();
            }
            else
            {
                GameObject temp = GameObject.Find(animatorName);
                if (temp != null)
                {
                    animatorOwn = temp.GetComponent<Animator>();
                }
                else
                {
                    Debug.LogWarning($"状态{parentSequnce.name}中存在aniamtorName 为NULL！请检查配置!!!");
                }
            }
        }
        
        foreach (var clip in clips)
        {
            clip.Init(this);
        }
    }

#if UNITY_EDITOR
    public override string name { get { return "AnimationTrack"; } }

    public override void AddPlayableClip(Rect rect)
    {
        if(clips == null)
        {
            clips = new List<PlayableClip>();
        }
        AnimationPlayableClip playableClip = new AnimationPlayableClip();
        playableClip.rectRange = rect;
        playableClip.startTime = rect.x / scale;
        playableClip.endTime = (rect.x + rect.width) / scale;
        playableClip.parentTrack = this;
        clips.Add(playableClip);  
    }

    public override void DrawHeader(Rect rect)
    {
        rectHeader = rect;
        if(animatorOwn == null)
        {          
            GUI.Box(rect, name+"(None)");
        }
        else
        {
            GUI.Box(rect, animatorOwn.name + "(Aniamtor)");
            animatorName = animatorOwn.gameObject.name;
        }
    }
#endif
}