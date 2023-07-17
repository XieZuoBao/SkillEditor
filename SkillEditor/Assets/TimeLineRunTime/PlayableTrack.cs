using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable]
public class PlayableTrack 
{
    //是否锁定 锁定:播放会忽略此轨道
    [Header("是否锁定轨道")]
    public bool isLock;
    public virtual string name { get; set; }

    [NonSerialized]
    //自己所属的sequnce序列
    public Sequnce parentSequnce;

    [HideInInspector]
    //自己拥有的播放片段
    public List<PlayableClip> clips;

    [HideInInspector]
    //时间刻度 比例 1s = 100f;
    public float scale = 100f;
    public virtual void Init(Sequnce parent)
    {
        parentSequnce = parent;
        foreach(var clip in clips)
        {
            clip.Init(this);
        }
    }

#if UNITY_EDITOR

    //绘制头部Rect
    [NonSerialized]
    public Rect rectHeader;
    //绘制身体Rect
    [NonSerialized]
    public Rect rectBody;
    //上次点击的位置
    [NonSerialized]
    private Vector2 onCLick;
    
    
    //添加可播放片段
    public virtual void AddPlayableClip(Rect rect)
    {

    }

    public virtual void DrawHeader(Rect rect) 
    {
        rectHeader = rect;
        GUI.Box(rect, name);
    }

    public virtual void DrawBody(Rect rect)
    {
        rectBody = rect;
        Color temp = Color.gray;
        temp.a = 0.75f;
        EditorGUI.DrawRect(rectBody, temp);
    }

    /// <summary>
    /// 处理头部事件
    /// </summary>
    /// <param name="evt">事件</param>
    /// <param name="heightOffset">垂直方向的偏移量</param>
    /// <param name="widthOffset">水平方向的偏移量</param>
    public virtual void ProcessHeaderEvent(Event evt,float heightOffset,float widthOffset,ref object track) 
    {
        switch (evt.type)
        {
            case EventType.MouseDown:
                {
                    //必须要没有锁定 才可删除
                    if (evt.button == 1 && !isLock)
                    {
                        Vector2 pointer = evt.mousePosition;
                        pointer.y -= heightOffset;
                        pointer.x -= widthOffset;
                        if (rectHeader.Contains(pointer))
                        {
                            GenericMenu genericMenu = new GenericMenu();

                            genericMenu.AddItem(new GUIContent("删除轨道"), false, () =>
                            {
                                parentSequnce.tracks.Remove(this);
                            });

                            genericMenu.ShowAsContext();
                        }
                    }
                    else if (evt.button == 0)
                    {
                        Vector2 pointer = evt.mousePosition;
                        pointer.y -= heightOffset;
                        pointer.x -= widthOffset;
                        if (rectHeader.Contains(pointer))
                        {
                            track = this;
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 处理身体事件
    /// </summary>
    /// <param name="evt">事件</param>
    /// <param name="heightOffset">垂直方向的偏移量</param>
    /// <param name="widthOffset">水平方向的偏移量</param>
    public virtual void ProcessBodyEvent(Event evt, float heightOffset, float widthOffset)
    {
            switch (evt.type)
            {
                case EventType.MouseDown:
                    {
                        if (evt.button == 1)
                        {
                            onCLick = evt.mousePosition;
                            onCLick.y -= heightOffset;
                            onCLick.x -= widthOffset;
                            if (rectBody.Contains(onCLick))
                            {
                                GenericMenu genericMenu = new GenericMenu();

                                genericMenu.AddItem(new GUIContent($"添加{name} Clip"), false, () => {
                                    Rect rect = new Rect();
                                    rect.x = onCLick.x;
                                    rect.y = rectBody.y;
                                    rect.height = rectBody.height;
                                    rect.width = 30f;
                                    AddPlayableClip(rect);
                                });

                                genericMenu.ShowAsContext();
                            }
                        }
                    }
                    break;
            }      
    }
#endif
}