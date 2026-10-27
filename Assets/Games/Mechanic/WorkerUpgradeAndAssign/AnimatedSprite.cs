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
    public new ItemData itemDataLocal;
    
    public void Initialize(ItemData itemData, GameManager gameManager)
    {
        if (itemData == null || gameManager == null) return;
        itemDataLocal = itemData;
        this.gameManager = gameManager;
        if (itemDataLocal.automaticWorkerCharacterData != null)
        {
            SetSprites(itemDataLocal.automaticWorkerCharacterData.sprite1, itemDataLocal.automaticWorkerCharacterData.sprite2);
            frameTime = itemDataLocal.automaticWorkerCharacterData.FireRate;
        }
        spriteRenderer.sprite = sprite1;
    }

    public void ModifyCharacterData(CharacterData characterData)
    {
        if (itemDataLocal == null || characterData == null)
            return;

        itemDataLocal.automaticWorkerCharacterData = characterData.Clone();
        SetSprites(characterData.sprite1, characterData.sprite2);
        frameTime = characterData.FireRate;
        timer = 0f;
        frame = false;
        spriteRenderer.sprite = sprite1;
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
        Vector3 spawnPosition = transform.position;
        double money = 0d;
        if (Random.value * 100 < itemDataLocal.automaticWorkerCharacterData.CritRate)
        {
            money = itemDataLocal.baseDamageUpgrade * itemDataLocal.automaticWorkerCharacterData.CritDamage;
            gameManager.AddCoins(money, transform.position + new Vector3(0f, 100f, 100f));
            gameManager.PlaySlimeEffect();
        }
        else
        {
            money = itemDataLocal.baseDamageUpgrade;
            gameManager.AddCoins(money, transform.position + new Vector3(0f, 100f, 100f));
        }
    }
}