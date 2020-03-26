using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Grid
{
    public Transform transform;
    public Vector3 pos;
}

/// <summary>
/// 游戏逻辑处理控制器
/// </summary>
public class PlayController : Singleton<PlayController> {

    
    [SerializeField]
    private Transform[] blocks;
    [SerializeField]
    private float effectSpanTime;//效果持续时间
    [SerializeField]
    private Transform shapesContainer;//方块的容器
    [SerializeField]
    private List<GameObject> m_modle;//方块模型
    [SerializeField]
    private bool isCreate;//是否可以生成
    [SerializeField]
    private GameObject eliminateGo;//消除时的效果对象
    private bool isStop;
    [SerializeField]
    private Model m_currentModel;//当前正在控制的方块
    private Grid[,] mapContainer;
    private Vector2 startPoint;//起始点
    private float createMinIndex;
    private float createMaxIndex;
    private float createY;
    [SerializeField]
    private float m_moveSpanTime;//移动的时间间隔
    private float m_moveDist;//移动的距离
    private int endRow;//结束行
    private float timer;
    private int[] rowCountTable;
    private int currentCreateIndex;//当前需要进行创建的方块的编号
    private int nextCreateIndex;//下一个要进行创建的方块的编号。

    /// <summary>
    /// 获取下一个将要生成的方块
    /// </summary>
    public int nextBlockIndex
    {
        get
        {
            return nextCreateIndex;
        }
    }
    public float CurrentMoveSpanTime
    {
        get { return this.m_moveSpanTime; }
    }

    public void Init()
    {
        this.mapContainer = new Grid[GameManager.Instance.RowCount,GameManager.Instance.ColCount];
        this.endRow = GameManager.Instance.RowCount - 2;
        this.m_moveDist = GameManager.Instance.ModelMoveDist;
        this.m_moveSpanTime = GameManager.Instance.BaseLevelSpeed;
        this.rowCountTable = new int[GameManager.Instance.RowCount];
        this.currentCreateIndex = 0;
        this.nextCreateIndex = GetRandomIndex();
        this.timer = 0;

        for (int i = 0; i < rowCountTable.Length; i++) rowCountTable[i] = 0;

        for(int k = 0;k<blocks.Length;k++)
        {
            int i = k/10;
            int j = k%10;

            mapContainer[i, j].pos = blocks[k].position;
            mapContainer[i, j].transform = null;
        }

        for (int j = 0; j < GameManager.Instance.ColCount; j++)
            mapContainer[endRow + 1, j].pos = mapContainer[endRow,j].pos+new Vector3(0,1,0);

        startPoint = mapContainer[0,0].pos;

        createMinIndex = 1;
        createMaxIndex = GameManager.Instance.ColCount - 3;
        createY = mapContainer[GameManager.Instance.RowCount-1,0].pos.y+2;

        //发出开始游戏的消息
        EventManager.Instance.CallEventImmediately( EventType.StartGame);
    }

    private void Update()
    {
        CreateModel();
    }

    public void ReStart()
    {
        //重新开始游戏
        //删除所有的方格
        for(int i = 0;i<GameManager.Instance.RowCount;i++)
        {
            for (int j = 0; j < GameManager.Instance.ColCount; j++)
                mapContainer[i, j].transform = null;
        }
        for(int i = 0;i<shapesContainer.childCount;i++)
        {
            var go = shapesContainer.GetChild(i).gameObject;
            GameObject.Destroy(go);
        }

        //设置相关的参数
        this.currentCreateIndex = 0;
        this.nextCreateIndex = GetRandomIndex();
        this.timer = 0;
        this.m_moveSpanTime = GameManager.Instance.BaseLevelSpeed;
    }

    /// <summary>
    /// 生成方块
    /// </summary>
    private void CreateModel()
    {
        if (isStop) return;
        if (!isCreate) return;

        this.currentCreateIndex = this.nextCreateIndex;

        Vector3 pos = new Vector3(0,0,-6);
        int index = m_modle.Count;
        index = (int)Random.Range(createMinIndex, createMaxIndex);
        Vector3 targetPos = mapContainer[GameManager.Instance.RowCount - 1, index].pos;
        if (currentCreateIndex == 0 || currentCreateIndex == 1)
            pos.x = targetPos.x + 0.5f;
        else pos.x = targetPos.x;

        pos.y = createY;

        if (currentCreateIndex == 0 || currentCreateIndex == 5 || currentCreateIndex == 6)
            pos.y += 0.5f;

        m_currentModel = GameObject.Instantiate(m_modle[currentCreateIndex],pos,Quaternion.identity).GetComponent<Model>();
        this.nextCreateIndex = GetRandomIndex();//获取一个随机的方块编号
        //进行UI事件的调用
        EventManager.Instance.CallEventDelay(EventType.CreateBlock);
        isCreate = false;
        //将所有的形状放在容器中。
        m_currentModel.transform.parent = this.shapesContainer;

        //播放方块生成声音
        EventManager.Instance.CallEventImmediately( EventType.PlayBlockGenernateAudio);
    }
    private int GetRandomIndex()
    {
        return Random.Range(0,m_modle.Count);
    }
    /// <summary>
    /// 停止游戏
    /// </summary>
    public void StopPlay()
    {
        isStop = true;
        isCreate = false;
        if (m_currentModel != null) m_currentModel.StopMove();
    }
    /// <summary>
    /// 继续游玩
    /// </summary>
    public void ContinuePlay()
    {
        this.isStop = false;
        if (m_currentModel == null) this.isCreate = true;
        else
        {
            this.isCreate = false;
            m_currentModel.RestartMove();
        }

    }
    public bool isTouch(Vector3[] pss)
    {
        foreach(var pos in pss)
        {
            int i = Mathf.RoundToInt((pos.y - startPoint.y));
            int j = Mathf.RoundToInt(pos.x - startPoint.x);

            if (i >= GameManager.Instance.RowCount|| i < 0)
            {
                Debug.Log("isTouch 方法中i索引超出范围");
                continue;
            }
            if(j>=GameManager.Instance.ColCount||j<0)
            {
                Debug.Log("isTouch 方法中j索引超出范围");
                continue;
            }

            if (mapContainer[i, j].transform != null) return true;//接触到了另外一个物体
        }


        return false;
    }
    /// <summary>
    /// 是否越过了边界
    /// </summary>
    /// <returns></returns>
    public bool isOutofBorder(Vector3[]pss)
    {

        foreach (var pos in pss)
        {
            int i = Mathf.RoundToInt((pos.y - startPoint.y));
            int j = Mathf.RoundToInt(pos.x - startPoint.x);

            //Debug.Log("out of border 中 pos = "+pos+" startPoint = "+startPoint+" i ="+i+" j ="+j);

            if (i < 0)
            {
                Debug.Log("OutofBorder中 i 超过了边界");
                return true;
            }
            if (j >= GameManager.Instance.ColCount || j < 0)
            {
                Debug.Log("OutofBorder中 j 超过了边界");
                return true;
            }
        }

        return false;
    } 
    /// <summary>
    /// 放置方块
    /// </summary>
    public void PlaceBlocks(Transform[]blocks)
    {
        foreach(var block in blocks)
        {
            int i = (int)(block.position.y - startPoint.y+0.5f);
            int j = (int)(block.position.x - startPoint.x+0.5f);

            if (i < 0 || i >= GameManager.Instance.RowCount) {
                throw new System.Exception("放置的目标超出了边界"+"i = "+i);
            }
            if (j < 0 || j >= GameManager.Instance.ColCount) {
                throw new System.Exception("被放置的目标超出了边界" + " j = " + j);
            }

            mapContainer[i, j].transform = block;
            
            block.position = mapContainer[i,j].pos;
        }
        m_currentModel = null;
        //TODO 判断是否有可以进行消除的

        if(checkEliminate())
        {
            StartCoroutine(reMoveRow());
            checkIsGameOver();

        }
        else if(blocks.Length != 0)
        {
            isCreate = !checkIsGameOver();
        }

    }
    public void isGameOver()
    {
        //TODO 游戏结束
    }
    public void InputMove(InputType type)
    {
        if (m_currentModel != null) m_currentModel.MoveInput(type);
    }
    public void ChangeMoveSpanTime(float time)
    {
        this.m_moveSpanTime = time;
    }
    /// <summary>
    /// 检查行
    /// </summary>
    private bool checkEliminate()
    {
        int row = GameManager.Instance.RowCount-1;
        int col = GameManager.Instance.ColCount-1;
        int count = 0;
        bool isEliminate = false;
        
        for(int i = 0;i<=row;i++)
        {
            bool isFind = true;
            for(int j = 0;j<=col;j++)
            {
                if (mapContainer[i, j].transform == null)
                {
                    isFind = false;
                    break;
                }
            }
            if(isFind)
            {
                //增加计数
                count++;
                //将这一行进行消除
                Debug.LogWarning("找到一行需要进行删除的行,行数为 "+i);
                for(int k = 0;k<=col;k++)
                {
                    GameObject.Destroy(mapContainer[i,k].transform.gameObject);
                    mapContainer[i,k].transform = null;
                }
                //创建消除效果
                Vector3 pos = new Vector3(mapContainer[i, 0].pos.x, mapContainer[i, 0].pos.y,-10);
                var go =  GameObject.Instantiate(eliminateGo,pos,Quaternion.identity);
                GameObject.Destroy(go,effectSpanTime);
                //添加记录
                for(int k = i+1;k<rowCountTable.Length; k++)
                {
                    rowCountTable[k]++;
                }
                isEliminate = true;
            }
        }
        //增加当前的分数
        if(count>0)
        GameManager.Instance.IncreaseScore(count);

        //判断是否可以进行消除
        return isEliminate;
        
    }
    /// <summary>
    /// 检查游戏是否已经结束
    /// </summary>
    private bool checkIsGameOver()
    {
        int i = GameManager.Instance.RowCount - 2;
        for(int j =0;j<GameManager.Instance.ColCount;j++ )
        {
            if(mapContainer[i,j].transform!=null)
            {
                //游戏结束
                EventManager.Instance.CallEventImmediately( EventType.GameOver);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否要增加难度
    /// </summary>
    IEnumerator reMoveRow()
    {
        EventManager.Instance.CallEventImmediately( EventType.PlayBlockEliminateAudio);
        //等待效果消除
        yield return new WaitForSeconds(effectSpanTime);

        for(int i = 0;i< rowCountTable.Length;i++)
        {
            if(rowCountTable[i]>0)
            {
                int targetRow = i - rowCountTable[i];
                if(targetRow<0)
                {
                    throw new System.Exception("要移动的目标行小于0");
                }
                for(int j = 0;j<GameManager.Instance.ColCount;j++)
                {
                    if(mapContainer[i,j].transform!=null)
                    {
                        mapContainer[targetRow, j].transform = mapContainer[i, j].transform;
                        mapContainer[i, j].transform.position = mapContainer[targetRow, j].pos;
                        mapContainer[i, j].transform = null;
                    }
                }
                rowCountTable[i] = 0;
            }
        }
        isCreate = true;
        yield return null;
    }



}
