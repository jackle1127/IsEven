using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jack.IsEven
{
    public class BallGenerator : MonoBehaviour
    {
        [SerializeField] private float _ballDistance = .2f;
        [SerializeField] private GameObject _ballPrefab;
        [SerializeField] private float _interval = .2f;
        public static BallGenerator Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void GenerateBalls(int numberOfBalls, Action onGenerationFinish)
        {
            StartCoroutine(GenerateBallsCoroutine(numberOfBalls, onGenerationFinish));
        }

        private IEnumerator GenerateBallsCoroutine(int numberOfBalls, Action onGenerationFinish)
        {
            Vector3 currentPosition = transform.position;
            for (int i = 1; i <= numberOfBalls; i++)
            {
                GameObject newBall = Instantiate(_ballPrefab, transform);
                newBall.transform.position = currentPosition;
                currentPosition += transform.forward * _ballDistance;

                yield return new WaitForSecondsRealtime(_interval);
            }

            onGenerationFinish?.Invoke();
        }
    }
}