using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class SoundManager : MonoBehaviour
{
    public enum AudioType
    { 
        background,
        battleground,
        Jhin,
        Kim,
        Lee,
        Park,
        leeHit,
        leeTimeChange,
        kimCostHit,
        jhinStrongHit,
        parkHPHit,
        parkCostHit,
        poisonHit,
        heal,
        hit,
        win,
        lose,
        enemyDead,
        enemyHPZero,
    }

    public static Action StartBackgroundAudio;
    public static Action StopBackgroundAudio;
    public static Action OnButtonDown;
    public static Action OnButtonUp;
    public static Action<AudioType, bool> PlayEffectAudio;
    public static Action<AudioType, bool> PlayHitAudio;
    public static Action<bool> OnFadeSound;

    private static SoundManager instance;

    [SerializeField] private Transform player;

    [SerializeField] private float soundChangeTime;

    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private Transform effectPlayers;
    [SerializeField] private Transform hitPlayers;
    [SerializeField] private AudioSource buttonDownSource;
    [SerializeField] private AudioSource buttonUpSource;

    [Space(10.0f), Header("Sounds")]
    [SerializeField] private List<AudioClip> introSounds;
    [SerializeField] private List<AudioClip> backgroundSounds;
    [SerializeField] private List<AudioClip> battlegroundSounds;
    [SerializeField] private Transform sources;
    //[SerializeField] private List<AudioClip> effectSounds;
    //[SerializeField] private List<AudioClip> hitSounds;

    [Space(10.0f), Header("Boss Audios")]
    [SerializeField] private AudioClip leeAudio;
    [SerializeField] private AudioClip jhinAudio;
    [SerializeField] private AudioClip kimAudio;
    [SerializeField] private AudioClip parkAudio;

    private bool changeBackgroundAudio;

    private List<AudioSource> sounds;
    //private List<AudioSource> effectAudioSources;
    //private List<AudioSource> hitAudioSources;

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static SoundManager Instance()
    {
        if (instance == null)
        {
            Debug.Log("SoundManager Instance is NULL");
        }

        return instance;
    }

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        SceneManagerEX.OnSwitchSceneToMap -= ChangeToMapBackgroundAudio;
        SceneManagerEX.OnSwitchSceneToMap += ChangeToMapBackgroundAudio;

        BattleManager.OnEnemyHPisZero -= PlayEnemyHPisZeroAudio;
        BattleManager.OnEnemyHPisZero += PlayEnemyHPisZeroAudio;

        BattleManager.OnBattleWin -= PlayBattleWinAudio;
        BattleManager.OnBattleWin += PlayBattleWinAudio;

        BattleManager.OnBattleLose -= PlayBattleLoseAudio;
        BattleManager.OnBattleLose += PlayBattleLoseAudio;

        OnButtonDown -= ButtonDown;
        OnButtonDown += ButtonDown;

        OnButtonUp -= ButtonUp;
        OnButtonUp += ButtonUp;

        PlayEffectAudio -= PlayOtherAudio;
        PlayEffectAudio += PlayOtherAudio;

        PlayHitAudio -= PlayOtherAudio;
        PlayHitAudio += PlayOtherAudio;

        OnFadeSound -= Fade;
        OnFadeSound += Fade;

        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks));

        soundChangeTime = 1 / soundChangeTime;
        changeBackgroundAudio = true;

        sounds = new List<AudioSource>();
        //effectAudioSources = new List<AudioSource>();
        //hitAudioSources = new List<AudioSource>();

        //StartCoroutine(ChangeAudioRoutine(backgroundSounds[1], true, 5.0f));

        //PlayOtherAudio(AudioType.parkCostHit);
    }

    private void Update()
    {
        FollowPlayer();
    }

    private void OnDestroy()
    {
        SceneManagerEX.OnSwitchSceneToMap -= ChangeToMapBackgroundAudio;
        BattleManager.OnBattleWin -= PlayBattleWinAudio;
        BattleManager.OnBattleLose -= PlayBattleLoseAudio;
        BattleManager.OnEnemyHPisZero -= PlayEnemyHPisZeroAudio;
        OnButtonDown -= ButtonDown;
        OnButtonUp -= ButtonUp;
        PlayEffectAudio -= PlayOtherAudio;
        PlayHitAudio -= PlayOtherAudio;
        OnFadeSound -= Fade;
    }

    private void PlayEnemyHPisZeroAudio()
    {
        PlayOtherAudio(AudioType.enemyHPZero, false);
    }

    private void PlayBattleWinAudio()
    {
        PlayOtherAudio(AudioType.win, false);
    }

    private void PlayBattleLoseAudio()
    {
        PlayOtherAudio(AudioType.lose, false);
    }

    private void FollowPlayer()
    {
        transform.position = player.position;
    }

    private void ButtonDown()
    {
        buttonDownSource.Play();
    }

    private void ButtonUp()
    {
        buttonUpSource.Play();
    }

    private void stopBackground()
    {
        if (backgroundSource.isPlaying)
            backgroundSource.Pause();
    }

    private void startBackground()
    {
        if (!backgroundSource.isPlaying)
            backgroundSource.Play();
    }

    public void SetChangeBackgroundAudio(bool change)
    {
        changeBackgroundAudio = change;
    }

    public void ChangeToBattleBackgroundAudio(AudioType type = AudioType.battleground)
    {
        if (type == AudioType.Kim)
        {
            StartCoroutine(ChangeAudioRoutine(kimAudio, true));
        }
        else if (type == AudioType.Lee)
        {
            StartCoroutine(ChangeAudioRoutine(leeAudio, true));
        }
        else if (type == AudioType.Park)
        {
            StartCoroutine(ChangeAudioRoutine(parkAudio, true));
        }
        else if (type == AudioType.Jhin)
        {
            StartCoroutine(ChangeAudioRoutine(jhinAudio, true));
        }
        else
        {
            int rand = (int)UnityEngine.Random.Range(0.0f, battlegroundSounds.Count);
            StartCoroutine(ChangeAudioRoutine(battlegroundSounds[rand], true));
        }
    }

    public void ChangeToMapBackgroundAudio()
    {
        if (!changeBackgroundAudio)
            return;

        int rand = (int)UnityEngine.Random.Range(0.0f, backgroundSounds.Count);
        StartCoroutine(ChangeAudioRoutine(backgroundSounds[rand], true));
    }

    private void PlayOtherAudio(AudioType type, bool loop = false)
    {
        AudioClip audio = GetAudio(type);

        int sourcesCount = sounds.Count;
        int i = 0;
        for (; i < sourcesCount; i++)
        {
            AudioSource currentSource = sounds[i];
            if (!currentSource.isPlaying)
            {
                currentSource.clip = audio;
                currentSource.loop = loop;
                currentSource.Play();

                break;
            }
        }

        if (i >= sourcesCount)
        {
            GameObject newSource = new GameObject();
            newSource = Instantiate(newSource);

            newSource.transform.SetParent(sources);
            newSource.AddComponent<AudioSource>();
            newSource.name = String.Format("source_{0}", i);

            AudioSource newAudioSoure = newSource.GetComponent<AudioSource>();

            sounds.Add(newAudioSoure);

            newAudioSoure.clip = audio;
            newAudioSoure.loop = loop;
            newAudioSoure.Play();
        }
    }

    public void Fade(bool isIn)
    {
        StopCoroutine("FadeSound");
        StartCoroutine(FadeSound(isIn));
    }

    AudioClip GetAudio(AudioType type)
    {
        AudioClip ret = null;
        string typeStr = type.ToString();
        string path = "";

        if (type == AudioType.hit)
        {
            int rand = UnityEngine.Random.Range(0, 2);
            path = String.Format("Sounds/Others/{0}_{1}", typeStr, rand);
        }
        else
        {
            path = String.Format("Sounds/Others/{0}", typeStr);
        }

        //Debug.Log(path);
        ret = Resources.Load<AudioClip>(path);
        ret = Instantiate(ret);

        return ret;
    }

    IEnumerator StopCurrentBackgroundAudio()
    {
        while (true)
        {
            yield return null;

            float volume = backgroundSource.volume;
            if (volume - Time.deltaTime * soundChangeTime < 0)
            {
                break;
            }
            else
            {
                backgroundSource.volume = volume - Time.deltaTime * soundChangeTime;
            }
        }
    }

    IEnumerator ChangeAudioRoutine(AudioClip audio, bool isBackground = false, float delayTime = 0.0f)
    {
        yield return new WaitForSecondsRealtime(delayTime);

        yield return FadeSound(true);

        backgroundSource.clip = audio;
        backgroundSource.volume = 1.0f;
        backgroundSource.Play();
        backgroundSource.loop = isBackground;
    }

    public IEnumerator FadeSound(bool isIn)
    {
        float target = isIn ? 0.0f : 1.0f;
        while(Mathf.Abs(backgroundSource.volume  - target) > 0.01)
        {
            yield return null;

            backgroundSource.volume = isIn ? backgroundSource.volume - Time.deltaTime * soundChangeTime :
                                                 backgroundSource.volume + Time.deltaTime * soundChangeTime;
        }

        backgroundSource.volume = target;
    }
}
