using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDetectionCollider : MonoBehaviour
{
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
       
    }

    // Update is called once per frame
    void Update()
    {
       transform.position = new Vector3(mainCamera.transform.position.x, gameObject.transform.position.y, mainCamera.transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        //CubeColor currentCube = other.gameObject.GetComponent<CubeColor>();

        other.gameObject.GetComponent<Cell>().activateCell();

    }
    private void OnTriggerExit(Collider other)
    {
        //other.gameObject.GetComponent<GameCell>().setColor(new Color(Color.white.r, Color.white.g, Color.white.b, 0.7f));
        other.gameObject.GetComponent<Cell>().deActivateCell();
    }
}
