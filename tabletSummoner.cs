
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class tabletSummoner : UdonSharpBehaviour
{
    public bool TabletOwner;
    public AudioSource testSound;
    public GameObject Tablet;
    public dronetablet theTab;

    private VRCPlayerApi player;
    private bool mybool;
    void Start()
    {
        TabletOwner = false;
        
    }

    public void newOwner()
    {
        TabletOwner = false;
    }

    public void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "newOwner");
        TabletOwner = true;
    }

    private void Update()
    {
        if(TabletOwner == true)
        {
            mybool = (bool)theTab.GetProgramVariable("isLocked");
            //&& Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") == 0 && Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") == 0
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") >= .5f && mybool == false  || Input.GetKeyDown(KeyCode.N))
            {
                testSound.Play();
                player = Networking.LocalPlayer;
                Networking.SetOwner(player, Tablet);
                Vector3 targetPosition = player.GetPosition() + new Vector3(.5f, 2f, 0f);
                Tablet.transform.position = targetPosition;
            }
        }
    }
}
