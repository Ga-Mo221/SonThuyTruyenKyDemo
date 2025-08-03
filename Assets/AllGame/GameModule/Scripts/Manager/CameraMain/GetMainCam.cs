using UnityEngine;

public class GetMainCam : MonoBehaviour
{
    void Start()
    {
        GameObject mainCamObj = Camera.main?.gameObject;

        Canvas canvas = transform.GetComponent<Canvas>();
        if (canvas != null && mainCamObj != null)
        {
            Camera cam = mainCamObj.GetComponent<Camera>();
            if (cam != null)
            {
                canvas.worldCamera = cam;
            }
            else
            {
                Debug.LogWarning("mainCamObj không chứa component Camera.");
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Canvas hoặc mainCamObj null.");
        }
    }
}
