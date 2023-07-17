using UnityEngine;

public enum TransformVector
{
    forward,//朝前
    backward,//朝后
    left,//朝左
    right//朝右
}

//逻辑位移 主要用于位移技能 考虑到后续如果网络同步 不用rootmotion
public class TransformPlayableClip : PlayableClip
{
    [Header("位移的方向")]
    public TransformVector transformVector;

    [Header("速度")]
    public float speed;

    //运行时候的物体
    private GameObject go;
    public override void BeginPlay()
    {
        base.BeginPlay();
        if(go == null)
        {
            TransformTrack transformTrack = parentTrack as TransformTrack;
            go = transformTrack.go;
        }
    }

    public override void Sample(float time)
    {
        if(go == null)
        {
            Debug.LogWarning($"等待位移的物体为空！请检查");
        }
        else
        {
            //暂时用transform位移后期看个人 选择cc  还是自定义一套管理 
            switch (transformVector)
            {
                case TransformVector.backward:
                    go.transform.position += Vector3.back * speed * time;
                    break;
                case TransformVector.forward:
                    go.transform.position += Vector3.forward * speed * time;
                    break;
                case TransformVector.left:
                    go.transform.position += Vector3.left * speed * time;
                    break;
                case TransformVector.right:
                    go.transform.position += Vector3.right * speed * time;
                    break;
            }
        }
    }

    public override void EndPlay()
    {
        base.EndPlay();
    }
}