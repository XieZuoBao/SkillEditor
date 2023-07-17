using UnityEngine;

public class Styles 
{
    //上方按钮 区域
    public Rect topButtonRect;
    //左侧添加轨道的区域!
    public Rect leftAddTrackRect;
    //右侧时间刻度的区域
    public Rect rightTimeLine;
    //左侧轨道区域
    public Rect leftTracksRect;
    //右侧轨道区域
    public Rect rightTrackRect;

    public Styles()
    {
        topButtonRect = new Rect();
        leftTracksRect = new Rect();
        rightTimeLine = new Rect();
    }

    public void Refresh(Rect win)
    {
        topButtonRect.x = 0;
        topButtonRect.y = 0;
        topButtonRect.width = win.width;
        topButtonRect.height = 20f;

        leftAddTrackRect.x = 0;
        leftAddTrackRect.y = topButtonRect.height;
        leftAddTrackRect.height = 30f;
        leftAddTrackRect.width = win.width / 5;

        leftTracksRect.x = 0;
        leftTracksRect.y = topButtonRect.height + leftAddTrackRect.height;
        leftTracksRect.width = win.width / 5;
        leftTracksRect.height = win.height - topButtonRect.height - leftAddTrackRect.height;

        rightTimeLine.x = leftTracksRect.width;
        rightTimeLine.y = topButtonRect.height;
        rightTimeLine.height = leftAddTrackRect.height;
        rightTimeLine.width = win.width / 5 * 4;

        rightTrackRect.x = leftTracksRect.width;
        rightTrackRect.y = leftTracksRect.y;
        rightTrackRect.width = win.width / 5 * 4;
        rightTrackRect.height = leftTracksRect.height;
    }
}