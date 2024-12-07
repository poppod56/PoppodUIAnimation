using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIState
{

    public void OnStart();
    public void OnUpdate();
    public void OnEnd();
}
