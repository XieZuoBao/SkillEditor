using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using System;

/// <summary>
/// 特效片段 runtime的时候 需要用对象池复用 
/// </summary>
public class EffectPlayableClip : PlayableClip
{
    [Header("偏移位置")]
    public Vector3 offset;

    [HideInInspector]
    public string effectName;

    //缓存
    [NonSerialized]
    private GameObject temp;

#if UNITY_EDITOR
    [Header("特效预制体")]
    public GameObject effectPrefab;

    [NonSerialized]
    private ParticleSystem particleSystem;

    [NonSerialized]
    private Animation[] animations;

    [NonSerialized]
    private Animator[] animators;

    [NonSerialized]
    private AudioSource[] audioSources;

#endif
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
            if (effectName != null)
            {
                effectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ResPathTools.skillFightEffect + effectName+".prefab");
            }

        }
#endif
    }

    public override void BeginPlay()
    {
        base.BeginPlay();
        if (Application.isPlaying)
        {
            temp = GameObject.Instantiate(effectPrefab);
            temp.transform.position = offset + ((EffectTrack)parentTrack).go.transform.position;
            temp.transform.localScale = Vector3.one;
            particleSystem = temp.GetComponent<ParticleSystem>();
            audioSources = temp.GetComponentsInChildren<AudioSource>();
            particleSystem.Play();

            if (audioSources != null)
            {
                foreach (var audio in audioSources)
                {
                    //audio.PlayScheduled(time);
                    audio.Play();
                }
            }
        }
        
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            particleSystem = null;
            animations = null;
            animators = null;
            audioSources = null;
            
            //TODO 先查找所有的物体
            if (effectPrefab != null)
            {
                //防止拖拽时候反复创建
                if (temp == null)
                {
                    temp = GameObject.Instantiate(effectPrefab);
                    temp.transform.position = offset + ((EffectTrack)parentTrack).go.transform.position;
                    temp.transform.localScale = Vector3.one;
                }                
                particleSystem = temp.GetComponent<ParticleSystem>();
                audioSources = temp.GetComponentsInChildren<AudioSource>();
            }
            if (audioSources != null)
            {
                foreach (var audio in audioSources)
                {
                    //audio.PlayScheduled(time);
                    audio.Play();
                }
            }
        }
#endif
    }

    public override void Sample(float time)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if(particleSystem != null)
            {
                particleSystem.Simulate(time, true);
            }
           

            //if(animations != null)
            //{
            //    foreach(var anim in animations)
            //    {
            //        if (anim.clip != null)
            //        {
            //            anim.clip.SampleAnimation(anim.gameObject, time);
            //        }
            //    }
            //}

            //if(particleSystems != null)
            //{
            //    foreach(var particleSys in particleSystems)
            //    {
            //        particleSys.useAutoRandomSeed = false;
            //        particleSys.Simulate(time);
            //    }
            //}
        }
#endif
    }

    public override void EndPlay()
    {
        base.EndPlay();
        if (Application.isPlaying)
        {          
            particleSystem.time = 0f;
            particleSystem.Stop(true);
            if(audioSources != null)
            {
                foreach(var audio in audioSources)
                {
                    audio.time = 0f;
                    audio.Stop();
                }
            }
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject.DestroyImmediate(temp);
        }      
#endif
    }


#if UNITY_EDITOR
    [NonSerialized]
    private SphereBoundsHandle handle = new SphereBoundsHandle();
    [NonSerialized]
    private Vector3 size;

    public override void Draw(float y, float xOffset)
    {
        if(effectPrefab != null)
        {
            effectName = effectPrefab.name;
        }
        base.Draw(y, xOffset);
    }

    public void DrawController()
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
                handle.center = offset;
                handle.radius =Math.Max(Math.Max(size.x,size.y),size.z);
                handle.DrawHandle();
                offset = handle.center;
                size = new Vector3(handle.radius,handle.radius,handle.radius);
                break;
        }

        Func<Vector3> getOffset = () => new Vector3(offset.x, offset.y, offset.z);
        Func<Vector3> getSize = () => new Vector3(size.x, size.y, size.z);
        offset = getOffset();
        size = getSize();
    }

    public void DrawPos()
    {
        if(handle == null)
        {
            handle = new SphereBoundsHandle();
        }
        Handles.SphereHandleCap(0, offset,Quaternion.identity,handle.radius,EventType.Repaint);
    }
#endif
}
