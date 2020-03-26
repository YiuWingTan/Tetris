using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//事件类型
public enum EventType
{
    None = 0,
    StopGame,//暂停游戏
    ContinueGame,//继续游戏
    EliminateRow,//当消除掉一行的时候进行调用
    CreateBlock,//创建一个新的方块的时候
    GameOver,//游戏结束
    QuitGame,
    ReStartGame,
    PlayCursorAudio,
    PlayBlockGenernateAudio,
    PlayBlockEliminateAudio,
    PlayBlockMoveAudio,
    PlayBlockControllAudio,
    ChangeAudioValue,
    ActiveAudio,
    LevelUp,
    SetDiffcultLevel,//设置难度等级
    StartGame,//开始游戏
}


/// <summary>
/// MVC 控制器类
/// </summary>
public class MVCController : Singleton<MVCController> {

    private MVCViewer view;


    protected override void Awake()
    {
        base.Awake();
        view = new MVCViewer();

       
        
    }

    private void Start()
    {
        //初始化所有的管理器
        EventManager.Instance.Init();
        RegisterPanels();
        RegisterEvent();
        UIManager.Instance.Init();
        GameManager.Instance.Init();
        AudioManager.Instance.Init();
        InputDetecter.Instance.Init();
        PlayController.Instance.Init();

        
    }

    public void RegisterPanels()
    {
        var panelObs = GameObject.FindGameObjectsWithTag("Panel");

        foreach(var panelOb in panelObs)
        {
            var panel = panelOb.GetComponent<BasePanel>();

            if (view.PanelContainer.ContainsKey(panel.panelType)) continue;

            panel.Init();//初始化

            view.PanelContainer.Add(panel.panelType,panel);

            if (!panel.isOpen) panel.gameObject.SetActive(false);
        }

        
    }
    /// <summary>
    /// 注册UI事件
    /// </summary>
    public void RegisterEvent()
    {
        //TODO 注册事件
        EventManager.Instance.RegisterEvent(EventType.StopGame, StopGame);
        EventManager.Instance.RegisterEvent(EventType.EliminateRow,EliminateRow);
        EventManager.Instance.RegisterEvent(EventType.ContinueGame,ContinueGame);
        EventManager.Instance.RegisterEvent( EventType.CreateBlock, CreateNewBlock);
        EventManager.Instance.RegisterEvent(EventType.GameOver, GameOver);
        EventManager.Instance.RegisterEvent(EventType.QuitGame,QuitGame);
        EventManager.Instance.RegisterEvent(EventType.ReStartGame,RestartGame);
        EventManager.Instance.RegisterEvent(EventType.PlayBlockEliminateAudio,PlayBlockEliminateAudio);
        EventManager.Instance.RegisterEvent(EventType.PlayBlockGenernateAudio,PlayBlockGenernateAudio);
        EventManager.Instance.RegisterEvent(EventType.PlayBlockMoveAudio,PlayBlockMoveAudio);
        EventManager.Instance.RegisterEvent(EventType.PlayCursorAudio,PlayCursorAudio);
        EventManager.Instance.RegisterEvent(EventType.ChangeAudioValue, ChangeAudioValue);
        EventManager.Instance.RegisterEvent(EventType.PlayBlockControllAudio, PlayBlockControllAudio);
        EventManager.Instance.RegisterEvent(EventType.LevelUp,LevelUp);
        EventManager.Instance.RegisterEvent(EventType.SetDiffcultLevel, SetDiffcultLevel);
        EventManager.Instance.RegisterEvent(EventType.StartGame,StartGame);
    }

    //TODO 编写一系列的UI方法

    //开启关闭UI方法
    public void OpenPanel(PanelType type)
    {
        if (type == PanelType.None) return;
        if (!view.PanelContainer.ContainsKey(type))
        {
            Debug.Log("没有这个面板");
            return;
        }

        UIManager.Instance.ShowPanel(view.PanelContainer[type]);
        StopGame();
    }
    public void ClosePanel(PanelType type)
    {
        if (type == PanelType.None) return;
        if (view.PanelContainer.ContainsKey(type) == false) return;

        UIManager.Instance.ClosePanel(view.PanelContainer[type]);
        //当所有的面板都被关闭的时候才继续游戏
        if(UIManager.Instance.isAllPanelClose())
            ContinueGame();
    }
    public void CloseAllPanel()
    {
        UIManager.Instance.CloseAllPanel();
        //当所有的面板都被关闭的时候才继续游戏
        if (UIManager.Instance.isAllPanelClose())
            ContinueGame();
    }
    
    /// <summary>
    /// 暂停游戏时调用的事件
    /// </summary>
    private void StopGame()
    {
        //停止方块的生成
        //停止现在方块的移动
        //停止输入检测器
        GameManager.Instance.IsStop = true;
        PlayController.Instance.StopPlay();
        InputDetecter.Instance.StopInputDetect();
    }
    /// <summary>
    /// 将游戏从暂停状态变回游戏状态
    /// </summary>
    private void ContinueGame()
    {
        GameManager.Instance.IsStop = false;
        PlayController.Instance.ContinuePlay();
        InputDetecter.Instance.ContinueInputDetect();
    }
    /// <summary>
    /// 
    /// </summary>
    private void EliminateRow()
    {
        //当消除了一行的时候触发的方法
        //检查是否需要增加难度


        //进行当前分数的增加
        //进行UI行数的修改
        int[] datas = new int[6];

        datas[0] = GameManager.Instance.Score;
        datas[1] = GameManager.Instance.HeightestScore;
        datas[2] = GameManager.Instance.ElinimateRowCount;
        datas[3] = GameManager.Instance.Level;
        datas[4] = GameManager.Instance.NextLevelUpRequestCount;
        datas[5] = -1;//不进行下一个方块的更新
        view.PanelContainer[PanelType.InformationPanel].UpdatePanelInfo(datas);

       

    }
    /// <summary>
    /// 当要生成一个新的方块的时候触发的事件
    /// </summary>
    private void CreateNewBlock()
    {
        int[] datas = new int[6];
        datas[0] = -1;
        datas[1] = -1;
        datas[2] = -1;
        datas[3] = -1;
        datas[4] = -1;
        datas[5] = PlayController.Instance.nextBlockIndex;
        view.PanelContainer[PanelType.InformationPanel].UpdatePanelInfo(datas);//进行下一个方块的显示更新。
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        //进行UI界面的更新
        int[] datas = new int[6];

        datas[0] = GameManager.Instance.Score;
        datas[1] = GameManager.Instance.HeightestScore;
        datas[2] = GameManager.Instance.ElinimateRowCount;
        datas[3] = GameManager.Instance.Level;
        datas[4] = GameManager.Instance.NextLevelUpRequestCount;
        datas[5] = -1;//不进行下一个方块的更新
        view.PanelContainer[PanelType.InformationPanel].UpdatePanelInfo(datas);
        //Debug.LogError("更新了UI面板的显示");
    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    private void GameOver()
    {
        CloseAllPanel();
        StopGame();
        OpenPanel(PanelType.GameOverPanel);

    }
    /// <summary>
    /// 重新开始游戏
    /// </summary>
    private void RestartGame()
    {
        //TODO 重新开始游戏的实现
        
        //重新设置所有的数据
        PlayController.Instance.ReStart();
        GameManager.Instance.RestartGame();
        //设置面板参数
        int[] datas = new int[6];

        datas[0] = GameManager.Instance.Score;
        datas[1] = GameManager.Instance.HeightestScore;
        datas[2] = GameManager.Instance.ElinimateRowCount;
        datas[3] = GameManager.Instance.Level;
        datas[4] = GameManager.Instance.NextLevelUpRequestCount;
        datas[5] = -1;
        view.PanelContainer[PanelType.InformationPanel].UpdatePanelInfo(datas);
        CloseAllPanel();//关闭所有的面板
        ContinueGame();
    }
    /// <summary>
    /// 退出游戏
    /// </summary>
    private void QuitGame()
    {
        //TODO 
        Application.Quit();
    }
    private void PlayCursorAudio()
    {
        AudioManager.Instance.PlayCursorAudio();
    }
    private  void PlayBlockGenernateAudio()
    {
        AudioManager.Instance.PlayBallonAudio();
    }
    private void PlayBlockMoveAudio()
    {
        AudioManager.Instance.PlayBlockMoveAudio();
    }
    private void PlayBlockEliminateAudio()
    {
        AudioManager.Instance.PlayBlockEliminate();
    }
    private void PlayBlockControllAudio()
    {
        AudioManager.Instance.PlayBlockControllAudio();
    }
    private void ChangeAudioValue()
    {
        var panel = view.PanelContainer[PanelType.AudioPanel] as AudioPanel;
        AudioManager.Instance.SetAudioValue(panel.CurrentAudioValue);
    }
    /// <summary>
    /// 数据的改变导致显示的改变
    /// </summary>
    private void LevelUp()
    {
        //显示升级面板
        var panel = this.view.PanelContainer[PanelType.LevelUpPanel];
        if (!panel.gameObject.activeInHierarchy)
        {
            //进行升级面板的显示
            panel.gameObject.SetActive(true);
            StopGame();
        }
    }
    private void SetDiffcultLevel()
    {

        //将GameManager中数据进行更新
        GameManager.Instance.SetDiffcultLevel();
        //将ui面板进行更新
        int []datas = new int[6];

        datas[0] = GameManager.Instance.Score;
        datas[1] = GameManager.Instance.HeightestScore;
        datas[2] = GameManager.Instance.ElinimateRowCount;
        datas[3] = GameManager.Instance.Level;
        datas[4] = GameManager.Instance.NextLevelUpRequestCount;
        datas[5] = -1;//不进行下一个方块的更新
        view.PanelContainer[PanelType.InformationPanel].UpdatePanelInfo(datas);
        
    }
}
