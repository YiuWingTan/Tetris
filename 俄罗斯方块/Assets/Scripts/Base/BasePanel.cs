using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    None = 0,
    SettingPanel,
    AudioPanel,
    DiffcultPanel,
    LevelUpPanel,
    GameOverPanel,
    InformationPanel,//信息面板
}

public  class BasePanel:MonoBehaviour {

    public bool isOverLap;//表示面板是否可以重叠显示
    public int groupID;//面板组ID
    public bool isActive;//表示面板是否被激活，不被激活的面板将不能进行交互
    public PanelType panelType;//面板的类型
    public Animator m_Animator { get { return animator; } }
    public int OpenHash { get { return openHash; } }
    public bool isOpen;//是否被打开


    private Animator animator;
    private int openHash;

    public virtual void ShowPanel()
    {

    }
    public virtual void ClosePanel()
    {

    }
    public virtual void Init()
    {
        //初始化方法
        animator = this.gameObject.GetComponent<Animator>();
        openHash = Animator.StringToHash("isOpen");
        isActive = false;
        isOpen = false;
    }
    /// <summary>
    /// 更新面板数据
    /// </summary>
    /// <param name="data"></param>
    public virtual void UpdatePanelInfo(int[] data)
    {
        
    }
    /// <summary>
    /// 禁止面板的交互
    /// </summary>
    public virtual void UnablePanel()
    {
        //TODO 禁止面板交互
        this.isActive = false;
    }
    /// <summary>
    /// 激活面板的交互
    /// </summary>
    public virtual void EnablePanel()
    {
        //TODO 激活面板的交互
        this.isActive = true;
    }
    
}
