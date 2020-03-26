using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {

	private float timer;
    private float m_modelMoveSpanTime;
    protected float m_modelMoveDist;
    protected bool isInteract;//是否可以进行控制
    protected bool isStop;
    [SerializeField]
    protected Transform pivot;
    [SerializeField]
    protected Transform[] blocks;

    private void Start()
    {
        this.isInteract = true;
        timer = 0;
        m_modelMoveDist = 2.0f;
        m_modelMoveSpanTime = PlayController.Instance.CurrentMoveSpanTime;

    }
    private void Update()
    {
        if (isStop) return;

        timer += Time.deltaTime;
        if(timer>=m_modelMoveSpanTime)
        {
            MoveDown();
            timer = 0;
            m_modelMoveDist = GameManager.Instance.ModelMoveDist ;
        }

    }
    protected void MoveDown()
    {
        Vector3[] pss = new Vector3[this.blocks.Length];
        for (int i = 0; i < pss.Length; i++)
        {
            pss[i] = this.blocks[i].position + new Vector3(0, -m_modelMoveDist, 0);
        }

        if (!PlayController.Instance.isOutofBorder(pss))
        {
            if (PlayController.Instance.isTouch(pss))
            {
                //停止该脚本的运行
                this.enabled = false;
                PlayController.Instance.PlaceBlocks(this.blocks);
            }
            else
            {
                this.transform.position += new Vector3(0, -m_modelMoveDist, 0);
                EventManager.Instance.CallEventImmediately( EventType.PlayBlockMoveAudio);
            }
        }
        else
        {
            //停止该脚本的运行
            this.enabled = false;
            PlayController.Instance.PlaceBlocks(this.blocks);

        }

    }
    public void MoveInput(InputType type)
    {
        if (!isInteract||type == InputType.None) return;

        float moveDist = 0 ; 
        bool isRotation = false;

        switch(type)
        {
            case InputType.Move_Left:
                moveDist = -1.0f;
                break;
            case InputType.Move_Right:
                moveDist = 1.0f;
                break;
            case InputType.Move_Down:

                this.m_modelMoveSpanTime = 0.01f;//快速移动
                this.isInteract = false;
                return;
            case InputType.Rotation:
                isRotation = true;
                break;
        }
        Vector3[] pss = new Vector3[this.blocks.Length];

        if (isRotation)
        {
            Quaternion rotaition = Quaternion.AngleAxis(-90,Vector3.forward);
            //TODO 处理旋转
            for(int i = 0;i<blocks.Length;i++)
            {
                Vector3 pos = blocks[i].position - pivot.position;
                pos = rotaition * pos;
                pss[i] = pos + pivot.position;
            }
        }
        else
        {
            for(int i = 0;i<blocks.Length;i++)
            {
                pss[i] = blocks[i].position + new Vector3(moveDist,0,0);
            }
        }

        if (!PlayController.Instance.isTouch(pss))
        {
            if (!PlayController.Instance.isOutofBorder(pss))
            {
                //可以进行变换
                if(!isRotation)
                    this.transform.position += new Vector3(moveDist,0,0);
                else
                {
                    for(int i = 0;i<blocks.Length;i++)
                    {
                        blocks[i].position = pss[i];
                    }
                }
            }
            else Debug.Log("方块因为超会出了边界，而无法进行左右移动");

        }
        else Debug.Log("方块因为和其他的方块产生了碰撞了，无法进行左右移动");


        EventManager.Instance.CallEventImmediately( EventType.PlayBlockControllAudio);
    }
    public void StopMove()
    {
        this.isStop = true;
    }
    public void RestartMove()
    {
        this.isStop = false;
    }
}
