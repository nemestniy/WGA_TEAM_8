using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSoundtrack : MonoBehaviour
{
    private bool muteGroup;

    [SerializeField]
    private AudioSource[] mainClips;

    [SerializeField]
    private AudioSource[] zoneClips;

    [SerializeField]
    private AudioSource[] energyClips;

    [SerializeField]
    private AudioSource[] drumClips;

    private GameManager _gm;
    /*
    #region Singletone
    public static CoreSoundtrack Instance { get; private set; }
    public CoreSoundtrack() : base()
    {
        Instance = this;
    }
    #endregion
    */
    void Start()
    {
        _gm = GameManager.Instance;
    }

    public void PlayAll()
    {
        if (mainClips != null)
        {
            foreach (AudioSource ass in mainClips)
            {
                //OffSource(ass);
                ass.Play();
            }
        }

        if (zoneClips != null)
        {
            foreach (AudioSource ass in zoneClips)
            {
                //OffSource(ass);
                ass.Play();
            }
        }

        if (energyClips != null)
        {
            foreach (AudioSource ass in energyClips)
            {
                //OffSource(ass);
                ass.Play();
            }
        }

        if (drumClips != null)
        {
            foreach (AudioSource ass in drumClips)
            {
                //OffSource(ass);
                ass.Play();
            }
        }
    }

    // Вызывай метод, чтобы отключить кор саундтрек во время катсцен или еще когда надо
    public void Mute()
    {
        if (mainClips != null)
        {
            foreach (AudioSource ass in mainClips)
            {
                ass.mute = true;
            }
        }

        if (zoneClips != null)
        {
            foreach (AudioSource ass in zoneClips)
            {
                ass.mute = true;
            }
        }

        if (energyClips != null)
        {
            foreach (AudioSource ass in energyClips)
            {
                ass.mute = true;
            }
        }

        if (drumClips != null)
        {
            foreach (AudioSource ass in drumClips)
            {
                ass.mute = true;
            }
        }
        
        muteGroup = true;
    }

    // Вызывай метод, чтобы снова включить кор саундтрек
    public void Resume()
    {
        muteGroup = false;
    }



    public void OnSource(AudioSource ass)
    {
        if(ass.mute == true) StartCoroutine(FadeUp(ass));
    }

    public void OffSource(AudioSource ass)
    {
        if (ass.mute == false) StartCoroutine(FadeDown(ass));
    }


    IEnumerator FadeUp(AudioSource ass)
    {
        ass.volume = 0;
        ass.mute = false;

        while (ass.volume < 100)
        {
            ass.volume += 0.0015f;
            yield return null;
        }
    }

    IEnumerator FadeDown(AudioSource ass)
    {
        while (ass.volume > 0.01f)
        {
            ass.volume -= 0.0015f;
            yield return null;
        }
        if (ass.volume < 0.01f)
        {
            ass.mute = true;
        }
    }


    int timer = 0;
    bool test = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            test = !test;

            if (test == false)
            {
                Mute();
            }
            else if (test == true)
            {
                Resume();
            }
            
        } 

        if (!muteGroup)
        {
            OnSource(mainClips[0]);
            OnSource(mainClips[1]);

            timer++;

            if (timer < 10)
            {
                

                OffSource(mainClips[2]);
                OffSource(mainClips[3]);
            }

            if (timer > 1500)
            {
                OnSource(mainClips[2]);
            }

            if (timer > 3000)
            {
                OnSource(mainClips[3]);
            }

            if (timer > 6000)
            {
                OnSource(mainClips[4]);
            }

            if (timer > 12000)
            {
                OnSource(mainClips[4]);
            }


            if (_gm.LampEnergyLvl > 0.80)
            {
                OffSource(energyClips[0]);
                OffSource(energyClips[1]);
                OffSource(energyClips[2]);
                OffSource(energyClips[3]);
            }

            if (_gm.LampEnergyLvl < 0.80)
            {
                OnSource(energyClips[0]);
            }

            if (_gm.LampEnergyLvl < 0.60)
            {
                OnSource(energyClips[1]);
            }
            if (_gm.LampEnergyLvl > 0.60)
            {
                OffSource(energyClips[1]);
            }

            if (_gm.LampEnergyLvl < 0.40)
            {
                OnSource(energyClips[2]);
            }
            if (_gm.LampEnergyLvl > 0.40)
            {
                OffSource(energyClips[2]);
            }

            if (_gm.LampEnergyLvl < 0.20)
            {
                OnSource(energyClips[3]);
            }
            if (_gm.LampEnergyLvl > 0.20)
            {
                OffSource(energyClips[3]);
            }

            if (_gm.DistanceToClosestEnemy > 45)
            {
                OffSource(drumClips[0]);
                OffSource(drumClips[1]);
                OffSource(drumClips[2]);
            }

            if (_gm.DistanceToClosestEnemy < 45)
            {
                OnSource(drumClips[0]);
            }
            if (_gm.DistanceToClosestEnemy > 30)
            {
                OffSource(drumClips[1]);
            }
            if (_gm.DistanceToClosestEnemy < 30)
            {
                OnSource(drumClips[1]);
            }
            if (_gm.DistanceToClosestEnemy > 15)
            {
                OffSource(drumClips[2]);
            }
            if (_gm.DistanceToClosestEnemy < 15)
            {
                OnSource(drumClips[2]);
            }

            /*
            if (_gm.DistanceToWell > 20)
            {
                OffSource(mainClips[4]);
            }
            if (_gm.DistanceToWell < 20)
            {
                OnSource(mainClips[4]);
            }
            */
        }  
    }
}
