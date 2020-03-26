using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InputType
{
    None,
    Move_Left,
    Move_Right,
    Move_Down,
    Rotation
}


/// <summary>
/// 输入探测器-需要进行安卓适配
/// </summary>
public class InputDetecter : Singleton<InputDetecter> {

    private bool m_isStop;

    [SerializeField]
    private Button m_leftButton;
    [SerializeField]
    private Button m_rightButton;
    [SerializeField]
    private Button m_rotationButton;
    [SerializeField]
    private Button m_downButton;

    public void Init()
    {
        this.m_isStop = false;
        m_leftButton.onClick.AddListener(()=> { PlayController.Instance.InputMove( InputType.Move_Left); });
        m_rightButton.onClick.AddListener(()=> { PlayController.Instance.InputMove( InputType.Move_Right); });
        m_rotationButton.onClick.AddListener(()=> { PlayController.Instance.InputMove(InputType.Rotation); });
        m_downButton.onClick.AddListener(()=> { PlayController.Instance.InputMove( InputType.Move_Down); });



#if UNITY_STANDALONE_WIN
        //在PC环境下隐藏
        m_leftButton.gameObject.SetActive(false);
        m_rightButton.gameObject.SetActive(false);
        m_downButton.gameObject.SetActive(false);
        m_rotationButton.gameObject.SetActive(false);
#endif
    }

    private void Update()
    {
#if UNITY_ANDROID
       //TODO 手机输入测试

        
#endif

#if UNITY_IPHONE
        Debug.Log("这里是苹果设备>_<");
#endif

#if UNITY_STANDALONE_WIN
        if (isStop) return;


        var inputType = DetectComputerInput();

        if (inputType == InputType.None) return;

        PlayController.Instance.InputMove(inputType);
#endif

    }

    /// <summary>
    /// 检测电脑输入
    /// </summary>
    private InputType DetectComputerInput()
    {
        if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return InputType.Move_Left;
        }
        if(Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow))
        {
            return InputType.Move_Right;
        }
        if(Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.DownArrow))
        {
            return InputType.Move_Down;
        }
        if(Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
        {
            return InputType.Rotation;
        }
       

        return InputType.None;
    }

    /// <summary>
    /// 检测手机的输入
    /// </summary>
    /// <returns></returns>
    private InputType DetectPhoneInput()
    {
        return InputType.None;
    }

    public void StopInputDetect()
    {
        this.m_isStop = true;
    }
    public void ContinueInputDetect()
    {
        this.m_isStop = false;
    }
}
