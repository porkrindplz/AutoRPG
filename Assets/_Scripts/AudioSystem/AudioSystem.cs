using System.Collections;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using _Scripts.Utilities;
using Logger = _Scripts.Utilities.Logger;

namespace __Scripts.Systems
{

    public enum AudioType
    {
        Main,
        Music,
        SFX,
    }

    public class AudioSystem : Singleton<AudioSystem>
    {
        private AudioSource _musicSourceCombat;
        private Coroutine _musicCombatTransition;
        private AudioSource _musicSourceDire;
        private Coroutine _musicDireTransition;
        private AudioSource _sfxSource;

        [SerializeField][Range(0, 1)] private float _mainVolume = 0.5f;
        [SerializeField][Range(0, 1)] private float _sfxVolume = 1;
        [SerializeField][Range(0, 1)] private float _musicVolume = 1;

        [SerializeField] private float _musicTransitionDuration = 0.5f;


        private AudioSource _activeSource;
        [SerializeField] private AudioClip[] MenuTracks;
        private AudioSource[] _menuSources;
        private Coroutine _musicMenuTransition;
        [SerializeField] private AudioClip[] StoryTracks;
        private AudioSource[] _storySources;
        private Coroutine _musicStoryTransition;
        [SerializeField] private AudioClip[] SetupTracks;
        private AudioSource[] _setupSources;
        private Coroutine _musicSetupTransition;
        [SerializeField] private AudioClip[] CombatATracks;
        private AudioSource[] _combatASources;
        private Coroutine _musicCombatATransition;
        [SerializeField] private AudioClip[] CombatBTracks;
        private AudioSource[] _combatBSources;
        private Coroutine _musicCombatBTransition;
        [SerializeField] private AudioClip[] CombatCTracks;
        private AudioSource[] _combatCSources;
        private Coroutine _musicCombatCTransition;

        [Header("Ambience")]
        private AudioSource _activeAmbiSource;
        private AudioSource _peacefulSource;
        private AudioClip _peacefulAmbi;
        private AudioSource _windySource;
        private AudioClip _windyAmbi;
        private AudioSource _stormySource;
        private AudioClip _stormyAmbi;
        

        [Header("Music")]
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip[] _combatMusic;
        [SerializeField] bool _playCombatMusicOnStart = true;
        [SerializeField] private AudioClip[] _direMusic;

        [Header("General SFX")]
        [SerializeField] private AudioClip _selectSound;
        [SerializeField] private AudioClip _confirmSound;
        [SerializeField] private AudioClip _negativeSound;

        [Header("Game Action SFX")] 
        [SerializeField] private AudioClip[] _noneSounds;
        [SerializeField] private AudioClip[] _fireSounds;
        [SerializeField] private AudioClip[] _iceSounds;
        [SerializeField] private AudioClip[] _waterSounds;
        [SerializeField] private AudioClip[] _earthSounds;
        [SerializeField] private AudioClip[] _poisonSounds;
        [SerializeField] private AudioClip[] _electricSounds;
        [SerializeField] private AudioClip[] _swordSounds;
        [SerializeField] private AudioClip[] _shieldSounds;
        [SerializeField] private AudioClip[] _bowSounds;
        

        private void OnEnable()
        {
            GameManager.Instance.OnAction += PlayActionSound;
            
            GameManager.Instance.OnBeforeGameStateChanged += OnStateChanged;
            EnemyManager.Instance.OnEnemySpawned += OnEnemyChanged;

            _menuSources = new AudioSource[MenuTracks.Length];
            for (int i = 0; i < MenuTracks.Length; i++)
            {
                _menuSources[i] = gameObject.AddComponent<AudioSource>();
                _menuSources[i].clip = MenuTracks[i];
                _menuSources[i].loop = true;
            }
            _storySources = new AudioSource[StoryTracks.Length];
            for (int i = 0; i < StoryTracks.Length; i++)
            {
                _storySources[i] = gameObject.AddComponent<AudioSource>();
                _storySources[i].clip = StoryTracks[i];
                _storySources[i].loop = true;
            }
            _setupSources = new AudioSource[SetupTracks.Length];
            for (int i = 0; i < SetupTracks.Length; i++)
            {
                _setupSources[i] = gameObject.AddComponent<AudioSource>();
                _setupSources[i].clip = SetupTracks[i];
                _setupSources[i].loop = true;
            }
            _combatASources = new AudioSource[CombatATracks.Length];
            for (int i = 0; i < CombatATracks.Length; i++)
            {
                _combatASources[i] = gameObject.AddComponent<AudioSource>();
                _combatASources[i].clip = CombatATracks[i];
                _combatASources[i].loop = true;
            }
            _combatBSources = new AudioSource[CombatBTracks.Length];
            for (int i = 0; i < CombatBTracks.Length; i++)
            {
                _combatBSources[i] = gameObject.AddComponent<AudioSource>();
                _combatBSources[i].clip = CombatBTracks[i];
                _combatBSources[i].loop = true;
            }
            _combatCSources = new AudioSource[CombatCTracks.Length];
            for (int i = 0; i < CombatCTracks.Length; i++)
            {
                _combatCSources[i] = gameObject.AddComponent<AudioSource>();
                _combatCSources[i].clip = CombatCTracks[i];
                _combatCSources[i].loop = true;
            }

            _activeSource = _menuSources[0];
            
            _musicSourceCombat = gameObject.AddComponent<AudioSource>();
            _musicSourceCombat.loop = true;

            _musicSourceDire = gameObject.AddComponent<AudioSource>();
            _musicSourceDire.loop = true;

            _sfxSource = gameObject.AddComponent<AudioSource>();

            ChangeVolume(AudioType.Music, _musicVolume);
            ChangeVolume(AudioType.SFX, _sfxVolume);
            ChangeVolume(AudioType.Main, _mainVolume);

            //PlayCombatMusic();
        }
        void OnEnemyChanged(Enemy enemy)
        {
            if(GameManager.Instance.CurrentGameState!=EGameState.Playing) return;
            Logger.Log("Enemies left: " + EnemyManager.Instance.GetEnemiesLeft());
                if (EnemyManager.Instance.GetEnemiesLeft()<=1)
                {
                    foreach (var source in _combatCSources)
                    {
                        if (source.isPlaying) return;
                        source.volume = 0;
                        source.time = _activeSource.time;
                    }
                    _musicCombatCTransition =
                        StartCoroutine(TransitionSounds(_combatCSources, _musicVolume, _musicTransitionDuration));
                    if (_menuSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_menuSources, _musicTransitionDuration));
                    if (_storySources[0].isPlaying)
                        StartCoroutine(TransitionOut(_storySources, _musicTransitionDuration));
                    if (_setupSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_setupSources, _musicTransitionDuration));
                    if(_combatASources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatASources, _musicTransitionDuration));
                    if(_combatBSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatBSources, _musicTransitionDuration));
                    _activeSource = _combatCSources[0];
                    return;
                }

                else if (EnemyManager.Instance.GetEnemiesLeft()<2)
                {
                    foreach (var source in _combatBSources)
                    {
                        if (source.isPlaying) return;

                        source.time = _activeSource.time;
                        source.volume = 0;

                    }
                    _musicCombatBTransition =
                        StartCoroutine(TransitionSounds(_combatBSources, _musicVolume, _musicTransitionDuration));
                    if (_menuSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_menuSources, _musicTransitionDuration));
                    if (_storySources[0].isPlaying)
                        StartCoroutine(TransitionOut(_storySources, _musicTransitionDuration));
                    if (_setupSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_setupSources, _musicTransitionDuration));
                    if(_combatASources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatASources, _musicTransitionDuration)); 
                    if(_combatCSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatCSources, _musicTransitionDuration));
                }
                
                else if(EnemyManager.Instance.GetEnemiesLeft()<3)
                {
                    foreach (var source in _combatASources)
                    {
                        if (source.isPlaying) return;

                        source.time = _activeSource.time;
                        source.volume = 0;

                    }

                    _musicCombatATransition =
                        StartCoroutine(TransitionSounds(_combatASources, _musicVolume, _musicTransitionDuration));
                    if (_menuSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_menuSources, _musicTransitionDuration));
                    if (_storySources[0].isPlaying)
                        StartCoroutine(TransitionOut(_storySources, _musicTransitionDuration));
                    if (_setupSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_setupSources, _musicTransitionDuration));
                    if(_combatBSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatBSources, _musicTransitionDuration));
                    if(_combatCSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatCSources, _musicTransitionDuration));
                    _activeSource = _combatASources[0];
                    return;
                }
            
        }
        void OnStateChanged(EGameState prevState, EGameState state)
        {
            if (state == EGameState.MainMenu)
            {
                foreach (var source in _menuSources)
                {
                    if (source.isPlaying) return;

                        source.time = _activeSource.time;
                        source.volume = 0;

                }
                _musicMenuTransition = StartCoroutine(TransitionSounds(_menuSources, _musicVolume, _musicTransitionDuration));
                if(_storySources[0].isPlaying) StartCoroutine(TransitionOut(_storySources, _musicTransitionDuration));
                if(_combatASources[0].isPlaying) StartCoroutine(TransitionOut(_combatASources, _musicTransitionDuration));
                if(_setupSources[0].isPlaying) StartCoroutine(TransitionOut(_setupSources, _musicTransitionDuration));
                if(_combatBSources[0].isPlaying) StartCoroutine(TransitionOut(_combatBSources, _musicTransitionDuration));
                if(_combatCSources[0].isPlaying) StartCoroutine(TransitionOut(_combatCSources, _musicTransitionDuration));
                _activeSource = _menuSources[0];
                
            }
            if(state==EGameState.Story)
            {
                foreach (var source in _storySources)
                {
                    if (source.isPlaying) return;

                        source.time = _activeSource.time;
                        source.volume = 0;

                }
                _musicStoryTransition = StartCoroutine(TransitionSounds(_storySources, _musicVolume, _musicTransitionDuration));
                if(_menuSources[0].isPlaying) StartCoroutine(TransitionOut(_menuSources, _musicTransitionDuration));
                if(_combatASources[0].isPlaying) StartCoroutine(TransitionOut(_combatASources, _musicTransitionDuration));
                if(_setupSources[0].isPlaying) StartCoroutine(TransitionOut(_setupSources, _musicTransitionDuration));
                if(_combatASources[0].isPlaying) StartCoroutine(TransitionOut(_combatASources, _musicTransitionDuration));
                if(_combatBSources[0].isPlaying) StartCoroutine(TransitionOut(_combatBSources, _musicTransitionDuration));
                if(_combatCSources[0].isPlaying) StartCoroutine(TransitionOut(_combatCSources, _musicTransitionDuration));
                _activeSource = _storySources[0];
            }

            if (state == EGameState.SetupGame)
            {
                foreach (var source in _setupSources)
                {
                    if (source.isPlaying) return;

                        source.time = _activeSource.time;
                        source.volume = 0;

                }
                _musicSetupTransition = StartCoroutine(TransitionSounds(_setupSources, _musicVolume, _musicTransitionDuration));
                if(_menuSources[0].isPlaying) StartCoroutine(TransitionOut(_menuSources, _musicTransitionDuration));
                if(_storySources[0].isPlaying) StartCoroutine(TransitionOut(_storySources, _musicTransitionDuration));
                if(_combatASources[0].isPlaying) StartCoroutine(TransitionOut(_combatASources, _musicTransitionDuration));
                if(_combatBSources[0].isPlaying) StartCoroutine(TransitionOut(_combatBSources, _musicTransitionDuration));
                if(_combatCSources[0].isPlaying) StartCoroutine(TransitionOut(_combatCSources, _musicTransitionDuration));
                _activeSource = _setupSources[0];
            }
            

            if(state==EGameState.Playing)
            {
                if (EnemyManager.Instance.GetCurrentGroup().CurrentEnemy ==
                    EnemyManager.Instance.GetCurrentGroup().GetCurrentSetCount())
                {
                    foreach (var source in _combatCSources)
                    {
                        if (source.isPlaying) return;

                        source.time = _activeSource.time;
                        source.volume = 0;

                    }
                    _musicCombatCTransition =
                        StartCoroutine(TransitionSounds(_combatCSources, _musicVolume, _musicTransitionDuration));
                    if (_menuSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_menuSources, _musicTransitionDuration));
                    if (_storySources[0].isPlaying)
                        StartCoroutine(TransitionOut(_storySources, _musicTransitionDuration));
                    if (_setupSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_setupSources, _musicTransitionDuration));
                    if(_combatASources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatASources, _musicTransitionDuration));
                    if(_combatBSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatBSources, _musicTransitionDuration));
                    _activeSource = _combatCSources[0];
                    return;
                }

                else if (EnemyManager.Instance.GetCurrentGroup().CurrentEnemy > 2)
                {
                    foreach (var source in _combatBSources)
                    {
                        if (source.isPlaying) return;

                        source.time = _activeSource.time;
                        source.volume = 0;

                    }
                    _musicCombatBTransition =
                        StartCoroutine(TransitionSounds(_combatBSources, _musicVolume, _musicTransitionDuration));
                    if (_menuSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_menuSources, _musicTransitionDuration));
                    if (_storySources[0].isPlaying)
                        StartCoroutine(TransitionOut(_storySources, _musicTransitionDuration));
                    if (_setupSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_setupSources, _musicTransitionDuration));
                    if(_combatASources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatASources, _musicTransitionDuration)); 
                    if(_combatCSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatCSources, _musicTransitionDuration));
                }
                
                else if(EnemyManager.Instance.GetCurrentGroup().CurrentEnemy ==2)
                {
                    foreach (var source in _combatASources)
                    {
                        if (source.isPlaying) return;

                        source.time = _activeSource.time;
                        source.volume = 0;

                    }

                    _musicCombatATransition =
                        StartCoroutine(TransitionSounds(_combatASources, _musicVolume, _musicTransitionDuration));
                    if (_menuSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_menuSources, _musicTransitionDuration));
                    if (_storySources[0].isPlaying)
                        StartCoroutine(TransitionOut(_storySources, _musicTransitionDuration));
                    if (_setupSources[0].isPlaying)
                        StartCoroutine(TransitionOut(_setupSources, _musicTransitionDuration));
                    if(_combatBSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatBSources, _musicTransitionDuration));
                    if(_combatCSources[0].isPlaying) 
                        StartCoroutine(TransitionOut(_combatCSources, _musicTransitionDuration));
                    _activeSource = _combatASources[0];
                    return;
                }
            }
        }
        
        
        public void PlayCombatMusic()
        {
            if (_musicSourceCombat.isPlaying) return;
            _musicSourceCombat.clip = _combatMusic[0];
            _musicCombatTransition = StartCoroutine(TransitionSound(_musicSourceCombat, _musicVolume, _musicTransitionDuration));
        }
        public void StopCombatMusic()
        {
            _musicCombatTransition = StartCoroutine(TransitionSound(_musicSourceCombat, 0, _musicTransitionDuration));
        }
        public void PlayDireMusic()
        {
            if (_musicSourceDire.isPlaying) return;
            _musicSourceDire.clip = _direMusic[0];
            _musicSourceDire.time = _musicSourceCombat.time;
            _musicDireTransition = StartCoroutine(TransitionSound(_musicSourceDire, _musicVolume, _musicTransitionDuration));
        }
        public void StopDireMusic()
        {
            _musicDireTransition = StartCoroutine(TransitionSound(_musicSourceDire, 0, _musicTransitionDuration));
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

        IEnumerator TransitionSounds(AudioSource[] sources, float targetVolume, float duration)
        {
            foreach (var source in sources)
            {
                if (source.isPlaying == false) source.Play();
            }

            while (Mathf.Abs(sources[0].volume - targetVolume) > 0.1f)
            {
                foreach (var source in sources)
                {
                    source.volume = Mathf.MoveTowards(source.volume, targetVolume, Time.deltaTime / duration);
                }
                yield return null;
            }

            foreach (var source in sources)
            {
                source.volume = targetVolume;
                if (targetVolume == 0)
                {
                    source.Stop();
                }
            }
        }

        IEnumerator TransitionOut(AudioSource[] sources, float time)
        {
            
            while (sources[0].volume > 0)
            {
                foreach (var source in sources)
                {
                    source.volume = Mathf.MoveTowards(source.volume, 0, Time.deltaTime / time);
                }
                yield return null;
            }

            foreach (var source in sources)
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
            if (clips == null || clips.Length == 0) return;
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
        public void PlayNegativeSound()
        {
            _sfxSource.pitch = 1;

            _sfxSource.PlayOneShot(_negativeSound, _sfxVolume);
        }

        public void ChangeVolume(AudioType type, float vol)
        {
            switch (type)
            {
                case AudioType.Main:
                    _mainVolume = vol;
                    _musicSourceCombat.volume = vol * _musicVolume;
                    _musicSourceDire.volume = vol * _musicVolume;
                    _sfxSource.volume = vol * _sfxVolume;
                    break;
                case AudioType.Music:
                    _musicVolume = vol;
                    _musicSourceCombat.volume = vol * _mainVolume;
                    _musicSourceDire.volume = vol * _mainVolume;
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
        
        void PlayActionSound(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
        {
            // if (action is AttackGameAction attack)
            // {
            //     PlayElementalSound(attack.Element);
            // }
            // else if (action is WeaponGameAction weapon)
            // {
            //     PlayWeaponSound(weapon.Weapon);
            // }
        }

        public void PlayElementalSound(ElementsType type)
        {
            switch (type)
            {
                case ElementsType.None:
                    PlaySound(_noneSounds,1,false);
                    break;
                case ElementsType.Fire:
                    PlaySound(_fireSounds,1,false);
                    break;
            }
        }
        public void PlayWeaponSound(WeaponsType type)
        {
            switch (type)
            {
                case WeaponsType.Sword:
                    PlaySound(_swordSounds,1,false);
                    break;
                case WeaponsType.Shield:
                    PlaySound(_shieldSounds,1,false);
                    break;
                case WeaponsType.Bow:
                    PlaySound(_bowSounds,1,false);
                    break;
            }
        }
    }
}