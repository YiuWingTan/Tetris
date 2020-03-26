using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    //AudioSource的数量
    [SerializeField]
    private int m_audioSourceNumber = 5;
    [SerializeField]
    private AudioClip m_cursor;
    [SerializeField]
    private AudioClip m_drop;
    [SerializeField]
    private AudioClip m_ballon;
    [SerializeField]
    private AudioClip m_linerclear;
    [SerializeField]
    private bool m_isMute;
    private List<AudioSource> m_sources;

    public void Init()
    {
        m_sources = new List<AudioSource>();
        for (int i = 0; i < m_audioSourceNumber; i++)
        {
            m_sources.Add(this.gameObject.AddComponent<AudioSource>());
            m_sources[i].playOnAwake = false;
        }
        this.m_isMute = false;
    }

    // 0号进行鼠标点击
    public void PlayCursorAudio()
    {
        PlayAudio(0,m_cursor);
    }
    //1 好播放方块生成声音
    public void PlayBallonAudio()
    {
        PlayAudio(1,m_ballon);
    }
    //2号播放方块移动声音
    public void PlayBlockMoveAudio()
    {
        //当玩家控制音效正在播放的时候不进行播放
        PlayAudio(2,m_drop);
    }
    //3 号播放方块消除的声音
    public void PlayBlockEliminate()
    {
        PlayAudio(3,m_linerclear);
    }
    //4 号 播放玩家移动方块的声音
    public void PlayBlockControllAudio()
    {
        //当前正在播放下落音效的时候停止其播放
        PlayAudio(4, m_ballon);
    }

    private void PlayAudio(int index, AudioClip clip)
    {
        //Debug.LogWarning("播放了声音");
        if (this.m_isMute) return;//当前静音
        if (index >= m_sources.Count) { throw new System.Exception("当前指定的AudioSource编号超出范围"); }
        if (clip == null) throw new System.Exception("要进行播放的音频文件是空的");

        m_sources[index].clip = clip;
        m_sources[index].Play();
    }
    public void ActiveAudio()
    {
        if (this.m_isMute) m_isMute = false;
    }
    public void SetAudioValue(float value)
    {
        foreach (var audios in m_sources)
        {
            audios.volume = value;
        }
        if (value <= 0.0)
            this.m_isMute = true;
        else 
            this.m_isMute = false;
    }

}
