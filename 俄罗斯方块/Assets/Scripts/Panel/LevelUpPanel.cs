using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 升级面板-进行显示后消失
/// </summary>
public class LevelUpPanel : BasePanel {

    private float m_timer;
    [SerializeField]
    private float m_spanTime;
    [SerializeField]
    private Text m_text;
    private Image m_Image;
    

    public override void Init()
    {
        base.Init();
        this.panelType = PanelType.LevelUpPanel;
        m_Image = this.gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        //对透明度进行插值
        Color image_color = m_Image.color;
        Color text_color = m_text.color;
        text_color.a = image_color.a = Mathf.Lerp(1, 0,m_timer/m_spanTime);
        //Debug.Log(text_color.a);
        //Debug.LogWarning(color);
        m_Image.color = image_color;
        m_text.color = text_color;

        if(m_timer>=m_spanTime)
        {
            //将面板进行隐藏
            this.gameObject.SetActive(false);
            text_color.a = 255;
            image_color.a = 255;
            m_Image.color = image_color;
            m_text.color = text_color;
            m_timer = 0;
            EventManager.Instance.CallEventImmediately( EventType.ContinueGame);
        }
    }

}
