using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    public ArrayLayout boardLayout; /// verificar si se puede cambiar por json o txt

    [Header("UI Elements")]
    public RectTransform gameBoard;
    public RectTransform killedBoard;
    public Sprite[] pieces;

    [Header("Prefabs")]
    public GameObject nodePieces;
    public GameObject killedPieces;

    int width = 9;
    int height = 11;
    Node[,] board;
    int[] fills;

    List<NodePiece> update;
    List<FlippedPiece> flipped;
    List<NodePiece> dead;
    List<KillPieces> killed;

    public static string estado = "idle";

    System.Random random;

    void Start() {
        StartGame();
    }

    private void Update() {
        List<NodePiece> finishUpdating = new List<NodePiece>();

        for (int i = 0; i < update.Count; i++) {
            NodePiece piece = update[i];
            if (!piece.UpdatePiece()) finishUpdating.Add(piece);
        }
        for (int i = 0; i < finishUpdating.Count; i++) {
            NodePiece piece = finishUpdating[i];
            FlippedPiece flip = getFlipped(piece);
            NodePiece flippedPiece = null;

            int x = (int)piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);

            List<Point> connected = isConnected(piece.index, true);
            bool wasFlipped = (flip != null);

            if (wasFlipped) {
                flippedPiece = flip.getOtherPiece(piece);
                AddPoints(ref connected, isConnected(flippedPiece.index, true));
            }
                
            if (connected.Count == 0) {
                if (wasFlipped) {
                    FlipPieces(piece.index, flippedPiece.index, false);
                }
            }
            else {
                foreach (Point pnt in connected) {
                    KillPiece(pnt);
                    Node node = getNodeAtPoint(pnt);
                    NodePiece nodePiece = node.getPiece();
                    if (nodePiece != null) {
                        nodePiece.gameObject.SetActive(false);
                        dead.Add(nodePiece);
                    }
                    node.SetPiece(null);
                }
                ApplyGravityToBoard();
            }
            flipped.Remove(flip);
            update.Remove(piece);
        }
        
    }

    void ApplyGravityToBoard() {
        for (int x = 0; x < width; x++) {
            for (int y = (height-1); y >= 0; y--) {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int val = getValueAtPoint(p);
                if (val != 0) continue;
                for (int ny = (y-1); ny >= -1; ny--) {
                    Point next = new Point(x, ny);
                    int nextVal = getValueAtPoint(next);
                    if (nextVal == 0) continue;
                    if(nextVal != -1) {
                        Node got = getNodeAtPoint(next);
                        NodePiece piece = got.getPiece();

                        //set hole
                        node.SetPiece(piece);
                        update.Add(piece);

                        //replace the hole
                        got.SetPiece(null);
                    }
                    else {
                        int newVal = fillPiece();
                        NodePiece piece;
                        Point fallPnt = new Point(x, (-1 - fills[x]));

                        if(dead.Count > 0) {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            revived.rect.anchoredPosition = getPositionFromPoint(fallPnt);
                            piece = revived;

                            dead.RemoveAt(0);
                        }
                        else {
                            GameObject obj = Instantiate(nodePieces, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            piece = n;
                        }

                        piece.Initialize(newVal, p, pieces[newVal - 1]);

                        Node hole = getNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    break;
                }
            }
        }
    }

    FlippedPiece getFlipped(NodePiece p) {
        FlippedPiece flip = null;
        for (int i = 0; i < flipped.Count; i++) {
            if (flipped[i].getOtherPiece(p) != null) {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }


    void StartGame() {
        fills = new int[width];
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPiece>();
        dead = new List<NodePiece>();
        killed = new List<KillPieces>();

        initializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }

    void initializeBoard() {
        board = new Node[width,height];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1 : fillPiece(), new Point(x, y));
            }
        }
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (getValueAtPoint(one) < 0) return;

        Node nodeOne = getNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.getPiece();

        if (getValueAtPoint(two) > 0) {
            Node nodeTwo = getNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.getPiece();

            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if(main)
                flipped.Add(new FlippedPiece(pieceOne,pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else {
            ResetPiece(pieceOne);
        }
    }

    void KillPiece(Point p) {
        List<KillPieces> available = new List<KillPieces>();
        KillPieces set = null;
        if (available.Count > 0) {
            set = available[0];
        }
        else {
            GameObject kill = GameObject.Instantiate(killedPieces, killedBoard);
            KillPieces kPiece = kill.GetComponent<KillPieces>();
            set = kPiece;
            killed.Add(kPiece);
        }

        int val = getValueAtPoint(p) - 1;
        if (set != null && val > 0 && val < pieces.Length)
            set.Initialize(pieces[val], getPositionFromPoint(p), val);
    }

    void VerifyBoard() {
        List<int> remove;
        for (int x  = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();
                while(isConnected(p,true).Count > 0) {
                    val = getValueAtPoint(p);
                    if (!remove.Contains(val))
                        remove.Add(val);
                    setValueAtPoint(p, newValue(ref remove));
                }
            }
        }
    }

    void InstantiateBoard() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Node node = getNodeAtPoint(new Point(x, y));
                int val = board[x, y].value;
                
                if (val <= 0) continue;

                GameObject p = Instantiate(nodePieces, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0 + (64 * x), 0 - (64 * y));
                piece.Initialize(val, new Point(x,y), pieces[val-1]);
                node.SetPiece(piece);
            }
        }
    }

    List<Point> isConnected(Point p, bool main) {
        List<Point> connected = new List<Point>();
        int val = getValueAtPoint(p);

        Point[] directions = { Point.up, Point.right, Point.down, Point.left };

        foreach (Point dir in directions) { //chequea si hay 2 o mas iconos iguales en cada direccion
            List<Point> line = new List<Point>();

            int same = 0;
            for (int i = 1; i < 3; i++) {
                Point check = Point.add(p, Point.mult(dir, i));
                if (getValueAtPoint(check) == val) {
                    Point puntoCto = Point.add(p, Point.mult(dir, (-1*i)));
                    if(getValueAtPoint(puntoCto) == val) {
                        line.Add(puntoCto);
                    }
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1) { // si hay mas de uno hubo un match
                AddPoints(ref connected, line); // agreco los puntos conectados a la lista
            }
        }
        
        for (int i = 0; i < 2; i++) { // chequeo si hay un match entre dos píezas distintas
            List<Point> line = new List<Point>();
            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };
            foreach (Point next in check) { //chequeo los bordes para ver si tienen el mismo valor
                if (getValueAtPoint(next) == val) {
                    line.Add(next);
                    same++;
                }
            }
            if (same > 1) {
                AddPoints(ref connected, line);
            }
        }
        
        for (int i = 0; i < 4; i++) {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;
            if (next >= 4) {
                next -= 4;
            }

            Point[] check = { 
                Point.add(p, directions[i]), 
                Point.add(p, directions[next]), 
                Point.add( p, Point.add(directions[i], directions[next]))
            };

            foreach (Point pnt in check) {
                if (getValueAtPoint(pnt) == val) {
                    square.Add(pnt);
                    same++;
                }
            }
            if (same > 2) {
                AddPoints(ref connected, square);
            }

        }

        if (main) { // chequeo de matchs no previstos
            for (int i = 0; i < connected.Count; i++) {
                AddPoints(ref connected, isConnected(connected[i], false));
            }
        }
        
        if (connected.Count > 0) {
            connected.Add(p);
        }
        
        return connected;
    }

    void AddPoints( ref List<Point> points, List<Point> add) {
        foreach(Point p in add) {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++) {
                if (add[i].Equals(p)) {
                    doAdd = false;
                    break;
                }
            }
            if (doAdd) points.Add(p);
        }
    }

    private int fillPiece() {
        int val = 1;
        val = Random.Range(1, pieces.Length);
        return val;
    }

    int getValueAtPoint(Point p) {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }
    void setValueAtPoint(Point p , int v) {
        board[p.x, p.y].value = v;
    }

    int newValue(ref List<int> remove) {
        List<int> aviable = new List<int>();
        for (int i = 0; i < pieces.Length; i++) {
            aviable.Add(i + 1);
        }
        foreach (int i in remove) {
            aviable.Remove(i);
        }

        if(aviable.Count <= 0) {
            return 0;
        }
        else {
            return aviable[random.Next(0, aviable.Count)];
        }
    }

    string getRandomSeed() {
        string seed = "";
        string aceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#%&/()";
        for (int i = 0; i < 20; i++) {
            seed += aceptableChars[Random.Range(0, aceptableChars.Length)];
        }
        return seed;
    }

    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(0 + (64 * p.x), -0 - (64 * p.y));
    }

}

[System.Serializable]
public class Node
{
    public int value;
    public Point index;
    NodePiece piece;

    public Node(int v, Point i) {
        value = v;
        index = i;
    }
    public void SetPiece(NodePiece p)
    {
        piece = p;
        value = (piece == null) ? 0 : piece.value;
        if (piece == null) return;
        piece.SetIndex(index);
    }

    public NodePiece getPiece()
    {
        return piece;
    }
}

[System.Serializable] 
public class FlippedPiece
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPiece(NodePiece o, NodePiece t) {
        one = o;
        two = t;
    }

    public NodePiece getOtherPiece(NodePiece p) {
        if (p == one) {
            return two;
        }
        else if(p == two) {
            return one;
        }
        else {
            return null;
        }
    }
}