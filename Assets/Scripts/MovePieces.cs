using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    public static MovePieces instance;
    Match game;

    NodePiece moving;
    Point newIndex;
    Vector2 mouseStart;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        game = GetComponent<Match>();
    }

    void Update()
    {
        if(moving != null)
        {
            Vector2 dir = ((Vector2)Input.mousePosition - mouseStart);
            Vector2 nDir = dir.normalized;
            Vector2 aDir= new Vector2 (Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            newIndex = Point.clone(moving.index);
            Point add = Point.zero;
            if(dir.magnitude > 32)
            {
                if (aDir.x > aDir.y)
                    add = (new Point((nDir.x> 0) ? 1 : -1, 0));
                else if(aDir.y> aDir.x)
                    add = (new Point(0,(nDir.y > 0) ? -1 : 1));
            }
            newIndex.add(add);

            Vector2 pos = game.getPositionFromPoint(moving.index);
            if (!newIndex.Equals(moving.index))
                pos += Point.mult(new Point(add.x,-add.y), 32).ToVector();
            moving.MovePositionTo(pos);
        }
    }
    public void MovePiece(NodePiece piece)
    {
        if(moving != null) return;
        moving = piece;
        mouseStart = Input.mousePosition;
        
        
    }
    public void DropPiece() 
    {
        if (moving == null) {
            //Match.estado = "idle";
            return;
        }

        if (!newIndex.Equals(moving.index)){
            game.FlipPieces(moving.index, newIndex,true);
            Match.estado = "Atacando";
        }
        else {
            game.ResetPiece(moving);
            Match.estado = "idle";
        }
        
        moving = null;
    }
}
