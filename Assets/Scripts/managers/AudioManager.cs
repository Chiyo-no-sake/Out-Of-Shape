using System.Collections;
using UnityEngine;
public class AudioManager : IManager {

    private AudioClip _player_shoot;
    private AudioClip _player_enemy_die;
    private AudioClip _enemy_hurt;
    private AudioClip _background_theme;
    private AudioSource _source;
    private GameObject _sourceGO;

    private static AudioManager instance;

    public static AudioManager GetInstance()
    {
        if (instance == null) instance = new AudioManager();
        return instance;
    }

    public enum AudioType
    {
        PLAYER_SHOOT,
        ENEMY_HURT,
        PLAYER_ENEMY_DIE,
        BACKGROUND_THEME
    }

    public void Init(){

        _sourceGO = GameObject.Find("AudioSource");
        _source = _sourceGO.GetComponent<AudioSource>();
        _player_shoot = Resources.Load<AudioClip>("sounds/player_laser_shoot");
        _enemy_hurt = Resources.Load<AudioClip>("sounds/enemy_hurt");
        _background_theme = Resources.Load<AudioClip>("sounds/background_theme_play");
        _player_enemy_die = Resources.Load<AudioClip>("sounds/enemy_player_death");
        _source.loop = true;
    }

    public bool IsReady(){

        return (_player_shoot != null && _enemy_hurt != null && _background_theme != null);

    }

    public void OnSetupComplete(){
        Play(AudioType.BACKGROUND_THEME);
    }

    public void Play(AudioType clip){

        switch (clip)
        {
            
            case AudioType.PLAYER_SHOOT:
                _source.PlayOneShot(_player_shoot);
                break;

            case AudioType.ENEMY_HURT:
                _source.PlayOneShot(_enemy_hurt);
                break;
            case AudioType.PLAYER_ENEMY_DIE:
                _source.PlayOneShot(_player_enemy_die);
                break;

            case AudioType.BACKGROUND_THEME:
                _source.clip = _background_theme;
                _source.Play();
                break;

            default:
                break;
        }

    }

    public void Stop(AudioType clip){

        switch (clip)
        {
            
            case AudioType.PLAYER_SHOOT:
            case AudioType.ENEMY_HURT:
            case AudioType.BACKGROUND_THEME:
                _source.Stop();
            break;
        }

    }

    void Pause(AudioType clip){

        switch (clip)
        {
            
            case AudioType.PLAYER_SHOOT:
            case AudioType.ENEMY_HURT:

            case AudioType.BACKGROUND_THEME:
                _source.Pause();
                break;
        }

    }

    void Resume(AudioType clip){

        switch (clip)
        {
            
            case AudioType.PLAYER_SHOOT:
            case AudioType.ENEMY_HURT:

            case AudioType.BACKGROUND_THEME:
                _source.UnPause();
                break;
        }

    }

    public AudioSource GetSource()
    {
        return _source;
    }

    public GameObject GetSourceGO()
    {
        return _sourceGO;
    }
}