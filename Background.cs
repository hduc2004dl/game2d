using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform minCam;
    public Transform slide;
    public Transform BackGround;
    public float length;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(minCam.position.x>BackGround.position.x)
        {
            UpdateMidbgPosition(Vector3.right);
        }
        else if(minCam.position.x<BackGround.position.x)
        {
            UpdateMidbgPosition(Vector3.left);
        }    
    }
    void UpdateMidbgPosition(Vector3 direction)
    {
        slide.position = BackGround.position + direction * length;
        Transform temp = BackGround;
        BackGround = slide;
        slide = temp;
    }
}
