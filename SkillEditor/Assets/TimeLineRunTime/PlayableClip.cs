using UnityEngine;
using System;
using UnityEditor;

[Serializable]
//可播放的片段
public class PlayableClip
{
    /// <summary>
    /// 默认帧率
    /// </summary>
    public const int DEFALUT_FRAME_RATE = 30;
    /// <summary>
    /// 默认长度
    /// </summary>
    public const int DEFALUT_LENGTH = 10;

    [NonSerialized]
    //所属轨道
    public PlayableTrack parentTrack;
    [Header("clip名称")]
    public string name;
    [Header("开始时间")]
    public float startTime;
    [Header("结束时间")]
    public float endTime;

    [NonSerialized]
    //当前播放时间
    private float currtentTime;
    private float durationTime => endTime - startTime;
    [NonSerialized]
    protected int _currtentFrame;
    [NonSerialized]
    protected int _frameLenth = DEFALUT_LENGTH;
    [NonSerialized]
    protected int _frameRate;

    [NonSerialized]
    public bool isPlayed;


    public int startFrame
    {
        get
        {
            return (int)(startTime / (1/_frameRate));
        }
    }

    public int endFrame
    {
        get
        {
            return (int)(endTime / (1 / _frameRate));
        }
    }

    public virtual bool IsFrameRange(int frame)
    {
        return frame >= startFrame && frame <= endFrame ? true : false;
    }

    public virtual bool IsTimeRange(float time)
    {
        return time >= startTime && time <= endTime ? true : false;
    }

    #region Play
    /// <summary>
    /// 开始播放时候调用此函数
    /// </summary>
    public virtual void BeginPlay() 
    {
        Debug.LogWarning($"进入{name}Clip");
        currtentTime = 0f;
    }

    /// <summary>
    /// 正在播放时候调用此函数
    /// </summary>
    public virtual void OnPlaying(float time) 
    {
        currtentTime += time;
        Sample(currtentTime);
    }

    /// <summary>
    /// 结束播放的时候调用此函数
    /// </summary>
    public virtual void EndPlay() 
    {
        currtentTime = 0f;
        Debug.LogWarning($"退出{name}Clip");
    }

    public virtual void Reset()
    {
        currtentTime = 0f;
        isPlayed = false;
    }
    //采样
    public virtual void Sample(int frame) { }

    public virtual void Sample(float time) { }

    //播放
    public virtual void Play() { }

    //暂停
    public  virtual void Pause() { }

    //停止
    public virtual void Stop() { }

    public virtual void Init(PlayableTrack track)
    {
        parentTrack = track;
        isPlayed = false;
    }
    #endregion

#if UNITY_EDITOR
    //初始化的时候 需要把这些改初始化的都弄出来
    [NonSerialized]
    public Rect rectRange;
    [NonSerialized]
    //第一次按下
    private Vector2 onClick;
    //是否拖拽
    [NonSerialized]
    private bool isDrag;
    //x是时间控制的 y是轨道控制
    public virtual void Draw(float y,float xOffset)
    {
        rectRange.y = y;
        rectRange.x = startTime * parentTrack.scale + xOffset;
        rectRange.width = durationTime * parentTrack.scale;
        //如果是编辑器下读取图形存档 为0 则赋值过去
        if(rectRange.height == 0f)
        {
            rectRange.height = 30f;
        }
        EditorGUI.DrawRect(rectRange, Color.red);
        Rect temp = new Rect();
        temp.x = rectRange.x;
        temp.y = rectRange.y;
        temp.height = rectRange.height;
        temp.width = 200f;
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        GUI.skin.label.fixedWidth = 200f;
        GUI.Label(temp, name + " "+"StartTime:"+startTime);
        GUI.skin.label.fixedWidth = 80f;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
    }

    public virtual void ProcessEvent(Event evt,float heightOffset,float widthOffset,ref object clip)
    {
        switch (evt.type)
        {
            case EventType.MouseDown:
                if(evt.button == 0)
                {
                    Vector2 pointer = evt.mousePosition;
                    pointer.y -= heightOffset;
                    pointer.x -= widthOffset;
                    if (rectRange.Contains(pointer))
                    {
                        onClick = evt.mousePosition;
                        isDrag = true;
                        clip = this;
                        evt.Use();
                    }
                }
                else if(evt.button == 1)
                {
                    Vector2 pointer = evt.mousePosition;
                    pointer.y -= heightOffset;
                    pointer.x -= widthOffset;
                    if (rectRange.Contains(pointer))
                    {
                        GenericMenu genericMenu = new GenericMenu();
                        genericMenu.AddItem(new GUIContent("删除Clip"),false,() => 
                        {
                            parentTrack.clips.Remove(this);
                        });

                        genericMenu.ShowAsContext();
                        clip = this;
                        evt.Use();
                    }
                }
                break;
            case EventType.MouseDrag:
                if (isDrag)
                {
                    float offset = evt.mousePosition.x - onClick.x;
                    //暂时更改Rect 应该是去改时间 拖动的距离换算成时间
                    float time = offset / parentTrack.scale;
                    float temp = durationTime;
                    startTime += time;
                    endTime = startTime + temp;

                    //防止两边拖过头 暂时禁止混合
                    foreach (var item in parentTrack.clips)
                    {
                        if(item != this)
                        {
                            if(startTime <= item.endTime && startTime >= item.startTime)
                            {
                                startTime = item.endTime;
                                endTime = startTime + temp;
                            }
                            else if(endTime >= item.startTime && endTime <= item.endTime)
                            {
                                endTime = item.startTime;
                                startTime = endTime - temp;
                            }
                        }
                    }

                    if(startTime < 0f)
                    {
                        startTime = 0f;
                        endTime = startTime + temp;
                    }

                    //不禁止末尾的超出 因为 这样会导致 不方便一些测试什么的
                    //if (endTime > parentTrack.parentSequnce.durationTime)
                    //{
                    //    endTime = parentTrack.parentSequnce.durationTime;
                    //    startTime = endTime - temp;
                    //}

                    onClick = evt.mousePosition;
                    evt.Use();
                }
                break;
            case EventType.MouseUp:
                isDrag = false;
                break;
        }
    }
#endif
}