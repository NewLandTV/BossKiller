using System;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private Sound[] bgmSounds;
    [SerializeField]
    private Sound[] sfxSounds;

    [SerializeField]
    private AudioSource bgmPlayer;
    [SerializeField]
    private AudioSource[] sfxPlayers;

    [Serializable]
    private struct Sound
    {
        public string name;
        public AudioClip clip;
    }

    private void Awake() => Setup(this);
    #region BGM
    public void PlayBGM(string bgmName, bool loop)
    {
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (!bgmSounds[i].name.Equals(bgmName))
            {
                continue;
            }

            bgmPlayer.clip = bgmSounds[i].clip;
            bgmPlayer.loop = loop;

            bgmPlayer.Play();

            return;
        }
    }

    public void PauseBGM() => bgmPlayer.Pause();

    public void StopBGM() => bgmPlayer.Stop();
    #endregion
    #region SFX
    public void PlaySFX(string sfxName, bool loop)
    {
        for (int i = 0; i < sfxSounds.Length; i++)
        {
            if (!sfxSounds[i].name.Equals(sfxName))
            {
                continue;
            }

            for (int j = 0; j < sfxPlayers.Length; j++)
            {
                if (sfxPlayers[j].isPlaying)
                {
                    continue;
                }

                sfxPlayers[j].clip = sfxSounds[i].clip;
                sfxPlayers[j].loop = loop;

                sfxPlayers[j].Play();

                return;
            }

            return;
        }
    }

    public void PauseAllSFX()
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i].Pause();
        }
    }

    public void StopAllSFX()
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i].Stop();
        }
    }
    #endregion
}
