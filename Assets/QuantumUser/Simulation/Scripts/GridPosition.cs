using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Deterministic;

namespace Quantum
{
    public class GridPosition : MonoBehaviour
    {
        [SerializeField] private FP x, y;
        [SerializeField] private FP posX, posY;
        [SerializeField] private int posArray;
        public static event Action<FP, FP, FP, FP, int> Click;
        private void OnMouseDown()
        {
            //Debug.Log("Position Click!" + x.ToString() + ", " + y.ToString());
            posX = (x - FP._1) * FP.FromFloat_UNSAFE(3.5f);
            posY = (y - FP._1) * FP.FromFloat_UNSAFE(3.5f);
            //Debug.Log("Position Click!" + posX.ToString() + ", " + posY.ToString());
            Click?.Invoke(x, y, posX, posY, posArray);
        }
        public FP GetX()
        {
            return x;
        }
        public FP GetY()
        {
            return y;
        }
    }
}
