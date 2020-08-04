using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //public SpriteRenderer rink;
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    int width = 5;
    int height = 5;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;

        if (widthInput.text != "" && heightInput.text != "")
        {
            width = int.Parse(widthInput.text);
            height = int.Parse(heightInput.text);
        }

        float targetRatio = width / height;
        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = height / 2 + 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = height / 2 * differenceInSize + 2;
        }

        Camera.main.rect = new Rect(0, 0, width + 2, height + 2);
    }
}
