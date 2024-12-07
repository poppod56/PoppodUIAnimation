using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIButtonState
{
    public void OnClick();
    public void OnHover();
    public void OnUnhover();
    public void OnSelect();
    public void OnUnselect();

}
