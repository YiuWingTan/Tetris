using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVCViewer {

    private Dictionary<PanelType, BasePanel> m_panels;
    public Dictionary<PanelType, BasePanel> PanelContainer;

    public MVCViewer()
    {
        m_panels = new Dictionary<PanelType, BasePanel>();
        PanelContainer = new Dictionary<PanelType, BasePanel>();
    }

    //TODO 编写一系列的UI更新消息
    
}
