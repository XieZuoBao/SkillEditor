using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System;
using UnityEditor.IMGUI.Controls;

public enum HitRangeType
{
    Box,//盒型
    Sphere,//球形
}



[Serializable]
//盒形打击范围
public class BoxRange
{
    [Header("偏移量")]
    public Vector3 offset;
    [Header("大小")]
    public Vector3 size;

    public BoxRange()
    {
       
 
    }

#if UNITY_EDITOR
    [NonSerialized]
    //用于绘制在Scence场景里面进行编辑Box的句柄
    private BoxBoundsHandle boxHandle = new BoxBoundsHandle();
    public  void DrawController()
    {
        //选择当前场景使用的工具类型
        switch (Tools.current)
        {
            case Tool.View:
                break;
            case Tool.Move:
                offset = Handles.DoPositionHandle(offset, Quaternion.identity);
                break;
            case Tool.Scale:
                float handleSize = HandleUtility.GetHandleSize(offset);
                size = Handles.DoScaleHandle(size, offset, Quaternion.identity, handleSize);
                break;

            case Tool.Transform:
                Vector3 _offset = offset;
                Vector3 _size = size;
                Handles.TransformHandle(ref _offset, UnityEngine.Quaternion.identity, ref _size);
                offset = _offset;
                size = _size;
                break;

            case Tool.Rect:
                //默认情况下是全部启用的
                //如果只想使用某些轴 如下
                //boxHandle.axes = PrimitiveBoundsHandle.Axes.X;
                //boxHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.Z;
                boxHandle.center = offset;
                boxHandle.size = size;
                boxHandle.DrawHandle();
                offset = boxHandle.center;
                size = boxHandle.size;
                break;
        }

        Func<Vector3> getOffset = () => new Vector3(offset.x, offset.y, offset.z);
        Func<Vector3> getSize = () => new Vector3(size.x, size.y, size.z);
        offset = getOffset();
        size = getSize();
    }

    //带有handles的 函数 必须要在 sceneview的回调中调用 才能生效 不然会报空
    public  void DrawRange()
    {
        Handles.color = Color.red;
        Vector3[] points = MathfHelp.CalcBoxVertex(size, Matrix4x4.Translate(offset));
        int[] indexs = MathfHelp.GetBoxSurfaceIndex();
        for (int i = 0; i < 6; i++)
        {
            Vector3[] polygon = new Vector3[] {
                    points[indexs[i * 4]],
                    points[indexs[i * 4 + 1]],
                    points[indexs[i * 4 + 2]],
                    points[indexs[i * 4 + 3]] };
            for (int z = polygon.Length - 1, j = 0; j < polygon.Length; z = j, j++)
            {
                Handles.DrawLine(polygon[z], polygon[j]);
            }
        }
    }
#endif
}

[Serializable]
//球形打击范围
public class SphereRange 
{
    [Header("偏移量")]
    public Vector2 offset;
    [Header("半径")]
    public float radis;

    public SphereRange()
    {
   
  
    }
}

[Serializable]
public class BoxInfoCfg
{
    [Header("打击次数")]
    public int hittedCount;

    [Header("打击间隔")]
    public float hitSpanTime;

    [BoxGroup("命中属性")]
    public HitInfoCfg hitInfo;

    //执行打击操作
    public void Excute()
    {

    }
}

[Serializable]
//打击信息范围
public class HitInfoCfg
{

    [Header("击退距离")]
    public float hitDistance;

    [Header("是否击飞")]
    public bool isHitedFly;

    [ShowIf("@isHitedFly==true")]
    [Header("击飞高度")]
    public float flyDistance;

    [Header("顿帧时间")]
    public float lockTime;

    [Header("命中的时候是否震屏")]
    public bool isCameraShake;    
    [ShowIf("@isCameraShake== true")]
    [Header("震动幅度")]
    public float shakeAmount;
    [ShowIf("@isCameraShake== true")]
    [Header("震动时间")]
    public float shakeTime;

    [Header("命中添加buffID")]
    public List<int> bufferID;

  
}

public class HitPlayableClip : PlayableClip
{
    [BoxGroup("打击范围")]
    [Header("碰撞盒类型")]
    public HitRangeType type;

    [BoxGroup("打击范围")]
    [ShowIf("@type==HitRangeType.Box")]
    public BoxRange boxRange;

    [BoxGroup("打击范围")]
    [ShowIf("@type==HitRangeType.Sphere")]
    public SphereRange sphereRange;

    
    public HitInfoCfg cfg;

    private float tempCount;

    private float tempTime;

    public override void Init(PlayableTrack track)
    {
        base.Init(track);

    }
#if UNITY_EDITOR
    private Matrix4x4 oldMat;
#endif
    public override void BeginPlay()
    {
        base.BeginPlay();
       
    }

    public override void OnPlaying(float time)
    {
        base.OnPlaying(time);
        if(tempCount > 0)
        {

            HitTrack hitTrack = parentTrack as HitTrack;
            RaycastHit[] infos = Physics.BoxCastAll(hitTrack.go.transform.position + boxRange.offset,boxRange.size /2, hitTrack.go.transform.forward);
            foreach(var item in infos)
            {

            }
            tempCount--;
        }
    }


    public override void EndPlay()
    {
        base.EndPlay();

        Handles.matrix = oldMat;
    }

#if UNITY_EDITOR
    //记住一些赋值的隐藏效果可以放到draw里面 每一帧都会去更新赋值 比如这里的name  就可以避开保存的时候需要单独赋值
    public override void Draw(float y, float xOffset)
    {    
       

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

        }
#endif
        //直接执行Runtime播放
    }

}
