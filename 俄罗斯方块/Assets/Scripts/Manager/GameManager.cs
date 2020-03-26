using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 难度等级
/// </summary>
public enum DiffcultLevel
{
    Easy,
    Normal,
    Hard
}



/// <summary>
/// 游戏管理器
/// </summary>
public class GameManager : Singleton<GameManager> {

    public int Level { get { return m_level; } }
    public int Score { get { return m_currentScore; } set { this.m_currentScore = value; } }
    public int HeightestScore { get { return m_heightestScore; }set { this.m_heightestScore = value; } }
    public int ElinimateRowCount { get { return m_elinimateRowCount; }set { this.m_elinimateRowCount = value; } }
    public int RowCount { get { return m_rowCount; } }
    public int ColCount { get { return m_colCount; } }
    public int ScorePerRow { get { return m_scorePerRow; } }
    public bool IsStop { get { return m_isStop; } set { this.m_isStop = true; } }
    public float ModelMoveSpanTime { get { return m_modelMoveSpanTime; } }
    public float ModelMoveDist { get { return m_modelMoveDist; } }
    public int MaxLevel { get { return m_diffcultSpeed.Length; } }
    public int LevelUpRequest { get { return m_diffcultRequestToEliminate[Level-1]; } }
    public float NextLeveSpeed { get { return Level + 1 >= MaxLevel ? m_diffcultSpeed[MaxLevel - 1] : m_diffcultSpeed[Level+1]; } }
    public float BaseLevelSpeed { get { return m_diffcultSpeed.Length>0?m_diffcultSpeed[0]:0.0f; } }
    public int NextLevelUpRequestCount { get { return m_nextLevelRequestCount; } }
    public DiffcultLevel CurrentDiffcultLevel { get { return m_diffcultLevel; }set { this.m_diffcultLevel = value; } }


    private int m_level = 1;//等级
    private int m_currentScore;//分数
    private int m_heightestScore;//最高分数
    private int m_elinimateRowCount;//消除的行数
    private int m_nextLevelRequestCount = 0;//下一次升级所需要达到的行数
    private int m_rowCount = 21;//行数----------------------------额外添加一行方便处理
    private int m_colCount = 10;//列数
    private bool m_isStop = false;
    private DiffcultLevel m_diffcultLevel;//当前的难度等级

    [SerializeField]
    private float m_modelMoveSpanTime = 0.5f;//方格移动的时间间隔
    [SerializeField]
    private float m_modelMoveDist = 1;//方格移动的距离
    [SerializeField]
    private int m_scorePerRow = 10;//消除每一行的分数增加值

    private int[] m_diffcultRequestToEliminate;
    private float[] m_diffcultSpeed;//难度速度

    [SerializeField]
    private int[] m_easyDiffcultRequestToEliminate;
    [SerializeField]
    private float[] m_easyDiffcultSpeed;
    [SerializeField]
    private int[] m_normalDiffcultRequestToEliminate;
    [SerializeField]
    private float[] m_normalFiffcultSpeed;
    [SerializeField]
    private int[] m_hardDiffcultRequestToEliminate;
    [SerializeField]
    private float[] m_hardDiffcultSpeed;

   
    public void Init()
    {
        //保存到文件中
        this.m_diffcultLevel = DiffcultLevel.Easy;
        SetDiffcultLevel();

        this.m_nextLevelRequestCount = m_diffcultRequestToEliminate[0];
    }
    /// <summary>
    /// 通过消除的行数来增加分数值
    /// </summary>
    public void IncreaseScore(int count)
    {
        if (count == 0) return;
        m_currentScore += count * m_scorePerRow;
        m_heightestScore = m_currentScore > m_heightestScore ? m_currentScore : m_heightestScore;
        m_elinimateRowCount += count;
        

        //判断是否可以进行升级
        if(this.m_level<this.MaxLevel&&m_nextLevelRequestCount<=this.ElinimateRowCount)
        {
            IncreaseLevel(); 
        }


        //通知UI进行更新。
        EventManager.Instance.CallEventDelay( EventType.EliminateRow);
    }
    public void IncreaseLevel()
    {
        //判断要升到那一级
        
        for(int i = m_diffcultRequestToEliminate.Length-1; i>=0;i--)
        {
            if(m_diffcultRequestToEliminate[i]<=this.m_elinimateRowCount)
            {
                this.m_level = i+2;
                break;
            }
        }

        Debug.Log("要升级到的等级是"+this.m_level);

        //调用升级事件
        EventManager.Instance.CallEventImmediately( EventType.LevelUp);
        if (this.m_level < GameManager.Instance.MaxLevel)
        {
            this.m_nextLevelRequestCount = m_diffcultRequestToEliminate[this.m_level-1];

            
        }
        else
            m_nextLevelRequestCount = int.MinValue;

        //更新PlayController中的方块下落的速度变量
        PlayController.Instance.ChangeMoveSpanTime(m_diffcultSpeed[this.m_level - 1]);
    }
    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        this.m_currentScore = 0;
        this.m_elinimateRowCount = 0;
        this.m_level = 1;
        this.m_nextLevelRequestCount = m_diffcultRequestToEliminate[0];
    }
    /// <summary>
    /// 设置难度等级
    /// </summary>
    public void SetDiffcultLevel()
    {
        switch(this.m_diffcultLevel)
        {
            case DiffcultLevel.Easy:
                this.m_diffcultRequestToEliminate = m_easyDiffcultRequestToEliminate;
                this.m_diffcultSpeed = m_easyDiffcultSpeed;
                break;
            case DiffcultLevel.Normal:
                this.m_diffcultRequestToEliminate = m_normalDiffcultRequestToEliminate;
                this.m_diffcultSpeed = m_normalFiffcultSpeed;
                break;
            case DiffcultLevel.Hard:
                this.m_diffcultRequestToEliminate = m_hardDiffcultRequestToEliminate;
                this.m_diffcultSpeed = m_hardDiffcultSpeed;
                break;
        }

        //重新设值当前的分数
        this.m_level = 1;
        for (int i = m_diffcultRequestToEliminate.Length - 1; i >= 0; i--)
        {
            if (m_diffcultRequestToEliminate[i] <= this.m_elinimateRowCount)
            {
                this.m_level = i + 2;
                break;
            }
        }

        Debug.Log("要升级到的等级是" + this.m_level);
        if (this.Level < this.MaxLevel)
            this.m_nextLevelRequestCount = this.m_diffcultRequestToEliminate[this.m_level - 1];
        else this.m_nextLevelRequestCount = int.MinValue;

        //更新PlayController中的方块下落的速度变量
        PlayController.Instance.ChangeMoveSpanTime(m_diffcultSpeed[this.m_level-1]);
    }
}
