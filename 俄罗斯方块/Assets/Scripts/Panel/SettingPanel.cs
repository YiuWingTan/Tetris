using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel {

    [SerializeField]
    private Button settingButton;
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Button audioPanelButton;
    [SerializeField]
    private Button diffcultButton;
    [SerializeField]
    private Button endGameButton;

    public override void Init()
    {
        base.Init();
        this.panelType = PanelType.SettingPanel;

        settingButton.onClick.AddListener(() => {
            MVCController.Instance.OpenPanel(this.panelType);
            settingButton.enabled = false;
            EventManager.Instance.CallEventImmediately(EventType.PlayCursorAudio);
        });
        closeButton.onClick.AddListener(()=> { MVCController.Instance.ClosePanel(this.panelType);settingButton.enabled = true;
            EventManager.Instance.CallEventImmediately(EventType.PlayCursorAudio); Debug.Log("关闭SettingPanel"); });
        audioPanelButton.onClick.AddListener(()=> { MVCController.Instance.OpenPanel(PanelType.AudioPanel);
            EventManager.Instance.CallEventImmediately(EventType.PlayCursorAudio); Debug.Log("打开了AudioPanel"); });
        diffcultButton.onClick.AddListener(()=> { MVCController.Instance.OpenPanel(PanelType.DiffcultPanel);
            EventManager.Instance.CallEventImmediately(EventType.PlayCursorAudio); Debug.Log("打开了DiffcultPanel"); });
        endGameButton.onClick.AddListener(()=> { EventManager.Instance.CallEventImmediately(EventType.QuitGame); });
    }


    public override void EnablePanel()
    {
        base.EnablePanel();
   
        closeButton.interactable = true;
        audioPanelButton.interactable = true;
        diffcultButton.interactable = true;

    }
    public override void UnablePanel()
    {
        base.UnablePanel();
       
        closeButton.interactable = false;
        audioPanelButton.interactable = false;
        diffcultButton.interactable = false;
    }
    public override void UpdatePanelInfo(int[] data)
    {
        base.UpdatePanelInfo(data);
    }
}
