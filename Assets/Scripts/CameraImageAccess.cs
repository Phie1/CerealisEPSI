using UnityEngine;
using System.Collections;

using Vuforia;
using UnityEngine.UI;

public class CameraImageAccess : MonoBehaviour
{
    Texture2D camTexture;
    public Camera cam;
    public GameObject plane;
    public Text peau;
    public Text museau;
    public Text ventre;

    public Vector3 peauPositionOffset;
    public Vector3 museauPositionOffset;
    public Vector3 ventrePositionOffset;

    public GameObject Model3D;

    Texture2D screenshot;
    public RawImage screenshotDisplay;
    Color[] greenColorBlock;

    Renderer Model3Drenderer;
    float xRatio = 1;
    float yRatio = 1;


    #region PRIVATE_MEMBERS

    private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.UNKNOWN_FORMAT;

    private bool mAccessCameraImage = true;
    private bool mFormatRegistered = false;

    bool needFocus;

    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS

    void Start()
    {

#if UNITY_EDITOR
        mPixelFormat = Vuforia.Image.PIXEL_FORMAT.GRAYSCALE; // Need Grayscale for Editor
#else
        mPixelFormat = Vuforia.Image.PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
#endif

        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);

        peauPositionOffset = new Vector3(0.5f, 0.0f, 3.0f);
        museauPositionOffset = new Vector3(1.0f, 0, 1.0f);
        ventrePositionOffset = new Vector3(-3.0f, 0, 3.5f);

        screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.name = "VuforiaBkgCam";
        greenColorBlock = new Color[100];
        for (int i = 0; i < 100; i++)
            greenColorBlock[i] = Color.green;
    }

    void Update()
    {
        if (needFocus)
        {

            var vd = CameraDevice.Instance;
            bool focused = vd.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

            if (focused)
            {
                needFocus = false;
            }
        }
#if UNITY_EDITOR

#else
        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            xRatio = ((float)screenshot.width / (float)Screen.height);
            yRatio = ((float)screenshot.height / (float)Screen.width);
        }
        else
        {
            xRatio = ((float)screenshot.width / (float)Screen.width);
            yRatio = ((float)screenshot.height / (float)Screen.height);
        }
#endif
    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    void OnApplicationFocus(bool focusStatus)
    {
        needFocus = true;
    }



    void OnVuforiaStarted()
    {

        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());

            mFormatRegistered = true;


            Model3Drenderer = Model3D.GetComponent<Renderer>();
#if UNITY_EDITOR
            xRatio = ((float)640 / (float)screenshot.width);
            yRatio = ((float)480 / (float)screenshot.height);
#else
                   
        xRatio = ((float)screenshot.width / (float)Screen.width);
        yRatio = ((float)screenshot.height / (float)Screen.height);

#endif
        }
        else
        {
            Debug.LogError(
                "\nFailed to register pixel format: " + mPixelFormat.ToString() +
                "\nThe format may be unsupported by your device." +
                "\nConsider using a different pixel format.\n");

            mFormatRegistered = false;
        }

    }

    /// <summary>
    /// Called each time the Vuforia state is updated
    /// </summary>
    void OnTrackablesUpdated()
    {
        if (mFormatRegistered)
        {
            if (mAccessCameraImage)
            {
                Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);

                if (image != null)
                {
                    //Debug.Log(
                    //    "\nImage Format: " + image.PixelFormat +
                    //    "\nImage Size:   " + image.Width + "x" + image.Height +
                    //    "\nBuffer Size:  " + image.BufferWidth + "x" + image.BufferHeight +
                    //    "\nImage Stride: " + image.Stride + "\n"
                    //);

                    // lis les pixels du flux video Vuforia

#if UNITY_EDITOR
                    screenshot.ReadPixels(new Rect(0, 0, 640, 480), 0, 0);
                    screenshot.Apply();
#else
                    //screenshot.ReadPixels(new Rect(0, 0, 640, 480), 0, 0);
                    //screenshot.Apply();
                    image.CopyToTexture(screenshot);
                    screenshot.Apply();                    
#endif


                    // positionne les "points à l'ecran

                    // Peau
                    Vector3 point = plane.transform.position + plane.transform.TransformPoint(peauPositionOffset);
                    Vector3 screenPos0 = cam.WorldToScreenPoint(point);
                    peau.transform.position = new Vector3(screenPos0.x, screenPos0.y, 0);
                    //print("screenPos0x: " + screenPos0.x + "screenPos0y: " + screenPos0.y);
                    int video_x = (int)(screenPos0.x * xRatio);
#if UNITY_EDITOR
                    int video_y = (int)((Screen.height - screenPos0.y) * yRatio);
#else                    
                    int video_y = (int)((Screen.height - screenPos0.y) * yRatio);
#endif
                    Color peauPixelColor = screenshot.GetPixel(video_x, video_y);
                    

                    //museau
                    point = plane.transform.position + plane.transform.TransformPoint(museauPositionOffset);
                    Vector3 screenPos1 = cam.WorldToScreenPoint(point);
                    museau.transform.position = new Vector3(screenPos1.x, screenPos1.y, 0);

                    video_x = (int)(screenPos1.x * xRatio);
#if UNITY_EDITOR
                    video_y = (int)((Screen.height - screenPos1.y) * yRatio);
#else                    
                    video_y = (int)((Screen.height - screenPos1.y) * yRatio);
#endif
                    Color museauPixelColor = screenshot.GetPixel(video_x, video_y);

                    //ventre
                    point = plane.transform.position + plane.transform.TransformPoint(ventrePositionOffset);
                    Vector3 screenPos2 = cam.WorldToScreenPoint(point);
                    ventre.transform.position = new Vector3(screenPos2.x, screenPos2.y, 0);

                    video_x = (int)(screenPos2.x * xRatio);
#if UNITY_EDITOR
                    video_y = (int)((Screen.height - screenPos2.y) * yRatio);
#else                    
                    video_y = (int)((Screen.height - screenPos0.y) * yRatio);
#endif
                    Color ventrePixelColor = screenshot.GetPixel(video_x, video_y);

                    // le point vert (debug)
                    video_x = (int)(screenPos0.x * xRatio);
#if UNITY_EDITOR
                    video_y = (int)((Screen.height - screenPos0.y) * yRatio);
#else                    
                    video_y = (int)((Screen.height - screenPos0.y) * yRatio);
#endif
                    //print("video_x: " + video_x + "  video_y:" + video_y + "width =" + screenshot.width + "height =" + screenshot.height);
                    if ( video_x > 0 &&  video_x + 10 < screenshot.width  && video_y > 0 && video_y + 10 < screenshot.width)
                    {
                        screenshot.SetPixels(video_x, video_y,10,10, greenColorBlock);
                        screenshot.Apply();
                    }
                    // vignette debug
                    screenshotDisplay.texture = screenshot;
                    // fin le point vert (debug)


                    // applique la couleur au point
                    UnityEngine.UI.Image img = peau.GetComponentsInChildren<UnityEngine.UI.Image>()[0];
                    img.color = peauPixelColor;

                    img = museau.GetComponentsInChildren<UnityEngine.UI.Image>()[0];
                    img.color = museauPixelColor;

                    img = ventre.GetComponentsInChildren<UnityEngine.UI.Image>()[0];
                    img.color = ventrePixelColor;

                    for(int i=0; i< Model3Drenderer.materials.Length; i++)
                    {
                        //renderer.materials[i].shader = Shader.Find("_Color");
                        if (Model3Drenderer.materials[i].name == "corps (Instance)")
                            Model3Drenderer.materials[i].color = peauPixelColor;
                        if (Model3Drenderer.materials[i].name == "museau (Instance)")
                            Model3Drenderer.materials[i].color = museauPixelColor;
                        if (Model3Drenderer.materials[i].name == "ventre")
                            Model3Drenderer.materials[i].color = ventrePixelColor;
                    }              


                    //if (camTexture == null)
                    //    camTexture = new Texture2D(image.Width, image.Height);

                        //camTexture.LoadImage(image.Pixels);

                        //camTexture.name = "cam texture";
                        //camTexture.LoadRawTextureData(image.Pixels);
                        //var data = camTexture.GetRawTextureData<Color32>();
                        //int index = 0;
                        //for (int y = 0; y < camTexture.height; y++)
                        //{
                        //    for (int x = 0; x < camTexture.width; x++)
                        //    {
                        //        data[index++] = image.Pixels[index++];
                        //    }
                        //}


                        //camTexture.Apply();

                        //Renderer renderer = GetComponent<Renderer>();
                        //renderer.material.mainTexture = camTexture;

                    //byte[] pixels = image.Pixels;

                    //if (pixels != null && pixels.Length > 0)
                    //{
                    //    Debug.Log(
                    //        "\nImage pixels: " +
                    //        pixels[0] + ", " +
                    //        pixels[1] + ", " +
                    //        pixels[2] + ", ...\n"
                    //    );
                    //}
                }
            }
        }
    }

    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }

    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }

    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }

#endregion //PRIVATE_METHODS
}