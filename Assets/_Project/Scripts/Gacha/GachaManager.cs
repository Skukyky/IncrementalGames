using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gacha : dépense des gemmes (gagnées à chaque rebirth) pour invoquer un personnage tiré au
/// hasard selon son dropWeight. Le personnage, même en doublon, rejoint la réserve assignable.
/// Les gemmes ne sont pas remises à zéro par le rebirth.
/// </summary>
public class GachaManager : MonoBehaviour
{
    [Tooltip("Personnages invocables et leurs poids de tirage (CharacterData.dropWeight).")]
    [SerializeField] private DataMinerList summonPool;
    [SerializeField] private int summonCost = 1;
    [SerializeField] private WorkerAssignmentManager assignments;
    [SerializeField] private GachaReveal reveal;

    [Header("UI de l'onglet Gacha")]
    [SerializeField] private Button summonButton;
    [SerializeField] private TMP_Text summonPriceText;

    public event Action<int> GemsChanged;

    public int Gems { get; private set; }
    public int SummonCost => summonCost;

    private void Start()
    {
        if (summonButton != null)
            summonButton.onClick.AddListener(Summon);
        RefreshUi();
    }

    public void AddGems(int amount)
    {
        if (amount <= 0) return;
        SetGems(Gems + amount);
    }

    public void SetGems(int amount)
    {
        Gems = Math.Max(0, amount);
        GemsChanged?.Invoke(Gems);
        RefreshUi();
    }

    public bool CanSummon => Gems >= summonCost && (reveal == null || !reveal.IsPlaying);

    /// <summary>Branché sur le bouton d'invocation.</summary>
    public void Summon()
    {
        if (!CanSummon)
            return;

        CharacterData rolled = Roll(out float chance);
        if (rolled == null)
            return;

        bool isNew = !assignments.Owns(rolled.CharacterName);
        SetGems(Gems - summonCost);

        // Ajouté tout de suite : une sauvegarde pendant l'animation ne le perd pas.
        assignments.ReturnToPool(rolled.Clone());

        if (reveal != null)
            reveal.Play(rolled, chance, isNew, RefreshUi);
    }

    /// <summary>Tirage pondéré : chaque personnage a poids / somme des poids de chances.</summary>
    private CharacterData Roll(out float chance)
    {
        chance = 0f;
        List<CharacterData> candidates = summonPool != null ? summonPool.MinerList : null;
        if (candidates == null)
            return null;

        float total = 0f;
        foreach (CharacterData character in candidates)
        {
            if (character != null)
                total += Mathf.Max(0f, character.dropWeight);
        }
        if (total <= 0f)
            return null;

        float roll = UnityEngine.Random.value * total;
        CharacterData last = null;
        foreach (CharacterData character in candidates)
        {
            if (character == null || character.dropWeight <= 0f)
                continue;

            last = character;
            if (roll < character.dropWeight)
            {
                chance = character.dropWeight / total;
                return character;
            }
            roll -= character.dropWeight;
        }

        chance = last != null ? last.dropWeight / total : 0f;
        return last;
    }

    private void RefreshUi()
    {
        if (summonPriceText != null)
            summonPriceText.text = summonCost + (summonCost > 1 ? " gemmes" : " gemme");
        if (summonButton != null)
            summonButton.interactable = CanSummon;
    }
}
