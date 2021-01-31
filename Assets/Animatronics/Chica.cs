using System;
using System.Collections;
using UnityEngine;
using RNG = UnityEngine.Random;

public partial class qkUCNScript
{
    private const int MinimumChicaPlay = 5;
    private bool ChicaAttack = false;

    private IEnumerator HandleChica()
    {
        Func<WaitForSeconds> PlayRandom = () =>
        {
            int Chosen = RNG.Range(1, 5);
            Audio.PlaySoundAtTransform(String.Format("Chica{0}", Chosen), transform);
            return new WaitForSeconds(Chosen == 1 || Chosen == 4 ? 5 : Chosen == 2 ? 10 : 7);
        };
        for(int i = 0;i<MinimumChicaPlay;i++)
        {
            yield return PlayRandom();
            yield return null;
        }
        while (!solved)
        {
            yield return null;
            if(!globalMusicBox && RNG.Range(1, 11)==1)
            {
                ChicaAttack = true;
                StopCoroutine(WaitForChicaPress());
                StartCoroutine(WaitForChicaPress());
                yield break;
            }
            yield return PlayRandom();
        }
    }

    private IEnumerator WaitForChicaPress()
    {
        yield return new WaitForSeconds(!TwitchPlaysActive ? 20f : 30f);
        if(ChicaAttack && !solved)
        {
            Strike("Chica");
            StopCoroutine(HandleChica());
            StartCoroutine(HandleChica());
        }
    }
}
