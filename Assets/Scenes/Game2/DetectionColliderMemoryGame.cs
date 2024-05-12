using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionColliderMemoryGame : MonoBehaviour
{
    Camera mainCamera;
    // Start is called before the first frame update
    float activationCellDuration = 1f;
    MemoryGame game;
    void Start()
    {
        mainCamera = Camera.main;
        game = FindObjectOfType<MemoryGame>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(mainCamera.transform.position.x, gameObject.transform.position.y, mainCamera.transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        //CubeColor currentCube = other.gameObject.GetComponent<CubeColor>();
        CellMemoryGame currentCell = other.gameObject.GetComponent<CellMemoryGame>();
        currentCell.setColor(currentCell.currentCellColor);
        currentCell.activateCellAndCheckFinish(activationCellDuration);
       

    }
    private void OnTriggerExit(Collider other)
    {
        //other.gameObject.GetComponent<GameCell>().setColor(new Color(Color.white.r, Color.white.g, Color.white.b, 0.7f));
        other.gameObject.GetComponent<CellMemoryGame>().cancelInvokeaAtivateCellAndCheckFinish();
        other.gameObject.GetComponent<CellMemoryGame>().deActivateCell();
    }
}
