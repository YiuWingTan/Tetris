using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//UI面板管理器用来控制UI面板的显示和退出
public class UIManager : Singleton<UIManager> {


    private Stack<BasePanel> m_stack;
    [SerializeField]
    private BasePanel m_currentPanel;//当前显示的面板

    public void Init()
    {
        m_stack = new Stack<BasePanel>();
        m_currentPanel = null;
    }

    /// <summary>
    /// 展示面板
    /// </summary>
    /// <param name="panel"></param>
    public void ShowPanel(BasePanel panel)
    {
        Debug.Log(panel.panelType);
        if(panel == null)
        {
            Debug.LogError("程序想要显示一个不存在的Panel");
            return;
        }
        if (panel == m_currentPanel) return;
        if(m_currentPanel == null)
        {
            m_currentPanel = panel;
            showPanel(m_currentPanel);
            return;
        }

        //当新来的面板和当前面板不是同一个GroupID的时候
        if(m_currentPanel.groupID!=panel.groupID)
        {
            //将面板关闭,并关闭清空栈中的所有面板
            CloseAllPanelInStack();
            closePanel(m_currentPanel);
            showPanel(m_currentPanel);
            m_currentPanel = panel;
            return;
        }

        m_stack.Push(m_currentPanel);

        if(m_currentPanel.isOverLap)
        {
            //允许重叠,不进行关闭
            //TODO 禁止面板进行交互
            m_currentPanel.UnablePanel();
        }
        else
        {
            //不允许重叠,将面板关闭
            closePanel(m_currentPanel);
        }

        showPanel(panel);
        m_currentPanel = panel;
    }
    /// <summary>
    /// 关闭当前的面板
    /// </summary>
    /// <param name="index"></param>
    public void ClosePanel(BasePanel panel)
    {
        if(m_currentPanel.panelType!=panel.panelType)
        {
           
            throw new System.Exception("想要关闭一个并非当前显示的面板");
        }

        panel = m_currentPanel;
        closePanel(panel);


        if (m_stack.Count > 0)
        {
            m_currentPanel = m_stack.Pop();
            showPanel(m_currentPanel);
        }
        else m_currentPanel = null;

    }
    /// <summary>
    /// 关闭所有的UI面板
    /// </summary>
    public void CloseAllPanel()
    {
        CloseAllPanelInStack();
        closePanel(m_currentPanel);
        m_currentPanel = null;
    }
    public bool isAllPanelClose()
    {
        if (m_stack.Count == 0 && m_currentPanel == null) return true;
        return false;
    }


    private void closePanel(BasePanel panel)
    {
        if (panel == null) return;
        if(!panel.isOpen)
        {
            //面板已经处于被关闭的状态
            throw new System.Exception("面板已经处于被关闭的状态");
        }

        panel.ClosePanel();
        panel.UnablePanel();//关闭面板的交互功能
        panel.m_Animator.SetBool(panel.OpenHash,false);
        StartCoroutine(closePanel_cor(panel));
        panel.isOpen = false;
    }
    private void showPanel(BasePanel panel)
    {
        if (panel == null) return;

        if(panel.isOpen&&!panel.isActive)
        {
            //当前面板没有被关闭,但交互被禁止
            panel.EnablePanel();
            return;
        }
        if(!panel.isOpen)
        {
            //当前面板已经被关闭或者是正在被关闭
            panel.gameObject.SetActive(true);
            panel.transform.SetAsLastSibling();
            panel.m_Animator.SetBool(panel.OpenHash,true);
            panel.ShowPanel();
            panel.EnablePanel();
            panel.isOpen = true;
        }
    }
    private void CloseAllPanelInStack()
    {
        while (m_stack.Count!=0)
        {
            var panel = m_stack.Pop();
            if(panel.gameObject.activeInHierarchy)
            {
                //关闭这个面板
                closePanel(panel);
            }
        }
    }
    //关闭面板的协程
    private IEnumerator closePanel_cor(BasePanel panel)
    {
        bool isWantClost = true;
        bool isClosed = false;

        while(isWantClost&&!isClosed)
        {
            isWantClost = !panel.m_Animator.GetBool(panel.OpenHash);
            if (panel.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("close") &&
                panel.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                isClosed = true;
            }
            else isClosed = false;
            Debug.Log(isWantClost+"  "+isClosed);
            yield return null;
        }

        if (isWantClost) panel.gameObject.SetActive(false);

    }
}
