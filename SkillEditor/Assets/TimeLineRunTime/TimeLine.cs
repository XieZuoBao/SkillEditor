using UnityEngine;
using Sirenix.OdinInspector;

public enum WrapMode
{
    None,//播放完毕后 动画恢复到开头
    Hold,//播放完毕后 保持最后的时间位置
    Loop,//循环
}

public enum TimeLineMode
{
    GameTime,//正常游戏那样播放
    Manual,//自己驱动
    UnscaledTime,//不受时间影响的
}
[DisallowMultipleComponent]
//主播放类 用于播放
public class TimeLine : MonoBehaviour
{   
    [FilePath(Extensions = ".txt")]
    public TextAsset textAsset;

    [HideInInspector]
    [Tooltip("当前播放的时间片段")]
    public Sequnce sequnce;

    //主要是为了技能编辑器使用的拓展特殊字段
    //比如受到控制 这种监听消息 或者受到击飞 监听消息 可能全局状态都存在  避免一个个配置 主要是监听消息的
    [HideInInspector]
    [Tooltip("全局播放的时间片段")]
    public Sequnce golalSequnce;

    [Tooltip("当前播放的时间模式\n GameTime: 正常游戏时间播放\nManual :自己调用驱动\n" +
        "UnscaledTime:不受时间因子影响")]
    public TimeLineMode timeMode;

    [Tooltip("当前播放的循环播放\n Loop: 循环模式\n Hold 保持最后一刻\n None 回到开头" )]
    public WrapMode wrapMode;

    [HideInInspector]
    public bool isPlay;

 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(golalSequnce != null)
        {
            golalSequnce.Update(Time.deltaTime);
        }

       if(sequnce != null)
        {
            golalSequnce.Update(Time.deltaTime);
        }
    }

    public void UpdateTimeLine()
    {
        switch (timeMode)
        {
            case TimeLineMode.GameTime:

                break;
            case TimeLineMode.Manual:

                break;
            case TimeLineMode.UnscaledTime:

                break;
        }
    }

    //播放指定序列
    public void Play(Sequnce sequnce)
    {
        this.sequnce = sequnce;
    }

    //播放当前序列
    public void Play()
    {
        isPlay = true;
    }
}