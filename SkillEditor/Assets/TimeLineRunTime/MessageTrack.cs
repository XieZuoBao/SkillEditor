using System.Collections.Generic;
using UnityEngine;

public class MessageTrack : PlayableTrack
{


#if UNITY_EDITOR
    public override string name
    { get { return "Message Track"; } }

    public override void AddPlayableClip(Rect rect)
    {
        if (clips == null)
        {
            clips = new List<PlayableClip>();
        }
        MessagePlayableClip playableClip = new MessagePlayableClip();
        playableClip.rectRange = rect;
        playableClip.startTime = rect.x / scale;
        playableClip.endTime = (rect.x + rect.width) / scale;
        playableClip.parentTrack = this;
        clips.Add(playableClip);
    }
#endif
}