using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
public enum FlyItemType
{
    Single,//单个飞行物
    Multie,//多个飞行物
}

//运动曲线
public enum FlyMoveCurve
{
    Line,//直线
}


[Serializable]
public class FlyItemCfg
{
    [Header("飞行物道具类型")]
    public FlyItemType type;

    [Header("飞行物道具")]
    [ShowIf(@"type==FlyItemType.Multie")]
    public List<GameObject> items;

    [Header("飞行物偏移量")]
    [ShowIf(@"type==FlyItemType.Multie")]
    public List<Vector3> offsets;


    [Header("飞行物道具")]
    [ShowIf(@"type==FlyItemType.Single")]
    public GameObject item;

    [Header("飞行物偏移量")]
    [ShowIf(@"type==FlyItemType.Single")]
    public Vector3 offset;


    [Header("飞行物速度")]
    public float speed;


    [BoxGroup]
    public HitInfoCfg cfg;
}

/// <summary>
/// 飞行物道具轨道
/// </summary>
public class FlyItemPlayableClip : PlayableClip
{

    public override void BeginPlay()
    {
        base.BeginPlay();
    }
}
