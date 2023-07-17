using UnityEngine;

/// <summary>
/// 特效管理类
/// </summary>
public class EffectManager : Singleton<EffectManager>
{
    private GameObject effectNode;
    //相对路径 后期整理到 一个 类中
    private const string relativePath = "";
    public EffectManager()
    {
        if(effectNode == null)
        {
            effectNode = new GameObject("EffectPool");
            GameObject.DontDestroyOnLoad(effectNode);
        }
    }

    public GameObject GetEffectObj(string effectName)
    {
        GameObject temp = null;
        temp = effectNode.transform.Find(effectName).gameObject;
        if(temp != null)
        {
            return temp;
        }
        else
        {
            //TODO:动态创建

            return temp;
        }

    }

    public void PushEffectObj(GameObject go)
    {
        go.transform.parent = effectNode.transform;
        go.transform.localPosition = Vector3.zero;
    }

    //过场景的时候销毁
    public void Clear()
    {
        foreach(var item in effectNode.GetComponentsInChildren<Transform>())
        {
            GameObject.Destroy(item);
        }
    }
}