using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Harry
{
    [Serializable]
    public class Node
    {
  
        public bool occupied = false;
        public Vector3 worldPosition = Vector3.zero;
        public Vector2Int gridPosition = new Vector2Int(0,0);
        public float pathCost = 0;
        public float distCost = 0;
        public float totalCost = float.MaxValue;
        public Node parentNode = null;

    }
}


