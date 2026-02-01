using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ball
{
    public class BallSynchronizer : MonoBehaviour
    {
        public Transform hamsterballParent;
        List<GameObject> hamsterBalls = new List<GameObject>();

        public float interval;
        
        List<Vector3> positions = new List<Vector3>();

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            hamsterBalls.Clear();
            for (int i = 0; i < hamsterballParent.childCount; i++)
            {
                hamsterBalls.Add(hamsterballParent.GetChild(i).gameObject);
            }
            positions.Clear();
        }

        void FixedUpdate()
        {
            if (hamsterBalls.Count == 1)
                return;
            positions.Add(hamsterBalls[0].transform.position);
            Vector3 currentPosition = hamsterBalls[0].transform.position;
            Vector3 forward = hamsterBalls[0].transform.forward;
            while (positions.Count > 1 && Vector3.Dot(currentPosition - positions[1], forward) > interval)
            {
                positions.RemoveAt(0);
            }
            
            for (int i = 1, j = positions.Count - 1; i < hamsterBalls.Count; i++)
            {
                while (j >= 0 && Vector3.Dot(currentPosition - positions[j], forward) < interval * i)
                {
                    j--;
                }
                if (j < 0)
                    break;
                hamsterBalls[i].transform.position = positions[j];
            }
        }
    }

}
