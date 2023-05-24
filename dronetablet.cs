using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class dronetablet : UdonSharpBehaviour
{
    public GameObject thescroller;
    public bool isLocked;
    public GameObject drone;
    private VRCPlayerApi player;
    public GameObject targetObject;
    public float speed = 5.0f;
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public float mouseSensitivity = 1.0f;

    public ParticleSystem bombs;
    public AudioSource bombSound;

    public float state2Duration = 2f;
    public float yOffset = 4f;

    public float swaySpeed = 1.0f;
    public float swayIntensity = 1.0f;
    public float rotationSpeed = 1.0f;
    public float rotationIntensity = 1.0f;

    public AudioSource audiosource;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public Text logText;
    public Slider scrollbar;
    public float progressSpeed = 0.05f;

    private string[] logMessages = { "Logging in as ", "Establishing connection", "Connected", "Disconnected" };
    private string recordingText = "Operator {playerName}  Recording";

    private string playerName;
    private float targetValue;
    private int state;
    private float state2Timer;

    public void Start()
    {
        player = Networking.LocalPlayer;
        originalPosition = drone.transform.position;
        originalRotation = drone.transform.rotation;
    }

    public void OnPickup()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        player = Networking.LocalPlayer;
        Networking.SetOwner(localPlayer, targetObject);
        Networking.SetOwner(localPlayer, drone);
        
        playerName = localPlayer.displayName;
        logText.text = logMessages[0] + playerName;
        state = 1;
        thescroller.SetActive(true);
    }

    public void DropBomb()
    {
        bombs.Play();
    }
    public void OnPickupUseDown()
    {
        isLocked = true;
        player.Immobilize(true);
        player.SetWalkSpeed(0);
        player.SetStrafeSpeed(0);
    }
    public void OnPickupUseUp()
    {
        isLocked = false;       
        player.Immobilize(false);
        player.SetWalkSpeed(2);
        player.SetStrafeSpeed(2);       
    }

    public void OnDrop()
    {
        logText.text = logMessages[3];
        state = 0;
    }
    public void ChangeStateToRecording()
    {
        state = 3;
        targetValue = 0f;
    }

    void Update()
    {
        if (isLocked)
        {
            float joystickHorizontal = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal");
            float joystickVertical = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical");
            float keyboardHorizontal = Input.GetAxis(horizontalAxis);
            float keyboardVertical = Input.GetAxis(verticalAxis);
            float mouseHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseVertical = Input.GetAxis("Mouse Y") * mouseSensitivity;
            float secondaryJoystickHorizontal = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal");
            float secondaryJoystickVertical = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical");

            float horizontal = joystickHorizontal != 0 ? joystickHorizontal : keyboardHorizontal;
            float vertical = joystickVertical != 0 ? joystickVertical : keyboardVertical;
            float height = secondaryJoystickVertical != 0 ? secondaryJoystickVertical : mouseVertical;
            float mouseX = secondaryJoystickHorizontal != 0 ? secondaryJoystickHorizontal : mouseHorizontal;

            drone.transform.Translate(new Vector3(-horizontal, 0f + height, -vertical) * Time.deltaTime * speed);
            drone.transform.Rotate(Vector3.up, mouseX * speed, Space.World);

            originalPosition = drone.transform.position;
            originalRotation = drone.transform.rotation;
            if (Input.GetButtonDown("Oculus_CrossPlatform_Button4") || Input.GetKeyDown(KeyCode.M))
            {
                //audiosource.Play();
                Vector3 targetPosition = player.GetPosition() + new Vector3(0f, yOffset, 0f);
                drone.transform.position = targetPosition;
            }
            if (Input.GetButtonDown("Jump") )
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DropBomb");
                DropBomb();
            }
        }
        else {
            // Calculate the sway and rotation based on the current time
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayIntensity;
            float swayY = Mathf.Sin(Time.time * swaySpeed * 2) * swayIntensity;
            float swayZ = Mathf.Sin(Time.time * swaySpeed * 3) * swayIntensity;
            float rotationX = Mathf.Sin(Time.time * rotationSpeed) * rotationIntensity;
            float rotationY = Mathf.Sin(Time.time * rotationSpeed * 2) * rotationIntensity;
            float rotationZ = Mathf.Sin(Time.time * rotationSpeed * 3) * rotationIntensity;

            // Apply the sway and rotation to the object's position and rotation
            drone.transform.position = originalPosition + new Vector3(swayX, swayY, swayZ);
            drone.transform.rotation = originalRotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
        }
        switch (state)
        {
            case 1:
                logText.text = logMessages[1];
                targetValue += progressSpeed * Time.deltaTime;
                scrollbar.value = Mathf.Clamp01(targetValue);
                if (scrollbar.value >= 1f)
                {
                    state = 2;
                    targetValue = 0f;
                }
                break;

            case 2:
                //logText.text = recordingText.Replace("{playerName}", playerName);
                logText.text = logMessages[2];
                targetValue += progressSpeed * Time.deltaTime;
                state2Timer += Time.deltaTime;
                if (state2Timer >= state2Duration)
                {
                    state = 3;
                    targetValue = 0f;
                    thescroller.SetActive(false);
                }
                break;

            case 3:
                logText.text = recordingText.Replace("{playerName}", playerName);
                targetValue += progressSpeed * Time.deltaTime;
                if (targetValue >= 0.5f)
                {
                    logText.text = "";
                    targetValue = 0f;
                }
                break;
        }

    }
}