using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiffcultPanel : BasePanel {

    [SerializeField]
    private Button m_closeButton;
    [SerializeField]
    private Toggle m_easyToggle;
    [SerializeField]
    private Toggle m_normalToggle;
    [SerializeField]
    private Toggle m_hardToggle;
   
    public override void Init()
    {
        base.Init();
        this.panelType = PanelType.DiffcultPanel;

        m_closeButton.onClick.AddListener(()=> { MVCController.Instance.ClosePanel(this.panelType);
            EventManager.Instance.CallEventImmediately(EventType.PlayCursorAudio); Debug.Log("关闭DiffcultPanel"); });

        m_easyToggle.onValueChanged.AddListener(onToggleValueChange);
        m_normalToggle.onValueChanged.AddListener(onToggleValueChange);
        m_hardToggle.onValueChanged.AddListener(onToggleValueChange);

    }
    public override void ShowPanel()
    {
        //判断当前的难度等级
        switch(GameManager.Instance.CurrentDiffcultLevel)
        {
            case DiffcultLevel.Easy:
                m_easyToggle.isOn = true;
                break;
            case DiffcultLevel.Normal:
                m_normalToggle.isOn = true;
                break;
            case DiffcultLevel.Hard:
                m_hardToggle.isOn = true;
                break;
        }
    }
    /// <summary>
    /// 当Toggle的值进行了更改的时候进行调用
    /// </summary>
    public void onToggleValueChange(bool isOn)
    {
        //选择出当前被选择的Toggle
        if (m_easyToggle.isOn) {
            GameManager.Instance.CurrentDiffcultLevel = DiffcultLevel.Easy;
            EventManager.Instance.CallEventImmediately( EventType.SetDiffcultLevel);
        }
        if (m_normalToggle.isOn) {
            GameManager.Instance.CurrentDiffcultLevel = DiffcultLevel.Normal;
            EventManager.Instance.CallEventImmediately( EventType.SetDiffcultLevel);

        }
        if(m_hardToggle.isOn)
        {
            GameManager.Instance.CurrentDiffcultLevel = DiffcultLevel.Hard;
            EventManager.Instance.CallEventImmediately( EventType.SetDiffcultLevel);
        }
    }

}
