using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXController : MonoBehaviour
{
    public static List<FXModel> particleList;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void SetParticles(List<FXModel> particleModels)
    {
        particleList = new List<FXModel>();
        foreach (var fx in particleModels)
        {
            particleList.Add(fx);
        }
    }

    private void Initialize()
    {
        var effects = Resources.LoadAll("FX", typeof(FXModel));

        List<FXModel> fxList = new List<FXModel>();

        foreach (var fx in effects)
        {
            fxList.Add((FXModel)fx);
        }

        SetParticles(fxList);
    }
}
