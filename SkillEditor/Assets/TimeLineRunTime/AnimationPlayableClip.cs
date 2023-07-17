using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class AnimationPlayableClip : PlayableClip
{
    [HideInInspector]
    public string animationClipName;

    [Header("动画片段")]
    public AnimationClip animationClip;

    [ReadOnly]
    [Header("动画长度")]
    public float lenth;

    public override void Init(PlayableTrack track)
    {
        //基类是用来赋值editor用的
        base.Init(track);

#if UNITY_EDITOR
        //编辑器模式预览下 才需要访问到真正的animationClip
        if(animationClipName != null && !Application.isPlaying)
        {
            AnimationTrack animationTrack = parentTrack as AnimationTrack;
            if(animationTrack.animatorOwn != null)
            {
                foreach (var clip in animationTrack.animatorOwn.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == animationClipName)
                    {
                        animationClip = clip;
                        break;
                    }
                }
            }        
        }
#endif
    }

    public override void BeginPlay()
    {
        base.BeginPlay();
        AnimationTrack track = parentTrack as AnimationTrack;
        if (track.animatorOwn == null)
        {
            Debug.LogError("未找到动画轨道可播放物体!!");
        }
        else
        {
            //track.animatorOwn.Play(animationClipName, 0, 0f);
            track.animatorOwn.CrossFade(animationClipName, 0f,0,0,0f);
            //track.animatorOwn.Update(0);
        }
    }
        

#if UNITY_EDITOR
    //记住一些赋值的隐藏效果可以放到draw里面 每一帧都会去更新赋值 比如这里的name  就可以避开保存的时候需要单独赋值
    public override void Draw(float y, float xOffset)
    {
        if (animationClip != null)
        {
            endTime = startTime + animationClip.length;
            lenth = animationClip.length;
            animationClipName = animationClip.name;
        }
        base.Draw(y, xOffset);
    }
#endif
    public override void Sample(float deltaTime)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
                     
        }
        else
        {
            if (animationClip != null)
            {
                AnimationTrack track = parentTrack as AnimationTrack;
                if (track.animatorOwn == null)
                {
                    Debug.LogWarning("请给动画轨道绑定播放物体");
                }
                else
                {
                    animationClip.SampleAnimation(track.animatorOwn.gameObject, deltaTime);
                }
            }
        }
#endif
        //直接执行Runtime播放
    }
}