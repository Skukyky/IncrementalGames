using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Mineur automatique : alterne deux sprites et frappe le caillou à chaque coup de pioche.
/// La cadence vient de FireRate, les coups critiques de CritRate / CritDamage du personnage assigné.
/// </summary>
public class AutomaticWorker : MonoBehaviour
{
    [SerializeField] private Image spriteRenderer;

    private ClickTarget _target;
    private Sprite _sprite1;
    private Sprite _sprite2;
    private double _frameTime = 0.5d;
    private double _timer;
    private bool _frame;

    /// <summary>Données de l'amélioration (partagées avec UpgradeItemUI : leurs niveaux s'appliquent ici).</summary>
    public ItemData ItemData { get; private set; }

    public void Initialize(ItemData itemData, ClickTarget target)
    {
        if (itemData == null || target == null) return;
        ItemData = itemData;
        _target = target;

        CharacterData character = ItemData.automaticWorkerCharacterData;
        if (character != null)
        {
            _sprite1 = character.sprite1;
            _sprite2 = character.sprite2;
            _frameTime = character.FireRate;
        }
        spriteRenderer.sprite = _sprite1;
    }

    public void ModifyCharacterData(CharacterData characterData)
    {
        if (ItemData == null || characterData == null)
            return;

        ItemData.automaticWorkerCharacterData = characterData.Clone();
        _sprite1 = characterData.sprite1;
        _sprite2 = characterData.sprite2;
        _frameTime = characterData.FireRate;
        _timer = 0d;
        _frame = false;
        spriteRenderer.sprite = _sprite1;
    }

    /// <summary>Gain moyen par seconde, critiques compris (sert au gain hors-ligne).</summary>
    public BigNumber EstimateIncomePerSecond()
    {
        if (ItemData == null || _frameTime <= 0d)
            return BigNumber.Zero;

        // Un coup toutes les deux images d'animation.
        double hitsPerSecond = 1d / (2d * _frameTime);
        double critMultiplier = 1d;
        CharacterData character = ItemData.automaticWorkerCharacterData;
        if (character != null)
        {
            double critChance = Mathf.Clamp01((float)(character.CritRate / 100d));
            critMultiplier = 1d + critChance * (character.CritDamage - 1d);
        }
        return ItemData.baseDamageUpgrade * (hitsPerSecond * critMultiplier);
    }

    private void Update()
    {
        if (_target == null) return;

        _timer += Time.deltaTime;
        if (_timer < _frameTime) return;

        _timer = 0d;
        _frame = !_frame;
        if (_frame)
            Strike();
        spriteRenderer.sprite = _frame ? _sprite2 : _sprite1;
    }

    private void Strike()
    {
        Vector3 popupPosition = transform.position + new Vector3(0f, 100f, 100f);
        CharacterData character = ItemData.automaticWorkerCharacterData;
        bool isCrit = character != null && Random.value * 100 <= character.CritRate;

        if (isCrit)
            _target.Hit(ItemData.baseDamageUpgrade * character.CritDamage, popupPosition, playSquash: true);
        else
            _target.Hit(ItemData.baseDamageUpgrade, popupPosition, playSquash: false);
    }
}
