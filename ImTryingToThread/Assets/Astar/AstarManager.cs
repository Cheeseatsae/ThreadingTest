using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Harry
{
    
    public class AstarManager : MonoBehaviour
    {
        private Node _startNode;
        private Node _targetNode;
        private Node _nextNode;
        private List<Node> _finalPath = new List<Node>();

        private bool done = false;

        // continue = break except it only does it for one instance of a loop
        public List<Node> openNodes = new List<Node>();
        public List<Node> closedNodes = new List<Node>();

        private GridBuilder _builder;
        System.Random rand = new System.Random();

        private void Awake()
        {
            _builder = GetComponent<GridBuilder>();
        }

        private void Start()
        {
            StartCoroutine(FindPath());

        }

        private void Update()
        {
            if (done)
            {
                StopAllCoroutines();
            }
        }

        [ContextMenu("New Path")]
        public void NewPath()
        {
            openNodes.Clear();
            closedNodes.Clear();
            _finalPath.Clear();
            done = false;
            
            StartCoroutine(FindPath());
        }
        
        
        private IEnumerator FindPath()
        {
            yield return new WaitForSeconds(1);
            
            Thread t = new Thread(StartSearch);
            t.Start();
        }

        private void StartSearch()
        {
            FindPossiblePath();

            while (_nextNode != _targetNode)
            {
                EvaluateNeighbours(_nextNode);
            }
            
            Debug.Log("Done?");
            
            _finalPath = new List<Node>();

            Node current = _targetNode;
            
            while (current.parentNode != null)
            {
                _finalPath.Add(current);
                current = current.parentNode;
            }
            
            _finalPath.Reverse();

            done = true;
            
        }
        
        private void FindPossiblePath()
        {
            
            
            Node testStart = GetNode(RandInt(0 ,_builder.resolution), RandInt(0 ,_builder.resolution));

            while (testStart.occupied)
            {
                testStart = GetNode(RandInt(0 ,_builder.resolution), RandInt(0 ,_builder.resolution));
            }

            _startNode = testStart;
            openNodes.Add(_startNode);
            //Debug.Log("Found Start");

            Node testEnd = GetNode(RandInt(0 ,_builder.resolution), RandInt(0 ,_builder.resolution));

            while (testEnd.occupied)
            {
                testEnd = GetNode(RandInt(0 ,_builder.resolution), RandInt(0 ,_builder.resolution));
            }

            _targetNode = testEnd;
            //Debug.Log("Found End");

            _nextNode = _startNode;
        }

        int RandInt(int x, int y)
        {
            int i = rand.Next(x, y);
            Debug.Log(i);
            return i;
        }
        
        private Node GetNode(int x, int y)
        {
            return _builder.map[x, y];
        }

        private void EvaluateNeighbours(Node n)
        { 
            //Debug.Log("Checking Neighbours");
            if (openNodes.Contains(n))
                openNodes.Remove(n);
            
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0) continue;
                
                    // Checks to not go outside of array    
                    if (n.gridPosition.x + x < 0 || n.gridPosition.x + x > _builder.resolution - 1) continue;
                    if (n.gridPosition.y + y < 0 || n.gridPosition.y + y > _builder.resolution - 1) continue;
                    
                    // current node we're checking
                    Node temp = _builder.map[n.gridPosition.x + x, n.gridPosition.y + y];
                    
                    // if its on a wall ignore it
                    if (temp.occupied) continue;
                    // If its closed we'll ignore it
                    if (closedNodes.Contains(temp)) continue;

                    // calculate path value depending on diagonal positioning
                    float pCost = Vector2.Distance(temp.gridPosition, n.gridPosition) + n.pathCost;
                    float dCost = Vector3.Distance(temp.worldPosition, _targetNode.worldPosition);
                    float tCost = dCost + pCost;
                    
                    if (temp.totalCost < tCost) continue; 
                    
                    // set parent node
                    // This need to take place AFTER the check for a better cost, otherwise the path
                    // will end up being the search direction, not the actual path to take
                    temp.parentNode = n;
                    
                    // setting costs 
                    temp.pathCost = pCost;
                    temp.distCost = dCost;
                    temp.totalCost = tCost;

                    // add to open node list
                    if (!openNodes.Contains(temp))
                        openNodes.Add(temp);
                    
                }
            }
            
            if (!closedNodes.Contains(n))
                closedNodes.Add(n);
            
            FindNextNode();
        }
        

        private void FindNextNode()
        {
            //Debug.Log("Finding Next Node");
            Node lowestCost = new Node();
            lowestCost.totalCost = float.MaxValue;
            
            foreach (Node n in openNodes)
            {
                if (lowestCost.totalCost > n.totalCost)
                {
                    lowestCost = n;
                }
            }

            _nextNode = lowestCost;
        }

        private void OnDrawGizmos()
        {
//            Gizmos.color = Color.blue;
//            if (_nextNode != null)
//                Gizmos.DrawCube(_nextNode.worldPosition, Vector3.one);
//            
//            Gizmos.color = Color.red;
//            if (closedNodes != null)
//                foreach (Node n in closedNodes)
//                {
//                    Gizmos.DrawCube(n.worldPosition, new Vector3(1 * _builder.xCheckSize / 2, 2, 1 * _builder.yCheckSize / 2));
//                }
//            
//            Gizmos.color = Color.yellow;
//            if (openNodes != null)
//                foreach (Node n in openNodes)
//                {
//                    Gizmos.DrawCube(n.worldPosition, new Vector3(1 * _builder.xCheckSize / 2, 2, 1 * _builder.yCheckSize / 2));
//                }
//            
            Gizmos.color = Color.green;
            
            if (_startNode != null && _targetNode != null)
            {
                Gizmos.DrawCube(_startNode.worldPosition, Vector3.one);
                Gizmos.DrawCube(_targetNode.worldPosition, Vector3.one);
            }

            if (!done) return;
            
            foreach (Node n in _finalPath)
            {
                Gizmos.DrawCube(n.worldPosition, new Vector3(1 * _builder.xCheckSize / 2, 2, 1 * _builder.yCheckSize / 2));
            }
            
        }
    }
    
}

