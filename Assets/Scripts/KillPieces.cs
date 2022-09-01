using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillPieces : MonoBehaviour
{
    float speed = 5f;
    RectTransform rect;
    Image img;
    public int piece;

    public Vector3 target;



    // Start is called before the first frame update
    public void Initialize(Sprite sprite, Vector2 start, int val)
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        img.sprite = sprite;
        rect.anchoredPosition = start;
        piece = val;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed);
    }

    public int ValuePiece() {
        return this.piece; 
    }
}
