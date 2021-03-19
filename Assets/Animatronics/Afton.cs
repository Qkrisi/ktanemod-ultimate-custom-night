using System;
using System.Reflection;
using System.Collections;
using UnityEngine;
using RNG = UnityEngine.Random;

public partial class qkUCNScript
{ 
    private IEnumerator HandleAfton()
    {
        Type FactoryType = ReflectionHelper.FindType("FactoryAssembly.FactoryRoom", "FactoryAssembly");
        if (FactoryType != null && FindObjectOfType(FactoryType) != null) yield break;
        yield return new WaitForSeconds(!TwitchPlaysActive ? 10f : 15f);
        while(true)
        {
            if (solved) yield break;
            yield return new WaitForSeconds(!TwitchPlaysActive ? 5f : 10f);
            yield return null;
            if(RNG.Range(1, 5)==1)
            {
     
                Type LightType = ReflectionHelper.FindType("ModCeilingLight").BaseType;
                Func<string, MethodInfo> GetLightMethod = name => LightType.GetMethod(name, MainFlags);
                MethodInfo TurnOn = GetLightMethod("TurnOn");
                MethodInfo TurnOff = GetLightMethod("TurnOff");
                var LightObject = FindObjectOfType(LightType);
                Action<MethodInfo> InvokeLight = (act) => {
					act.Invoke(LightObject, new object[] { true });
					act.Invoke(LightObject, new object[] { false });
				};
				Audio.PlaySoundAtTransform("ventbang", transform);
                for(int i = 0;i<5;i++)
                {
                    InvokeLight(TurnOff);
                    yield return new WaitForSeconds(.4f);
                    InvokeLight(TurnOn);
                    yield return new WaitForSeconds(.4f);
                }
                InvokeLight(TurnOff);
                StartCoroutine(WaitForAftonPress(() => InvokeLight(TurnOn)));
                yield break;
            }
        }
    }

    private IEnumerator WaitForAftonPress(Action TurnOn)
    {
        for(int i = 0;i<(!TwitchPlaysActive ? 15 : 50);i++)
        {
            if(RightVentClosed || solved)
            {
                Audio.PlaySoundAtTransform("Bang", transform);
                yield return new WaitForSeconds(.1f);
                TurnOn();
                yield break;
            }
            yield return new WaitForSeconds(.2f);
        }
        Strike("Afton");
        yield return new WaitForSeconds(.1f);
        TurnOn();
    }
}
