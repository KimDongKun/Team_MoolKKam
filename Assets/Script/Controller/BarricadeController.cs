using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarricadeController : BuildObject
{
    public GameObject endingUI_DieStatu;
    public bool IsStatue = false;

    private void OnDisable()
    {
        if (IsStatue && currentHP <= 0)
        {
            endingUI_DieStatu.SetActive(true);
        }
    }
}
