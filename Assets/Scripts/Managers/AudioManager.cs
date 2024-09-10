using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;
    [SerializeField] private float sfxMinimumDistance;

    public bool playBgm;
    private int bgmIndex;
    private bool canPlaySFX;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        Invoke("AllowSFX", .1f);     //让游戏延迟一秒后才能发出声效
    }

    private void Update()
    {
        if (!playBgm)
        {
            StopAllBgm();
        }
        else
        {
            if (!bgm[bgmIndex].isPlaying)
                PlayBgm(bgmIndex);
        }
    }

    public void PlaySfx(int _index, Transform _source)
    {
        //if (sfx[_index].isPlaying)        //防止出现多个相同物体同时发出声音
        //return;

        if (canPlaySFX == false)
            return;

        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinimumDistance)
            return;

        if (_index < sfx.Length)
        {
            sfx[_index].pitch = Random.Range(.85f, 1.1f);
            sfx[_index].Play();
        }
    }

    public void StopSfx(int _index) => sfx[_index].Stop();

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBgm(bgmIndex);
    }

    public void PlayBgm(int _index)
    {
        bgmIndex = _index;

        StopAllBgm();

        if (_index < sfx.Length)
        {
            bgm[_index].Play();
        }
    }

    public void StopAllBgm()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    private void AllowSFX() => canPlaySFX = true;

    public void StopSFXWithTime(int _index)
    {
        StartCoroutine(DecreaseVolume(sfx[_index]));
    }

    private IEnumerator DecreaseVolume(AudioSource _audio)      //退出区域后，声音缓慢减少
    {
        float defaultVolume = _audio.volume;
        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.3f);
            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }
}
