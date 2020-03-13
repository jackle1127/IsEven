using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jack.IsEven
{
    public class BallGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _ballPrefab;
        [SerializeField] private float _interval = .2f;
        public static BallGenerator Instance { get; private set; }
        public bool Generating { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void GenerateBalls(int numberOfBalls)
        {
            StartCoroutine(GenerateBallsCoroutine(numberOfBalls));
        }

        private IEnumerator GenerateBallsCoroutine(int numberOfBalls)
        {
            Generating = true;
            for (int i = 1; i <= numberOfBalls; i++)
            {
                GameObject newBall = Instantiate(_ballPrefab, transform);
                newBall.transform.position = transform.position;

                yield return new WaitForSecondsRealtime(_interval);
            }
            Generating = false;
        }
    }
}