using System.Collections;
using _Scripts.Utilities;
using UnityEngine;

namespace __Scripts.Systems
{
    public enum AudioEnemy
    {
        Shoot,
        Hit,
        Die,
        Alert,
    }
    public enum AudioPlayer
    {
        Shoot,
        Melee,
        Hit,
        Die,
        Pickup,
        ShieldHit,
        Dash,
    }

    public enum AudioType
    {
        Main,
        Music,
        SFX,
    }

    public class AudioSystem : Singleton<AudioSystem>
    {
        private AudioSource _musicSourceEnviro;
        private Coroutine _musicEnviroTransition;
        private AudioSource _musicSourceCombat;
        private Coroutine _musicCombatTransition;
        private AudioSource _sfxSource;

        [SerializeField][Range(0, 1)] private float _mainVolume = 0.5f;
        [SerializeField][Range(0, 1)] private float _sfxVolume = 1;
        [SerializeField][Range(0, 1)] private float _musicVolume = 1;

        [SerializeField] private float _musicTransitionDuration = 0.5f;


        [Header("Music")]
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip[] _enviroMusic;
        [SerializeField] bool _playEnviroMusicOnStart = true;
        [SerializeField] private AudioClip[] _combatMusic;

        [Header("General SFX")]
        [SerializeField] private AudioClip _selectSound;
        [SerializeField] private AudioClip _confirmSound;

        private void OnEnable()
        {
            _musicSourceEnviro = gameObject.AddComponent<AudioSource>();
            _musicSourceEnviro.loop = true;

            _musicSourceCombat = gameObject.AddComponent<AudioSource>();
            _musicSourceCombat.loop = true;

            _sfxSource = gameObject.AddComponent<AudioSource>();

            ChangeVolume(AudioType.Music, _musicVolume);
            ChangeVolume(AudioType.SFX, _sfxVolume);
            ChangeVolume(AudioType.Main, _mainVolume);

            PlayEnviroMusic();
        }

        public void PlayMusic()
        {
            PlayEnviroMusic();
        }
        public void PlayEnviroMusic()
        {
            if (_musicSourceEnviro.isPlaying) return;
            _musicSourceEnviro.clip = _enviroMusic[0];
            _musicEnviroTransition = StartCoroutine(TransitionSound(_musicSourceEnviro, _musicVolume, _musicTransitionDuration));
        }
        public void PlayCombatMusic()
        {
            if (_musicSourceCombat.isPlaying) return;
            _musicSourceCombat.clip = _combatMusic[0];
            _musicSourceCombat.time = _musicSourceEnviro.time;
            _musicCombatTransition = StartCoroutine(TransitionSound(_musicSourceCombat, _musicVolume, _musicTransitionDuration));
        }
        public void StopEnviroMusic()
        {
            _musicEnviroTransition = StartCoroutine(TransitionSound(_musicSourceEnviro, 0, _musicTransitionDuration));
        }
        public void StopCombatMusic()
        {
            _musicCombatTransition = StartCoroutine(TransitionSound(_musicSourceCombat, 0, _musicTransitionDuration));
        }
        IEnumerator TransitionSound(AudioSource source, float targetVolume, float duration)
        {
            if (source.isPlaying == false) source.Play();

            while (Mathf.Abs(source.volume - targetVolume) > 0.1f)
            {
                source.volume = Mathf.MoveTowards(source.volume, targetVolume, Time.deltaTime / duration);
                yield return null;
            }

            source.volume = targetVolume;
            if (targetVolume == 0)
            {
                source.Stop();
            }
        }

        public void PlaySound(AudioClip[] clips, Vector3 pos, float vol = 1, bool randomPitch = false)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (randomPitch) RandomizeSound(clip);
            _sfxSource.transform.position = pos;
            PlaySound(clip, pos, vol);
        }
        public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1, bool randomPitch = false)
        {
            if (randomPitch) RandomizeSound(clip);
            else _sfxSource.pitch = 1;
            _sfxSource.transform.position = pos;
            PlaySound(clip, vol);
        }

        public void PlaySound(AudioClip[] clips, float vol = 1f, bool randomPitch = false)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (randomPitch) RandomizeSound(clip);
            else _sfxSource.pitch = 1;
            _sfxSource.PlayOneShot(clip, _sfxVolume * vol);
        }
        public void PlaySound(AudioClip clip, float vol = 1f, bool randomPitch = false)
        {
            if (randomPitch) RandomizeSound(clip);
            else _sfxSource.pitch = 1;
            _sfxSource.PlayOneShot(clip, _sfxVolume * vol);
        }

        public void PlayMenuSelectSound()
        {
            _sfxSource.pitch = 1;
            _sfxSource.PlayOneShot(_selectSound, _sfxVolume);
        }
        public void PlayMenuConfirmSound()
        {
            _sfxSource.pitch = 1;

            _sfxSource.PlayOneShot(_confirmSound, _sfxVolume);
        }

        public void ChangeVolume(AudioType type, float vol)
        {
            switch (type)
            {
                case AudioType.Main:
                    _mainVolume = vol;
                    _musicSourceEnviro.volume = vol * _musicVolume;
                    _musicSourceCombat.volume = vol * _musicVolume;
                    _sfxSource.volume = vol * _sfxVolume;
                    break;
                case AudioType.Music:
                    _musicVolume = vol;
                    _musicSourceEnviro.volume = vol * _mainVolume;
                    _musicSourceCombat.volume = vol * _mainVolume;
                    break;
                case AudioType.SFX:
                    _sfxVolume = vol;
                    _sfxSource.volume = vol * _mainVolume;
                    break;
            }
        }
        void RandomizeSound(AudioClip clip)
        {
            _sfxSource.pitch = Random.Range(0.9f, 1.1f);
        }
    }
}