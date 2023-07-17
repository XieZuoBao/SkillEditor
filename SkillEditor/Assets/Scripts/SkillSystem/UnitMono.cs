using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class UnitMono : MonoBehaviour
{
    [Header("角色状态配置表")]
    public TextAsset cfg;

    [HideInInspector]
    public UnitStateMap stateMap;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    //邮箱
    public List<MessageType> messages;

    [HideInInspector]
    private Sequnce currtentSequnce;
    void Awake()
    {
        animator = GetComponent<Animator>();
        stateMap = SerializationUtility.DeserializeValue<UnitStateMap>(cfg.bytes, DataFormat.JSON);
        stateMap.Init();
        messages = new List<MessageType>();
        currtentSequnce = stateMap.stateMapSequnce[stateMap.defalutState];    
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!currtentSequnce.isInit)
        {
            currtentSequnce.isInit = true;
            currtentSequnce.Init(this);
            Debug.Log($"进入{currtentSequnce.name}状态");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            messages.Add(MessageType.Move);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            messages.Add(MessageType.Move);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            messages.Add(MessageType.Move);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            messages.Add(MessageType.Move);
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) ||
            Input.GetKeyUp(KeyCode.D))
        {
            messages.Add(MessageType.Idle);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            messages.Add(MessageType.Sword_3_Hit);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            messages.Add(MessageType.NormalAtk);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {

        }
      
        currtentSequnce.Update(Time.deltaTime);
    }

    public void ChangeState(string nextStateName)
    {
        Debug.Log($"退出{currtentSequnce.name}状态");
        //上一个重置
        currtentSequnce.Reset();
        currtentSequnce = stateMap.stateMapSequnce[nextStateName];
        if (!currtentSequnce.isInit)
        {
            currtentSequnce.isInit = true;
            currtentSequnce.Init(this);
        }
        Debug.Log($"进入{currtentSequnce.name}状态");
    }

    private void LateUpdate()
    {
        messages.Clear();
    }
}