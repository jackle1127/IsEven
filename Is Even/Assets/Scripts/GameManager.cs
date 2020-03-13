using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Jack.IsEven
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputField _numberToCheckField;
        [SerializeField] private GameObject _firstPanel;
        [SerializeField] private GameObject _secondPanel;
        [SerializeField] private Text _numberToShow;
        [SerializeField] private GameObject _whatWeCareAbout;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Text _numberOnSpawner;
        [SerializeField] private Animator _spawnerAnimator;
        [SerializeField] private BallGenerator _ballGenerator;
        [SerializeField] private Animator _screwAnimator;
        [SerializeField] private GameObject _resultArrow;
        [SerializeField] private GameObject _thirdPanel;
        private string _fullNumber;
        private int _numberToCheck;

        public void StartSequence()
        {
            if (_numberToCheckField.text != "")
            {
                _fullNumber = _numberToCheckField.text;
                _numberToCheck = int.Parse(_fullNumber.Substring(_fullNumber.Length - 1));
                _numberOnSpawner.text = _numberToCheck + "";
                StartCoroutine(TheWholeDamnSequence());
            }
        }

        public void StartOver()
        {
            SceneManager.LoadScene(0);
        }

        private IEnumerator TheWholeDamnSequence()
        {
            _firstPanel.SetActive(false);

            yield return new WaitForSeconds(.2f);

            // Second panel
            _secondPanel.SetActive(true);
            for (int i = 0; i < _fullNumber.Length; i++)
            {
                _numberToShow.text += _fullNumber[i];
                yield return new WaitForSeconds(.3f);
            }
            _whatWeCareAbout.SetActive(true);

            yield return new WaitForSeconds(3);
            _secondPanel.SetActive(false);

            // Player start
            _playerController.enabled = true;
            yield return new WaitForSeconds(.4f);

            // Start ball spawning machine
            _spawnerAnimator.SetTrigger("Start");

            while (!_spawnerAnimator.GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                yield return null;
            }

            yield return new WaitForSeconds(.4f);

            _ballGenerator.GenerateBalls(_numberToCheck);

            while (_ballGenerator.Generating)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1);
            _screwAnimator.SetTrigger("Start");

            // Wait for the last ball to die
            while (FindObjectOfType<BallIdentifier>() != null)
            {
                yield return null;
            }

            _resultArrow.SetActive(true);

            yield return new WaitForSeconds(3);

            _thirdPanel.SetActive(true);
        }
    }
}