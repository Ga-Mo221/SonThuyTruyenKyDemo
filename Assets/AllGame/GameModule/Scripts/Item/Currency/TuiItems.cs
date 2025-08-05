using System.Collections.Generic;
using UnityEngine;

public class TuiItems : MonoBehaviour
{
    [SerializeField] private GameObject F;
    [SerializeField] Animator _anim;
    private Chest _chest;
    private bool _canOpen = false;

    void Start()
    {
        if (_chest == null)
        {
            Debug.LogError("Hãy gắn Chest vào TuiItems");
        }
    }

    void Update()
    {
        openTui();
    }


    private void openTui()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canOpen)
        {
            if (_chest._rtItems.Count > 0)
            {
                _anim.SetTrigger("open");
                _chest.displayItems();
                _chest._itemsDropParent.parent.parent.parent.gameObject.SetActive(true);
                PlayerManager.Instance._knocked = true;
            }
        }
    }

    public void setTuiItems(Chest chest)
    {
        _chest = chest;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _canOpen = true;
            F.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _canOpen = false;
        F.SetActive(false);
    }
}
