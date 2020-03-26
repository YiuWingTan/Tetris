using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//事件对象里面封装着需要进行调用的方法
public class EventObject
{
    public EventType eventType { get; set; }
    public Action actionNull { get; set; }
    public Action<object> objectAction { get; set; }
    public Func<object> getObject { get; set; }
}

//事件管理器
public class EventManager : Singleton<EventManager> {

    private Dictionary<EventType, List<EventObject>> eventList;


    protected override void Awake()
    {
        base.Awake();
        
    }

    public void Init()
    {
        eventList = new Dictionary<EventType, List<EventObject>>();
    }

    /// <summary>
    /// 注册事件的方法
    /// </summary>
    /// <returns></returns>
    public void RegisterEvent(EventType type,Action nullAction,
        Action<object> argAction = null,Func<object> getObjectFunc = null)
    {
        if (argAction == null && nullAction == null) return;
        if (argAction == null && getObjectFunc != null)
            throw new Exception("argAction is  null but getobjectFunc is not null");
        if (argAction != null && getObjectFunc == null)
            throw new Exception("argAction is not null but getObjectFunc is null");

        if (eventList.ContainsKey(type))
        {
            EventObject eo = new EventObject();
            eo.objectAction = argAction;
            eo.actionNull = nullAction;
            eo.getObject = getObjectFunc;
            eo.eventType = type;
            eventList[type].Add(eo);
        }
        else
        {
            List<EventObject> list = new List<EventObject>();
            EventObject eo = new EventObject();
            eo.actionNull = nullAction;
            eo.objectAction = argAction;
            eo.getObject = getObjectFunc;
            eo.eventType = type;
            list.Add(eo);

            eventList.Add(type,list);
        }
        Debug.Log("成功注册了"+type+"类型的事件");
    }

    /// <summary>
    /// 阻塞的调用一个事件调用
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool CallEventImmediately(EventType type)
    {
        if (!eventList.ContainsKey(type)) return false;

        foreach(var eventObjects in eventList[type])
        {
            if (eventObjects.actionNull != null)
                eventObjects.actionNull();
            if (eventObjects.objectAction != null)
                eventObjects.objectAction(eventObjects.getObject());
        }
        return true;
    }

    /// <summary>
    /// 使用协程来调用事件
    /// </summary>
    public void CallEventDelay(EventType type)
    {
        if (!eventList.ContainsKey(type)) return;

        StartCoroutine(EventHandle(eventList[type]));
    }

    //使用协程来进行事件的处理
    IEnumerator EventHandle(List<EventObject> obs)
    {
        
        foreach (var eventObjects in obs)
        {
            if (eventObjects.actionNull != null)
                eventObjects.actionNull();
            if (eventObjects.objectAction != null)
                eventObjects.objectAction(eventObjects.getObject());
        }
        yield return null;
    }

}
