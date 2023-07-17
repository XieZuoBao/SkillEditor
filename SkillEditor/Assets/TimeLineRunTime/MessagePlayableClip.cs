using UnityEngine;

public enum MessageType
{
    Idle,//待机消息
    Move,//移动消息
    NormalAtk,//普通攻击
    Jump,//跳跃消息
    Sword_3_Hit,//剑士 三连斩
}
public class MessagePlayableClip : PlayableClip
{
    [Header("监听消息")]
    public MessageType message;

    [Header("是否强制打断")]
    public bool isForce;

    [Header("消息跳转状态")]
    public string state;

    public override void Init(PlayableTrack track)
    {
        base.Init(track);
    }

    public override void OnPlaying(float time)
    {
        base.OnPlaying(time);
        if (Application.isPlaying)
        {
            if (parentTrack.parentSequnce.mUnit.messages.Contains(message))
            {
                if (isForce)
                {
                    parentTrack.parentSequnce.mUnit.ChangeState(state);
                }
                else
                {

                }
            }
        }
    }
}