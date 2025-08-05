using System.Collections;
using UnityEngine;


public class ItemPickup : MonoBehaviour
{
    public Item item;

    private bool _canAttack;
    private SpriteRenderer _img;
    private bool _isPickedUp = false;

    void Awake()
    {
        _img = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _canAttack = PlayerManager.Instance.Stats._tutorialAttack;
    }
    
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero); // direction = Vector2.zero để kiểm tra điểm

        if (hit.collider != null && hit.transform == transform)
        {
            if (_canAttack)
            {
                PlayerManager.Instance.Stats._tutorialAttack = false;
            }
        }
        else
        {
            PlayerManager.Instance.Stats._tutorialAttack = _canAttack;
        }
    }

    private void Pickup()
    {
        if (item == null)
        {
            Debug.LogWarning("không có item nào được gắn");
            return;
        }
        _isPickedUp = true;
        InventoryManager.Instance.AddItemInList(item);
        StartCoroutine(setCanAttack());
    }

    private IEnumerator setCanAttack()
    {
        _img.enabled = false;
        yield return new WaitForSeconds(0.2f);
        PlayerManager.Instance.Stats._tutorialAttack = _canAttack;
        Destroy(gameObject);
    }

    void OnMouseDown()
    {
        if (_isPickedUp) return;
        Debug.Log("Đã nhặt " + item.name);
        Pickup();
    }
}
