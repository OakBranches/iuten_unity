using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject firstSquare;
    [SerializeField] private float xoffset;
    [SerializeField] private float yoffset;

    private List<List<GameObject>> table = new List<List<GameObject>>();

    void Start()
    {
        for (int i = 0; i < 11; i++)
        {
            List<GameObject> row = new List<GameObject>();
            for (int j = 0; j < 9; j++)
            {
                GameObject square = Instantiate(firstSquare, new Vector3(firstSquare.transform.position.x + xoffset * j, firstSquare.transform.position.y + yoffset * i, firstSquare.transform.position.z), Quaternion.identity);
                row.Add(square);
            }
            table.Add(row);
        }
        firstSquare.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Vector3 position = new Vector3(firstSquare.transform.position.x + xoffset * j, firstSquare.transform.position.y + yoffset * i, firstSquare.transform.position.z);
                table[i][j].transform.position = position;
            }
        }
    }
}
