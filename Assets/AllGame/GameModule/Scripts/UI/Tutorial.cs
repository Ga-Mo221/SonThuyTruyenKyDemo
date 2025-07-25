using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialText;
    private bool _display = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_display)
        {
            _tutorialText.SetActive(true);
            StartCoroutine(offText());
            switch (_tutorialText.name)
            {
                case "Jump":
                    PlayerManager.Instance.Stats._tutorialJump = true;
                    break;
                case "Sit":
                    PlayerManager.Instance.Stats._tutorialSit = true;
                    break;
                case "Run":
                    PlayerManager.Instance.Stats._tutorialRun = true;
                    break;
            }
        }
    }

    private IEnumerator offText()
    {
        yield return new WaitForSeconds(2f);
        _tutorialText.SetActive(false);
        Destroy(gameObject);
    }
}
