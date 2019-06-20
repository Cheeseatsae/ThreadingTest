using UnityEngine;

namespace Harry
{

    public class GridBuilder : MonoBehaviour
    {       
        public Vector3 startPoint;
        public Vector3 endPoint;
        public float xDist;
        [HideInInspector] public float xCheckSize;
        public float yDist;
        [HideInInspector] public float yCheckSize;

        private GameObject floor;

        public int resolution = 10;

        public Node[,] map;
        
        private void Awake()
        {           
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 100f);
            floor = hit.collider.gameObject;
            
            map = new Node[resolution,resolution];
            FindGrid();
        }

        private void FindGrid()
        {
            startPoint = floor.GetComponent<Collider>().bounds.min;
            endPoint = floor.GetComponent<Collider>().bounds.max;

            xDist = Mathf.Abs(endPoint.x - startPoint.x);
            yDist = Mathf.Abs(endPoint.z - startPoint.z);

            xCheckSize = xDist / resolution;
            yCheckSize = yDist / resolution;

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    Vector3 checkPos = new Vector3(startPoint.x + (xCheckSize * j), startPoint.y + 1, startPoint.z + (yCheckSize * i));
                    map[j, i] = new Node();
                    map[j, i].worldPosition = checkPos - Vector3.up;
                    map[j, i].gridPosition = new Vector2Int(j,i);
                    
                    if (Physics.CheckBox(checkPos, new Vector3(xCheckSize / 2, 0.5f, yCheckSize / 2), Quaternion.identity))
                        map[j, i].occupied = true;
                    else
                        map[j, i].occupied = false;
                }
            }

        }

        private void Update()
        {
            foreach (Node n in map)
            {
                Debug.DrawLine(n.worldPosition, n.worldPosition + (n.occupied ? (Vector3.up * 2f) : (Vector3.up * 0.2f)), n.occupied ? Color.red : Color.cyan);
            }
        }
    }
    
}
