using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class AudioPlayControl : Singleton<AudioPlayControl>
{
    private AudioSource audioSource;
    private AudioSource SecondSource;
    private AudioSource MainSource;
    private List<AudioSource> AudioList = new List<AudioSource>();
    private int AudioIndex = 0;




    /// <summary>
    /// 乌鸦
    /// </summary>
    public AudioClip CrowAudio;
    public AudioClip CoinAudio;
    public AudioClip CleanUpAudio;
    public AudioClip StropArriveAudio;
    public AudioClip GrassAudio;
    public AudioClip FillBalloonAudio;
    public AudioClip outFireClip;
    public AudioClip ChickShoutClip;
    public AudioClip DuckShoutClip;
    public AudioClip CatchClip;
    public AudioClip CatShoutClip;
    public AudioClip DogShoutClip;
    public AudioClip UpgradeFarmClip;
    public AudioClip SnowballHitClip;
    public AudioClip TimeUpClip;
    public AudioClip MouseClip;



    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;



        audioSource = gameObject.AddComponent<AudioSource>();
        SecondSource = gameObject.AddComponent<AudioSource>();
        MainSource = gameObject.AddComponent<AudioSource>();

        AudioList.Add(audioSource);
        AudioList.Add(SecondSource);
        AudioList.Add(MainSource);
        foreach (var source in AudioList)
        {
            source.loop = false;
        }

    }




    public void PlayClip(AudioClip clip)
    {
        if (GameSetting.AudioSwitch == true && AudioIndex < AudioList.Count)
        {
            AudioList[AudioIndex].clip = clip;
            AudioList[AudioIndex].Play();

            AudioIndex = AudioIndex == 0 ? 1 : 0;
        }
    }

    public void PlayLongClip(AudioClip clip)
    {
        if (GameSetting.AudioSwitch == true)
        {
            if (MainSource.clip != clip)
            {
                MainSource.clip = clip;
                MainSource.Play();

            }
        }
    }

    public void PauseAllClip()
    {
        //audioSource.Pause();
        foreach (var source in AudioList)
        {
            source.Pause();
        }
    }

    public void PauseClip(AudioClip clip)
    {
        foreach (var source in AudioList)
        {
            if (source.clip == clip)
            {
                source.Pause();
            }
        }
    }



    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        foreach (var source in AudioList)
        {
            source.Stop();
            source.clip = null;
        }
    }

}
