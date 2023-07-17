using System.Collections.Generic;
using UnityEngine;
using System;

public class Sequnce
{
    public Sequnce()
    {
        tracks = new List<PlayableTrack>();
    }
    [Header("片段名称")]
    public string name;
    [HideInInspector]
    public List<PlayableTrack> tracks;
    [HideInInspector]
    public float currtentTime;
    [Header("默认进入的下一个状态")]
    public string defalutNextTime;
    [Header("持续时间")]
    public float durationTime;
    [Header("循环模式")]
    public WrapMode wrapMode;
    //本状态所属单位
    [NonSerialized]
    public UnitMono mUnit;
    //是否已经初始化
    [NonSerialized]
    public bool isInit;

    /// <summary>
    /// 一定要初始化 在运行的时候 会去找到场景中对应的资源绑定起来 
    /// 这样就对上层屏蔽了细节
    /// 这个是可定制化的 取决你自己的需求 
    /// 比如我们在编辑的时候拖拽的一些unity场景组件 我们是不可能序列化本身的 只能去记录他的信息 比如名字 类型等等
    /// 然后在运行的时候sequnce被反序列化 然后调用init 去初始化这些物体，可以是从场景中查找，也可以是从资源里面加载
    /// 全部取决于你自己的决策，然后最终调用到的其实就是每个playableClip里面的初始化，比如相机，如果场景查找不到自己保存的
    /// 就可以选择调用Camera.main 这类似的。
    /// </summary>
    public void Init(UnitMono mono = null)
    {
        if(mono != null && mUnit==null)
        {
            mUnit = mono;
        }
        currtentTime = 0;
        foreach (var track in tracks)
        {
            if (!track.isLock)
            {
                track.Init(this);
            }
        }
    }
    /// <summary>
    /// 这样运行的逻辑会有个bug
    /// 如果你是idle的循环 进去一开始会瞬间播放idle 然后 你还没播放到整个的begin设置 就跳转到run
    /// 这时候idle的 isplayed并没有重置 此刻 如果又从run 回到ilde 需要等待idle跑完才会执行 
    /// 所以reset的时候 也要reset isplayed
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <param name="currtent"></param>
    public void Update(float deltaTime,List<HitPlayableClip> currtent = null)
    {
        currtentTime += deltaTime;
        if (currtentTime <= durationTime)
        {
            Debug.Log($"在{name}状态中");
            foreach (var track in tracks)
            {
                if (!track.isLock)
                {
                    if (track.clips != null)
                    {
                        foreach (var clip in track.clips)
                        {
                            if (clip.IsTimeRange(currtentTime) && !clip.isPlayed)
                            {
                                clip.BeginPlay();
                                clip.isPlayed = true;
                                if(currtent != null)
                                {
                                    if (clip.GetType() == typeof(HitPlayableClip))
                                    {
                                        currtent.Add((HitPlayableClip)clip);
                                    }
                                }
                                
                            }
                            else if (clip.IsTimeRange(currtentTime))
                            {
                                clip.OnPlaying(deltaTime);
                            }
                            else if (currtentTime > clip.endTime
                                && clip.isPlayed == true)
                            {
                                clip.isPlayed = false;
                                clip.EndPlay();
                                if (currtent != null)
                                {
                                    if (clip.GetType() == typeof(HitPlayableClip))
                                    {
                                        currtent.Remove((HitPlayableClip)clip);
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
        else
        {
            if(wrapMode == WrapMode.Loop)
            {
                currtentTime = 0f;
            }
            else
            {
                mUnit.ChangeState(defalutNextTime);
            }
        }
    }

    public void Reset()
    {
        currtentTime = 0;
        foreach (var track in tracks)
        {
            if (track.clips != null)
            {
                foreach (var clip in track.clips)
                {
                    clip.Reset();
                }
            }
        }
    }
}