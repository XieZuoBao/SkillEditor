using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Sirenix.Serialization;

[Serializable]
//主要是一张数据结构表
public class UnitStateMap
{
    //单位名字
    [Header("单位名字")]
    public string unitName;

    [Header("默认状态")]
    public string defalutState;

    [Header("局部状态")]
    [Sirenix.OdinInspector.FilePath(ParentFolder = "Assets/AssetsBundle/TimeLineTextAssets")]
    //单位的状态储存集合 
    public List<string> stateTextAsset;

    [Header("全局状态")]
    [Sirenix.OdinInspector.FilePath(ParentFolder = "Assets/AssetsBundle/TimeLineTextAssets")]
    //全局配置
    public string gloabTextAsset;

    [NonSerialized]
    //局部时间序列
    public Dictionary<string, Sequnce> stateMapSequnce;

    //全局时间序列
    [NonSerialized]
    public Sequnce gloabSequnce;

    public void Init()
    {
        stateMapSequnce = new Dictionary<string, Sequnce>();
        foreach (var state in stateTextAsset)
        {
            //暂时使用Assetsdatabase 加载 后期整理为 资源接口加载
#if UNITY_EDITOR
            TextAsset cfg = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/AssetsBundle/TimeLineTextAssets/" + state);
#endif
            Sequnce sequnce =
                Sirenix.Serialization.SerializationUtility.DeserializeValue<Sequnce>(cfg.bytes, DataFormat.JSON);
            stateMapSequnce.Add(sequnce.name, sequnce);
        }
    }
}