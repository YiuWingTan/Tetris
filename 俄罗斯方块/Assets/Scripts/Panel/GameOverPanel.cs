using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
    [SerializeField]
    private Button m_continueGameButton;
    [SerializeField]
    private Button m_quitGameButton;


    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        this.isOverLap = true;

        m_quitGameButton.onClick.AddListener(()=> { EventManager.Instance.CallEventImmediately(EventType.QuitGame);
            EventManager.Instance.CallEventImmediately(EventType.QuitGame);
        });
        m_continueGameButton.onClick.AddListener(() => { EventManager.Instance.CallEventImmediately(EventType.ReStartGame);
            EventManager.Instance.CallEventImmediately(EventType.PlayCursorAudio);
        });

    }
}
