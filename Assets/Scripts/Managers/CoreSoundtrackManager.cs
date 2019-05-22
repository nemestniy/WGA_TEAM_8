using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSoundtrackManager : MonoBehaviour
{

    public bool muteGroup;

    public AudioSource[] mainClips;

    public AudioSource[] zoneClips;

    public AudioSource[] energyClips;

    public AudioSource[] drumClips;

    public GameManager gm;


    // Вызывай метод, чтобы отключить кор саундтрек во время катсцен или еще когда надо
    public void AllMute()
    {
        foreach (AudioSource ass in mainClips)
        {
            //OffSource(ass);
            ass.mute = true;
        }
        foreach (AudioSource ass in zoneClips)
        {
            //OffSource(ass);
            ass.mute = true;
        }
        foreach (AudioSource ass in energyClips)
        {
            //OffSource(ass);
            ass.mute = true;
        }
        foreach (AudioSource ass in drumClips)
        {
            //OffSource(ass);
            ass.mute = true;
        }
        muteGroup = true;
    }

    // Вызывай метод, чтобы снова включить кор саундтрек
    public void AllStart()
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

    // Start is called before the first frame update
    void Start()
    {
        
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
                AllMute();
            }
            else if (test == true)
            {
                AllStart();
            }
            
        } 

        if (!muteGroup)
        {
            timer++;

            if (timer < 10)
            {
                OnSource(mainClips[0]);
                OnSource(mainClips[1]);

                OffSource(mainClips[2]);
                OffSource(mainClips[3]);
            }

            if (timer > 800)
            {
                OnSource(mainClips[2]);
            }

            if (timer > 1600)
            {
                OnSource(mainClips[3]);
            }

            if (timer > 2400)
            {
                OnSource(mainClips[4]);
            }


            if (gm.LampEnergyLvl > 0.80)
            {
                OffSource(energyClips[0]);
                OffSource(energyClips[1]);
                OffSource(energyClips[2]);
                OffSource(energyClips[3]);
            }

            if (gm.LampEnergyLvl < 0.80)
            {
                OnSource(energyClips[0]);
            }

            if (gm.LampEnergyLvl < 0.60)
            {
                OnSource(energyClips[1]);
            }
            if (gm.LampEnergyLvl > 0.60)
            {
                OffSource(energyClips[1]);
            }

            if (gm.LampEnergyLvl < 0.40)
            {
                OnSource(energyClips[2]);
            }
            if (gm.LampEnergyLvl > 0.40)
            {
                OffSource(energyClips[2]);
            }

            if (gm.LampEnergyLvl < 0.20)
            {
                OnSource(energyClips[3]);
            }
            if (gm.LampEnergyLvl > 0.20)
            {
                OffSource(energyClips[3]);
            }

            if (gm.DistanceToClosestEnemy > 35)
            {
                OffSource(drumClips[0]);
                OffSource(drumClips[1]);
                OffSource(drumClips[2]);
            }

            if (gm.DistanceToClosestEnemy < 35)
            {
                OnSource(drumClips[0]);
            }
            if (gm.DistanceToClosestEnemy > 25)
            {
                OffSource(drumClips[1]);
            }
            if (gm.DistanceToClosestEnemy < 25)
            {
                OnSource(drumClips[1]);
            }
            if (gm.DistanceToClosestEnemy > 15)
            {
                OffSource(drumClips[2]);
            }
            if (gm.DistanceToClosestEnemy < 15)
            {
                OnSource(drumClips[2]);
            }


            if (gm.DistanceToWell > 20)
            {
                OffSource(mainClips[4]);
            }
            if (gm.DistanceToWell < 20)
            {
                OnSource(mainClips[4]);
            }
        }  
    }
}
