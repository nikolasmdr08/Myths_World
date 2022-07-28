using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public int value;
    public Point index;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public NodePiece flipped;
    [HideInInspector]
    public RectTransform rect;

    bool updating;
    Image img;

    public void Initialize(int v, Point p, Sprite piece) 
    {
        flipped = null;
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        value = v;
        SetIndex(p);
        img.sprite = piece;
    }

    public void SetIndex(Point p) {
        index = p;
        ResetPosition();
        UpdateName();
    }

    public void ResetPosition() {
        pos = new Vector2(0 + (64 * index.x), 0 - (64 * index.y));
    }

    public void MovePosition(Vector2 move)
    {
        rect.anchoredPosition += move*Time.deltaTime * 16f;
    }
    public void MovePositionTo(Vector2 move)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
    }
    void UpdateName() {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }

    public bool UpdatePiece()
    {
        if(Vector3.Distance(rect.anchoredPosition, pos)> 1)
        {
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
        return true;
        //return false if it is not moving
    }
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Let go" + transform.name);
        MovePieces.instance.DropPiece();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (updating) return;
        MovePieces.instance.MovePiece(this);
        Debug.Log("Grab" + transform.name);
    }
}
