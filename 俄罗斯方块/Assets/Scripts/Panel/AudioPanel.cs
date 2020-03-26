using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPanel : BasePanel {

    [SerializeField]
    private Button m_closeButton;
    [SerializeField]
    private Slider m_AudioValueSlider;
    public float CurrentAudioValue { get { return m_AudioValueSlider.value; } }


    public override void Init()
    {
        base.Init();
        this.panelType = PanelType.AudioPanel;

        m_closeButton.onClick.AddListener(()=>{ MVCController.Instance.ClosePanel(this.panelType);EventManager.Instance.CallEventImmediately( EventType.PlayCursorAudio);
            Debug.Log("关闭AudioPanels"); });
        m_AudioValueSlider.onValueChanged.AddListener(SliderValueChange);
    }

    public void SliderValueChange(float value)
    {
        if(this.gameObject.activeInHierarchy)
         EventManager.Instance.CallEventImmediately( EventType.ChangeAudioValue);
    }

}
