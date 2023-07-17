using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Serialization;
using System.IO;
/// <summary>
/// 迭代任务：1. 完成存档更改名称  以及 技能编辑器的时候 创建角色配置表* (此任务 需要和策划沟通一下)
/// 2. 完成碰撞轨道的绘制 与逻辑执行 (包含runtime的逻辑) *
/// 3.完成特效轨道的释放与回收 （包含runtime的逻辑） *
/// 4.完成音效轨道的释放与回收 (包含runtime的逻辑) *
/// 5.完成消息控制轨道的释放与回收 （包含runtime的逻辑） *
/// 6.实现物体播放的管理配置表(会记录下一个物体 所有的TimeLine 配置 对于技能编辑器来讲 一个timeline就是一个 state) *
/// 7.考虑把6中管理配置表 有可能话 做成node节点编辑器
/// 8.目前重复保存的时候 文件名字 状态 会不断的叠加 
/// 9.AnimationClip 要来源于Animator 想办法 做成枚举类型筛选
/// （为了兼容后续的一些剧情编辑器拓展 这里没有定死在某个aniamtor 但是 我们做技能的时候基本都是从
/// 一个aniamtor上面进行获取）
/// 
/// //修复内容
/// 1.已经修复打开空配置时候 面板无法显示的问题
/// 2.新增循环模式
/// 3.修复某些情况下 clip无法播放的问题
/// </summary>
public class TimeLineWindow : EditorWindow
{
    public static bool willRepaint;

    private const string Version = "Version 1.0";

    private static TimeLine timeLine;
    public static Styles styles;
    public static TimeLineWindow currtent;
    public static object select;
    private static bool isPlay;
    //本次打开的配置表
    public static TextAsset cfg;
    //记得和playTrack 里面的scale同步 懒得整理代码了
    private float scale = 100f;
    //上一次的时间
    private static double lastTime = 0.0f;
    //本次状态总共持续的时间
    private static float totalTime = 0.0f;
    //当前时间指针的Rect
    private Rect currtenTimeRect;
    //当前结束时间指针的Rect
    private Rect endTimeRect;
    //是否处于拖拽当前时间指针
    private bool isDragCurrtent;
    //是否处于拖拽结束时间指针
    private bool isDragEnd;
    //是否处于轨道拖拽状态 用于拖动时间轴的
    private bool isDragTimeLine;
    //当前时间轨道的偏移量
    private Vector2 offset = Vector2.zero;
    //上次按下的位置
    private Vector2 lastPostion;
    //路径
    static string path;
    //当前配置表路径 每次调用Create后
    static string currtentPath;
    //当前的场景物体
    private static SceneView currtentView;
    //当前已经开始播放的碰撞盒
    public static List<HitPlayableClip> currtentHit;

    public static void Open(TimeLine temp,TextAsset asset)
    {
        currtent = GetWindow<TimeLineWindow>();
        currtent.titleContent = new GUIContent("时间轴编辑器");
        InSpectorWindow.OpenWindow();
        styles = new Styles();
        isPlay = false;
        timeLine = temp;
        path = Application.streamingAssetsPath.Replace("StreamingAssets", "") + "AssetsBundle/TimeLineTextAssets/" + timeLine.gameObject.name;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        AssetDatabase.Refresh();
        path += "/";
        cfg = asset;
        if(cfg != null)
        {
            //如果传入的是配置表才会去执行
            if (cfg.name.Contains("SequnceAsset"))
            {
                timeLine.sequnce = Sirenix.Serialization.SerializationUtility.DeserializeValue<Sequnce>(cfg.bytes, DataFormat.JSON);
                timeLine.sequnce.Init();
            }
            //否则就会new 配置 强行覆盖timeline中的选择回去
            else
            {
                CreateSequnce();
                //这一帧是找不到资源的
                //cfg = AssetDatabase.LoadAssetAtPath<TextAsset>(currtentPath);
                //timeLine.textAsset = cfg;
            }

        }
        else
        {
            CreateSequnce();
            //这一帧是找不到资源的
            //cfg = AssetDatabase.LoadAssetAtPath<TextAsset>(currtentPath);
            //timeLine.textAsset = cfg;
        }
        currtentHit = new List<HitPlayableClip>();
        EditorApplication.update += PlayUpdate;
        SceneView.duringSceneGui += OnScenceView;
        lastTime = EditorApplication.timeSinceStartup;
        select = null;
    }

    //用于场景拖拽物体
    private static void OnScenceView(SceneView view)
    {
        if(currtentView != view)
        {
            currtentView = view;
        }

        //当选中碰撞范围的时候
        if(select!= null && select.GetType() == typeof(HitPlayableClip)&&!isPlay)
        {
            HitPlayableClip clip = select as HitPlayableClip;
            HitTrack track = clip.parentTrack as HitTrack;
            Matrix4x4 localToWorld = default;
            if (track.go == null)
            {
                Debug.LogWarning("---请添加碰撞检测基础范围物体!----");
                return;
            }
            else
            {
                localToWorld = (Matrix4x4)track.go.transform.localToWorldMatrix;
            }
            Matrix4x4 localToWorldNoScale = Matrix4x4.TRS(localToWorld.MultiplyPoint3x4(Vector3.zero), localToWorld.rotation, Vector3.one);

            Matrix4x4 oldMat = Handles.matrix;
            Handles.matrix = localToWorldNoScale;
            if (clip.type == HitRangeType.Box)
            {
                clip.boxRange.DrawController();
                clip.boxRange.DrawRange();
            }
            else if (clip.type == HitRangeType.Sphere)
            {
                //clip.sphereRange.DrawController();
                //clip.sphereRange.DrawRange();
            }
            Handles.matrix = oldMat;
            InSpectorWindow.win.Repaint();
        }
        //当选中特效附着点的时候
        if(select != null && select.GetType() == typeof(EffectPlayableClip) && !isPlay)
        {
            EffectPlayableClip clip = select as EffectPlayableClip;
            EffectTrack track = clip.parentTrack as EffectTrack;
            Matrix4x4 localToWorld = default;
            if (track.go == null)
            {
                localToWorld = (Matrix4x4)timeLine.transform.localToWorldMatrix;
            }
            else
            {
                localToWorld = (Matrix4x4)track.go.transform.localToWorldMatrix;
            }
            Matrix4x4 localToWorldNoScale = Matrix4x4.TRS(localToWorld.MultiplyPoint3x4(Vector3.zero), localToWorld.rotation, Vector3.one);

            Matrix4x4 oldMat = Handles.matrix;
            Handles.matrix = localToWorldNoScale;
            clip.DrawController();
            clip.DrawPos();
            Handles.matrix = oldMat;
            InSpectorWindow.win.Repaint();
        }
        if (isPlay)
        {
            foreach(var clip in currtentHit)
            {
                HitTrack track = clip.parentTrack as HitTrack;
                Matrix4x4 localToWorld = default;
                if (track.go == null)
                {
                    Debug.LogWarning("---请添加碰撞检测基础范围物体!----");
                    return;
                }
                else
                {
                    localToWorld = (Matrix4x4)track.go.transform.localToWorldMatrix;
                }
                Matrix4x4 localToWorldNoScale = Matrix4x4.TRS(localToWorld.MultiplyPoint3x4(Vector3.zero), localToWorld.rotation, Vector3.one);

                Matrix4x4 oldMat = Handles.matrix;
                Handles.matrix = localToWorldNoScale;
                if (clip.type == HitRangeType.Box)
                {
                    clip.boxRange.DrawRange();
                }
                else if (clip.type == HitRangeType.Sphere)
                {
                    //clip.sphereRange.DrawController();
                    //clip.sphereRange.DrawRange();
                }
                Handles.matrix = oldMat;
            }
        }
    }

    //用于Editor下进行播放预览的
    private static void PlayUpdate()
    {
        //确保在编辑器下
        if (!Application.isPlaying&& isPlay)
        {
            float deltaTime = 0;
            deltaTime = (float)(EditorApplication.timeSinceStartup - lastTime);
            lastTime = EditorApplication.timeSinceStartup;
            totalTime += deltaTime;
            if(totalTime < timeLine.sequnce.durationTime)
            {
                timeLine.sequnce.Update(deltaTime,currtentHit);
                //防止报空 一开始没有激活场景 就不会调用回调就为空
                if(currtentView != null)
                {
                    currtentView.Focus();
                    currtentView.Repaint();
                }                
            }
            else if(timeLine.sequnce.wrapMode == WrapMode.Loop)
            {
                totalTime = 0;
                isPlay = true;
                timeLine.sequnce.Reset();
            }
            else if(timeLine.sequnce.wrapMode == WrapMode.Hold)
            {
                isPlay = false;
            }
            else if(timeLine.sequnce.wrapMode == WrapMode.None)
            {
                totalTime = 0;
                isPlay = false;
                timeLine.sequnce.Reset();
                //这里可能会让特效轨道重新创建
                //timeLine.sequnce.Update(0f);
            }
            willRepaint = true;
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnGUI()
    {      
        if (cfg != null)
        {
            styles.Refresh(position);
            #region 绘制播放工具栏
            //绘制播放工具栏
            GUILayout.BeginArea(styles.topButtonRect);
            EditorGUILayout.BeginHorizontal();
            Color tempColor = GUI.color;
            GUI.color = Color.red;
            GUILayout.Label($"{timeLine.gameObject.name}");
            GUILayout.Label($"{timeLine.sequnce.name}");
            GUI.color = tempColor;
            float buttonWidth = GUI.skin.button.fixedWidth;
            float labelWidth = GUI.skin.label.fixedWidth;
            GUI.skin.button.fixedWidth = 80f;
            GUI.skin.label.fixedWidth = 100f;
            if (!isPlay)
            {
                if (GUILayout.Button("播放"))
                {
                    totalTime = timeLine.sequnce.currtentTime;
                    isPlay = true;
                    lastTime = EditorApplication.timeSinceStartup;
                }
            }
            else
            {
                if (GUILayout.Button("暂停"))
                {
                    isPlay = false;
                }
            }

            if (GUILayout.Button("重置"))
            {
                isPlay = false;
                timeLine.sequnce.Reset();
                timeLine.sequnce.Update(0f);
            }
            if (GUILayout.Button("创建新配置"))
            {
                CreateSequnce();
                cfg = AssetDatabase.LoadAssetAtPath<TextAsset>(currtentPath);
                timeLine.textAsset = cfg;
            }
      
            if (GUILayout.Button("保存"))
            {
                byte[] bytes = Sirenix.Serialization.SerializationUtility.SerializeValue(timeLine.sequnce, DataFormat.JSON);
                string cfgName = new string(cfg.name.ToCharArray());
                if(File.Exists(path + cfg.name + ".txt"))
                {
                    File.WriteAllBytes(path + cfg.name + ".txt", bytes);
                    File.Move(path + cfg.name + ".txt", path + timeLine.sequnce.name + "_" + cfg.name + ".txt");
                    AssetDatabase.Refresh();                 
                }
                else
                {
                    File.WriteAllBytes(path + timeLine.sequnce.name + "_" + cfg.name + ".txt", bytes);
                }
                AssetDatabase.Refresh();
                currtentPath = $"Assets/AssetsBundle/TimeLineTextAssets/{timeLine.gameObject.name}/{timeLine.sequnce.name + "_" + cfgName}.txt";
                cfg = null;                         
            }
            if (GUILayout.Button("设置"))
            {
                select = timeLine.sequnce;
            }
            EditorGUILayout.LabelField(Version);    
            GUI.skin.label.fixedWidth = labelWidth;
            GUI.skin.button.fixedWidth = buttonWidth;       
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion

            #region 绘制左侧添加轨道

            GUILayout.BeginArea(styles.leftAddTrackRect);
            GUI.skin.button.fixedWidth = 80f;
            if (GUILayout.Button("添加轨道"))
            {
                GenericMenu genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("Animation Track"), false, () =>
                {
                    PlayableTrack track =  new AnimationTrack();
                    track.parentSequnce = timeLine.sequnce;
                    timeLine.sequnce.tracks.Add(track);
                });
                genericMenu.AddItem(new GUIContent("Camera Track"), false, () =>
                {
                    PlayableTrack track = new CameraTrack();
                    track.parentSequnce = timeLine.sequnce;
                    timeLine.sequnce.tracks.Add(track);
                });
                genericMenu.AddItem(new GUIContent("Hit Track"), false, () =>
                {
                    PlayableTrack track = new HitTrack();
                    track.parentSequnce = timeLine.sequnce;
                    timeLine.sequnce.tracks.Add(track);
                });
                genericMenu.AddItem(new GUIContent("Audio Track"), false, () =>
                {
                    PlayableTrack track = new AudioTrack();
                    track.parentSequnce = timeLine.sequnce;
                    timeLine.sequnce.tracks.Add(track);
                });
                genericMenu.AddItem(new GUIContent("Effect Track"), false, () =>
                {
                    PlayableTrack track = new EffectTrack();
                    track.parentSequnce = timeLine.sequnce;
                    timeLine.sequnce.tracks.Add(track);
                });
                genericMenu.AddItem(new GUIContent("Message Track"), false, () =>
                {
                    PlayableTrack track = new MessageTrack();
                    track.parentSequnce = timeLine.sequnce;
                    timeLine.sequnce.tracks.Add(track);
                });
                genericMenu.AddItem(new GUIContent("FlyItem Track"), false, () =>
                {
                    PlayableTrack track = new FlyItemTrack();
                    track.parentSequnce = timeLine.sequnce;
                    timeLine.sequnce.tracks.Add(track);
                });
                genericMenu.AddItem(new GUIContent("Transform Track"), false, () =>
                {
                    PlayableTrack track = new TransformTrack();
                    track.parentSequnce = timeLine.sequnce;
                    timeLine.sequnce.tracks.Add(track);
                });
                genericMenu.ShowAsContext();
            }
            GUI.skin.button.fixedWidth = buttonWidth;
            GUILayout.EndArea();
            #endregion

            #region 绘制轨道头部
            //绘制轨道
            GUILayout.BeginArea(styles.leftTracksRect);
            Rect rect = new Rect();
            int index = 0;
            rect.x = 0;
            rect.width = styles.leftTracksRect.width;
            rect.height = 30f;
            rect.y = index * rect.height;
            foreach (var track in timeLine.sequnce.tracks)
            {               
                track.DrawHeader(rect);
                if (track.isLock)
                {
                    Color color = GUI.color;
                    GUI.color = Color.red;
                    GUI.Label(rect, "已被锁定 无法编辑和播放 请解锁");
                    GUI.color = color;
                }
                index++;
                //间隔5f
                rect.y = index * (rect.height + 5f);
            }
            GUILayout.EndArea();
            #endregion           

            #region 绘制轨道身体
            GUILayout.BeginArea(styles.rightTrackRect);
            int index1 = 0;
            Rect rect1 = new Rect();
            rect1.x = 0;
            rect1.width = styles.rightTrackRect.width;
            rect1.height = 30f;
            rect1.y = 0;
            foreach (var track in timeLine.sequnce.tracks)
            {
                track.DrawBody(rect1);
                index1++;
                //间隔5f
                rect1.y = index1 * (rect1.height + 5f);
            }
            GUILayout.EndArea();
            #endregion

            #region 绘制clips
            GUILayout.BeginArea(styles.rightTrackRect);
            int index3 = 0;
            float y = 0;
            foreach(var item in timeLine.sequnce.tracks)
            {
                if (item.clips != null)
                {
                    foreach (var clip in item.clips)
                    {
                        Color tempColor1 = GUI.color;
                        if (select == clip)
                        {                            
                            GUI.color = Color.green;
                        }
                        clip.Draw(y,offset.x);
                        GUI.color = tempColor1;

                    }
                }          
                index3++;
                y = index3 * (30f + 5f);
            }
            GUILayout.EndArea();
            #endregion

            #region 绘制时间刻度线
            GUILayout.BeginArea(styles.rightTimeLine);
            //先画横线 不受偏移影响
            Rect rect3 = new Rect();
            rect3.x = 0;
            rect3.height = 1f;
            rect3.width = styles.rightTimeLine.width;
            rect3.y = styles.rightTimeLine.height - 1f;
            EditorGUI.DrawRect(rect3, Color.red);

            //再画刻度线 受到偏移影响
            float count = (styles.rightTimeLine.width + Mathf.Abs(offset.x)) / scale;
            for (int i = 0; i < count; i++)
            {
                rect3.x = i * scale + offset.x;
                rect3.height = styles.rightTimeLine.height / 2;
                rect3.y = styles.rightTimeLine.height / 2;
                rect3.width = 1f;
                EditorGUI.DrawRect(rect3, Color.red);
                rect3.y = 0;
                rect3.width = 60f;
                rect3.height = styles.rightTimeLine.height / 2;
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUI.Label(rect3, i.ToString());
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(styles.rightTrackRect);
            for (int i = 0; i < count; i++)
            {
                rect3.x = i * scale + offset.x;
                rect3.height = styles.rightTrackRect.height;
                rect3.y = 0f;
                rect3.width = 1f;
                EditorGUI.DrawRect(rect3, Color.red);
            }
            GUILayout.EndArea();
            #endregion

            #region 绘制时间指针 和 截止时间
            GUILayout.BeginArea(styles.rightTimeLine);
            //绘制时间指针
            Rect rect2 = new Rect();
            rect2.height = styles.rightTimeLine.height;
            rect2.width = 15f;
            rect2.y = 0;
            rect2.x = timeLine.sequnce.currtentTime * 100f + offset.x;
            //给当前时间拖拽框赋值
            currtenTimeRect = rect2;
            EditorGUI.DrawRect(rect2, Color.red);
            //绘制上方时间提示
            rect2.width = 100f;
            GUI.Label(rect2, $"当前时间:{timeLine.sequnce.currtentTime}");

            rect2.width = 15f;
            //绘制截止时间
            rect2.x = timeLine.sequnce.durationTime * 100f + offset.x;
            EditorGUI.DrawRect(rect2, Color.blue);
            //给结束拖拽框 赋值
            endTimeRect = rect2;
            //绘制结束提示
            rect2.width = 100f;
            GUI.Label(rect2, $"结束时间:{timeLine.sequnce.durationTime}");
            GUILayout.EndArea();
            
            GUILayout.BeginArea(styles.rightTrackRect);
            //绘制时间指针
            rect2.x = timeLine.sequnce.currtentTime * 100f + offset.x;
            rect2.height = styles.rightTrackRect.height;
            rect2.width = 2f;
            EditorGUI.DrawRect(rect2, Color.red);

            //绘制截止时间
            rect2.x = timeLine.sequnce.durationTime * 100f + offset.x;
            EditorGUI.DrawRect(rect2, Color.blue); ;
            GUILayout.EndArea();
            #endregion

            #region 处理事件
            //处理轨道事件
            foreach (var track in timeLine.sequnce.tracks)
            {
                track.ProcessHeaderEvent(Event.current, styles.topButtonRect.height + styles.leftAddTrackRect.height, 0f,ref select);
                if (!track.isLock)
                {
                    track.ProcessBodyEvent(Event.current, styles.topButtonRect.height + styles.leftAddTrackRect.height, styles.leftAddTrackRect.width);
                }               
            }

            //处理clips事件
            foreach(var track in timeLine.sequnce.tracks)
            {
                if(track.clips != null && !track.isLock)
                {
                    foreach(var clip in track.clips)
                    {
                        clip.ProcessEvent(Event.current, styles.topButtonRect.height + styles.leftAddTrackRect.height, styles.leftAddTrackRect.width,ref select);
                    }
                }
            }

            //处理拖拽当前时间指针事件
            ProcessDragEndTimeLine(Event.current);
            //处理拖拽结束指针事件
            ProcessDragCurrtentTimeLine(Event.current);
            //处理拖拽时间轴事件
            ProcessDragTimeLine(Event.current);
            //这里是为了让在TimeLine窗体的修改 能够立即同步到数据面板
            InSpectorWindow.win.Repaint();
            #endregion
        }
    }

    private void Update()
    {
        if(cfg == null)
        { 
            cfg = AssetDatabase.LoadAssetAtPath<TextAsset>(currtentPath);
            timeLine.textAsset = cfg;
        }
        //一旦 timeline.textAsset 等于null 就会赋值为当前的cfg
        //当前的cfg永远不会为空 如果为空那么打开的时候 就会创建新的
        if (timeLine.textAsset == null)
        {
            timeLine.textAsset = cfg;
        }

        //现在切换选择的时候 会赋值为空
        if(cfg != null && timeLine.textAsset.name != cfg.name) 
        {
            if (timeLine.textAsset.name.Contains("SequnceAsset"))
            {
                cfg = timeLine.textAsset;
                ReadFromSequnceText();
            }
            else
            {
                //如果上一次new过配置 那么赋值了 不是sequence的配置 就会设置为null 与上方的if为null呼应了起来
                timeLine.textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(currtentPath);
            }
            willRepaint = true;
        }
        if (willRepaint)
        {
            willRepaint = false;
            Repaint();
        }
    }

    private void OnDisable()
    {
        EditorApplication.update -= PlayUpdate;
        SceneView.duringSceneGui -= OnScenceView;
    }

    //拖拽currtent
    private void ProcessDragCurrtentTimeLine(Event evt)
    {
        switch (evt.type)
        {
            case EventType.MouseDown:
                {
                    if (evt.button == 0)
                    {
                        lastPostion = evt.mousePosition;
                        lastPostion.x -= styles.leftAddTrackRect.width;
                        lastPostion.y -= styles.topButtonRect.height;
                        if (currtenTimeRect.Contains(lastPostion))
                        {
                            isDragCurrtent = true;
                            lastPostion = evt.mousePosition;
                            evt.Use();
                        }
                    }
                }
                break;
            case EventType.MouseDrag:
                {
                    if (isDragCurrtent)
                    {
                        float tempX = evt.mousePosition.x - lastPostion.x;
                        timeLine.sequnce.currtentTime += tempX / 100f;
                        if(timeLine.sequnce.currtentTime >= timeLine.sequnce.durationTime)
                        {
                            timeLine.sequnce.currtentTime = timeLine.sequnce.durationTime;
                        }
                        else if(timeLine.sequnce.currtentTime < 0f)
                        {
                            timeLine.sequnce.currtentTime = 0f;
                        }
                        lastPostion = evt.mousePosition;

                        //拖拽时间轴的时候采样全部的
                        foreach (var track in timeLine.sequnce.tracks)
                        {
                            if (!track.isLock)
                            {
                                if (track.clips != null)
                                {
                                    foreach (var clip in track.clips)
                                    {
                                        if (clip.IsTimeRange(timeLine.sequnce.currtentTime) && !clip.isPlayed)
                                        {
                                            clip.BeginPlay();
                                            clip.isPlayed = true;
                                        }
                                        else if (clip.IsTimeRange(timeLine.sequnce.currtentTime))
                                        {
                                            clip.Sample(timeLine.sequnce.currtentTime - clip.startTime);
                                        }
                                        else if (timeLine.sequnce.currtentTime > clip.endTime
                                            && clip.isPlayed == true)
                                        {
                                            clip.isPlayed = false;
                                            clip.EndPlay();
                                        }

                                    }
                                }
                            }
                        }
                        if(currtentView != null)
                        {
                            currtentView.Repaint();
                        }
                        evt.Use();
                    }
                }
                break;
            case EventType.MouseUp:
                isDragCurrtent = false;
                break;
        }
    }

    //拖拽end
    private void ProcessDragEndTimeLine(Event evt)
    {
        switch (evt.type)
        {
            case EventType.MouseDown:
                {
                    if(evt.button == 0)
                    {
                        lastPostion = evt.mousePosition;
                        lastPostion.x -= styles.leftAddTrackRect.width;
                        lastPostion.y -= styles.topButtonRect.height;
                        if (endTimeRect.Contains(lastPostion))
                        {
                            isDragEnd = true;
                            lastPostion = evt.mousePosition;
                            evt.Use();
                        }
                    }
                }
                break;
            case EventType.MouseDrag:
                {
                    if (isDragEnd)
                    {
                        float tempX = evt.mousePosition.x - lastPostion.x;
                        timeLine.sequnce.durationTime += tempX / 100f;
                        if(timeLine.sequnce.durationTime < 0f)
                        {
                            timeLine.sequnce.durationTime = 0f;
                        }

                        float bestMaxClips = 0f;
                        foreach(var track in timeLine.sequnce.tracks)
                        {
                            if(track.clips != null)
                            {
                                foreach(var clip in track.clips)
                                {
                                    if(clip.endTime > bestMaxClips)
                                    {
                                        bestMaxClips = clip.endTime;
                                    }
                                }
                            }
                        }
                        if(timeLine.sequnce.durationTime < bestMaxClips)
                        {
                            timeLine.sequnce.durationTime = bestMaxClips;
                        }

                        lastPostion = evt.mousePosition;
                        evt.Use();
                    }
                }              
                break;
            case EventType.MouseUp:
                isDragEnd = false;
                break;
        }
    }

    private void ProcessDragTimeLine(Event evt)
    {
        switch (evt.type)
        {
            case EventType.MouseDown:
                {
                    if (evt.button == 0)
                    {
                        lastPostion = evt.mousePosition;
                        lastPostion.x -= styles.leftAddTrackRect.width;
                        lastPostion.y -= styles.topButtonRect.height;
                        if (styles.rightTrackRect.Contains(lastPostion))
                        {
                            isDragTimeLine = true;
                            lastPostion = evt.mousePosition;
                            evt.Use();
                        }
                    }
                }
                break;
            case EventType.MouseDrag:
                {
                    if (isDragTimeLine)
                    {
                        float tempX = evt.mousePosition.x - lastPostion.x;                              
                        offset.x += tempX;
                        //避免越界0    
                        if (offset.x > 0f)
                        {
                            offset.x = 0f;
                        }
                        lastPostion = evt.mousePosition;
                        evt.Use();
                    }
                }
                break;
            case EventType.MouseUp:
                isDragTimeLine = false;
                break;
        }
    }

    //创建新的文档
    private static void CreateSequnce()
    {
        timeLine.sequnce = new Sequnce();
        byte[] bytes = Sirenix.Serialization.SerializationUtility.SerializeValue(timeLine.sequnce, DataFormat.JSON);
        int index = 1;
        string temp = path + $"{timeLine.gameObject.name}SequnceAsset.txt";
        currtentPath = $"Assets/AssetsBundle/TimeLineTextAssets/{timeLine.gameObject.name}/{timeLine.gameObject.name}SequnceAsset.txt";
        while (File.Exists(temp))
        {
            temp = path + $"{timeLine.gameObject.name}SequnceAsset_{index}.txt";
            currtentPath = $"Assets/AssetsBundle/TimeLineTextAssets/{timeLine.gameObject.name}/{timeLine.gameObject.name}SequnceAsset_{index}.txt"; ;
            index++;
        }
        File.WriteAllBytes(temp, bytes);
        AssetDatabase.Refresh();
        timeLine.sequnce.Init();
        willRepaint = true;
    }

    private static void ReadFromSequnceText()
    {
        timeLine.sequnce = Sirenix.Serialization.SerializationUtility.DeserializeValue<Sequnce>(cfg.bytes, DataFormat.JSON);
        timeLine.sequnce.Init();
    }
    private void SamplTime(float deltaTime)
    {

    }
    
    private void OnDestroy()
    {
        InSpectorWindow.win.Close();
    }
}