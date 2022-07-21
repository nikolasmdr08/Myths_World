using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    public ArrayLayout boardLayout; /// verificar si se puede cambiar por json o txt

    [Header("UI Elements")]
    public RectTransform gameBoard;
    public Sprite[] pieces;

    [Header("Prefabs")]
    public GameObject nodePieces;

    int width = 9;
    int height = 11;
    Node[,] board;

    System.Random random;

    void Start()
    {
        StartGame();
    }

    void StartGame() {

        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());

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
                int val = board[x, y].value;
                
                if (val <= 0) continue;

                GameObject p = Instantiate(nodePieces, gameBoard);
                NodePiece node = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0 + (64 * x), 0 - (64 * y));
                node.Initialize(val, new Point(x,y), pieces[val-1]);
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
            for (int i = 0; i < add.Count; i++) {
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
        //val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        val = Random.Range(1, pieces.Length);
        return val;
    }

    int getValueAtPoint(Point p) {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
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

    void Update()
    {
        
    }

    string getRandomSeed() {
        string seed = "";
        string aceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#%&/()";
        for (int i = 0; i < 20; i++) {
            seed += aceptableChars[Random.Range(0, aceptableChars.Length)];
        }
        return seed;
    }
}

[System.Serializable]
public class Node
{
    public int value;
    public Point index;

    public Node(int v, Point i) {
        value = v;
        index = i;
    }
}