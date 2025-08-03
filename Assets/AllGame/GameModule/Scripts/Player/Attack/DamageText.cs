using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private GameObject _physicDamage;
    [SerializeField] private GameObject _magicDamage;

    private RectTransform _textTransform;

    private TextMeshProUGUI _damageText;
    private Vector3 _moveSpeed = new Vector3(0, 5, 0);
    private float _timeToFace = 1f;

    private float _timeElapsed = 0f;
    private Color _startColor;

    private float _damage;

    void Awake()
    {
        _damageText = GetComponent<TextMeshProUGUI>();
        _textTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (_startColor != null)
            _damageText.color = _startColor;
        else
            Debug.LogError("Chưa gắn startColor");
        _damageText.text = "-" + _damage.ToString("F0");
    }

    void Update()
    {
        animText();
    }

    private void animText()
    {
        _textTransform.position += _moveSpeed * Time.deltaTime;
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed < _timeToFace)
        {
            float _faceAlpha = _startColor.a * (1 - (_timeElapsed / _timeToFace));
            _damageText.color = new Color(_startColor.r, _startColor.g, _startColor.b, _faceAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void setDamage(float damage, bool magic)
    {
        _damage = damage;
        if (!magic)
        {
            _startColor = new Color(217f / 255f, 7f / 255f, 7f / 255f, 255f / 255f); // đỏ
            _physicDamage.SetActive(true);
        }
        else
        {
            _startColor = new Color(197f / 255f, 8f / 255f, 231f / 255f, 255f / 255f); // tím
            _magicDamage.SetActive(true);
        }
    }
}
