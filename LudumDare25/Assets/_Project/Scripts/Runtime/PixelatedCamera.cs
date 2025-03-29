using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;


//all thanks to https://www.youtube.com/watch?v=L-tNbbov6Bo

//for a pixelate effect on individual objects check out https://www.youtube.com/watch?v=Z8xB7i3W4CE
//or https://www.youtube.com/watch?v=-8xlPP4qgVo
public enum PixelScreenMode
{
    Resize,
    Scale
}

[System.Serializable]
public struct ScreenSize
{
    public int width;
    public int height;
}

public class PixelatedCamera : MonoBehaviour
{
    [Header("Screen scaling settings")]
    public PixelScreenMode mode;

    [OnValueChanged("Init")]
    public ScreenSize targetScreenSize = new ScreenSize { width = 256, height = 144 };

    [OnValueChanged("Init")]
    public uint screenScaleFactor = 1;

    [Header("Display")]
    public RawImage display;

    private Camera _renderCamera;
    private RenderTexture _renderTexture;
    private int _screenWidth, _screenHeight;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (HasScreenResized()) Init();
    }

    private bool HasScreenResized()
    {
        return Screen.width != _screenWidth || Screen.height != _screenHeight;
    }

    private void Init()
    {
        //init the camera and get screen size values
        if (!_renderCamera) _renderCamera = GetComponent<Camera>();
        _screenHeight = Screen.height;
        _screenWidth = Screen.width;
        var color = display.color;
        color.a = 255;
        display.color = color;

        //prevent any error
        if (screenScaleFactor < 1) screenScaleFactor = 1;
        if (targetScreenSize.width < 1) targetScreenSize.width = 1;
        if (targetScreenSize.height < 1) targetScreenSize.height = 1;

        //calculate the render texture size
        int width = mode == PixelScreenMode.Resize
            ? (int)targetScreenSize.width
            : _screenWidth / (int)screenScaleFactor;
        int height = mode == PixelScreenMode.Resize
            ? (int)targetScreenSize.height
            : _screenHeight / (int)screenScaleFactor;

        //init render texture
        _renderTexture = new RenderTexture(width, height, 24)
        {
            filterMode = FilterMode.Point,
            antiAliasing = 1
        };

        //set render texture as cameras output
        _renderCamera.targetTexture = _renderTexture;
        //attach texture to display ui rawImage
        display.texture = _renderTexture;
    }
}