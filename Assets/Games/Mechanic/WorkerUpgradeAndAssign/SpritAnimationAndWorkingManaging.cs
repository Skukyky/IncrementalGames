using UnityEngine;
using UnityEngine.UI;

public class AnimatedSprite : MonoBehaviour
{
    [SerializeField] private Image spriteRenderer;
    [SerializeField] private GameManager gameManager;
    
    private Sprite sprite1;
    private Sprite sprite2;
    private double frameTime = 0.5f;
    private double timer;
    private bool frame;
    private ItemData itemDataLocal;
    
    public void Initialize(ItemData itemData, GameManager gameManager)
    {
        if (itemData == null||gameManager == null) return;
        itemDataLocal = itemData;
        this.gameManager = gameManager;
        print("Automatic Click");
        SetSprites(itemData.automaticWorkerCharacterData.sprite1, itemData.automaticWorkerCharacterData.sprite2);
        spriteRenderer.sprite = sprite1;
        frameTime = itemData.automaticWorkerCharacterData.FireRate;
    }

    public void ModifyCharacterData(CharacterData characterData)
    {
        itemDataLocal.automaticWorkerCharacterData = characterData;
        SetSprites(characterData.sprite1, characterData.sprite2);
        frameTime = characterData.FireRate;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameTime)
        {
            timer = 0f;
            frame = !frame;
            if (frame)
            {
                AutomaticClick();
            }
            spriteRenderer.sprite = frame ? sprite2 : sprite1;
        }
    }

    public void SetSprites(Sprite newSprite1, Sprite newSprite2)
    {
        sprite1 = newSprite1;
        sprite2 = newSprite2;
    }
    
    private void AutomaticClick()
    {
        if (Random.value * 100 < itemDataLocal.automaticWorkerCharacterData.CritRate)
        {
            gameManager.coins += (int)(itemDataLocal.baseDamageUpgrade * itemDataLocal.automaticWorkerCharacterData.CritDamage);
            gameManager.PlaySlimeEffect();
        }
        else
        {
            gameManager.coins += itemDataLocal.baseDamageUpgrade;
        }
        gameManager.CalculateNewMoney();
    }
}