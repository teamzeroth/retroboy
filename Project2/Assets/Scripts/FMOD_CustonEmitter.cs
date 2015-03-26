using UnityEngine;
using System.Collections;

public class FMOD_CustonEmitter : FMOD_StudioEventEmitter {

    public void Init(FMODAsset asset)  {
        this.asset = asset;
        startEventOnAwake = false;
    }

    public void Init(string path = "") {
        this.path = path;
        startEventOnAwake = false;
    }

    public void SetParameter(string name, float value) {
        FMOD.Studio.ParameterInstance parameter = getParameter(name);
        parameter.setValue(value);
    }
}