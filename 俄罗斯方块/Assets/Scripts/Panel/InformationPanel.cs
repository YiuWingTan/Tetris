using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 信息面板
/// </summary>
public class InformationPanel : BasePanel {

    [SerializeField]
    private Text m_currentScore;
    [SerializeField]
    private Text m_heightestScore;//当前最高分
    [SerializeField]
    private Text m_rowNums;//行数
    [SerializeField]
    private Text m_currentLevel;//当前等级
    [SerializeField]
    private Text m_levepUpRequstText;
    [SerializeField]
    private GameObject m_nextBlock;//下一个方块
    [SerializeField]
    private GameObject[] m_uiShapesContainer;//保存所有的UI形状。
    [SerializeField]
    private Transform m_nextBlockPos;//下一个方块的保存位置。


    public override void Init()
    {
        this.isActive = true;
        this.isOpen = true;
        UnablePanel();
    }

    public override void UpdatePanelInfo(int[] data)
    {
        if (data.Length == 0) return;
        if(data[0]>=0)
            m_currentScore.text = data[0].ToString();
        if (data[1] >= 0)
            m_heightestScore.text = data[1].ToString();
        if (data[2] >= 0)
            m_rowNums.text = data[2].ToString();
        if (data[3] >= 0)
            m_currentLevel.text = data[3].ToString();
        if (data[4] >= 0)
            m_levepUpRequstText.text = data[4].ToString();
        else if (data[4] == int.MinValue)
            m_levepUpRequstText.text = "已经达到最高难度了";
        if(data[5]>=0)
        {
            //进行下一个方块的显示更新.

            //删除上一个显示的方块
            if (m_nextBlock != null) GameObject.Destroy(m_nextBlock);
            m_nextBlock = GameObject.Instantiate(m_uiShapesContainer[data[5]]);
            m_nextBlock.transform.parent = m_nextBlockPos;
            m_nextBlock.transform.localScale = new Vector3(35,35,0);
            //m_nextBlock.transform.localRotation = new Quaternion();
            m_nextBlock.transform.localPosition = new Vector3();
        }
    }
    
}
