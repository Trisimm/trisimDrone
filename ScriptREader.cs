
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ScriptREader : UdonSharpBehaviour
{
    public GameObject targetObject;
    public float toggleInterval = 2f;

    private bool isActive = false;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= toggleInterval)
        {
            isActive = !isActive;
            targetObject.SetActive(isActive);

            timer = 0f;
        }
    }
}
