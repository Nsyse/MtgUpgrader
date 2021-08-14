using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Card
{
    private class ModificationWouldCreateLoopException : Exception
    {
    }

    [SerializeField] public List<string> ManuallyRegisteredBetterCardsIds = new List<string>();

    [SerializeField] private string _id;
    public string Id
    {
        get => _id;
        private set => _id = value;
    }

    private ICardCollection _manuallyRegisteredBetterCards = new CardCollection();
    private ICardCollection _immediatelyBetterCards = new CardCollection();
    private ICardCollection _immediatelyWorseCards = new CardCollection();

    public Card()
    {
        _manuallyRegisteredBetterCards = new CardCollection();
        _immediatelyBetterCards = new CardCollection();
        _immediatelyWorseCards = new CardCollection();
    }

    public Card(string id)
    {
        Id = id;
    }

    public bool IsOutclassed()
    {
        return !_immediatelyBetterCards.IsEmpty();
    }

    public void AddSingleUpgrade(Card upgrade)
    {
        try
        {
            TryAddSingleUpgrade(upgrade);
        }
        catch (ModificationWouldCreateLoopException e)
        {
            Debug.LogError("Can't add upgrade, would create a cycle.");
        }
    }

    public void RemoveSingleUpgrade(Card upgrade)
    {
        RemoveManualUpgrade(upgrade);
        RemoveImmediateUpgrade(upgrade);
        RecursivelyUnsilenceRelevantManualUpgrades();
    }

    public bool MatchesId(string id)
    {
        return Id.Equals(id);
    }

    public bool IsOrphaned()
    {
        return _immediatelyBetterCards.IsEmpty() && _immediatelyWorseCards.IsEmpty();
    }

    public bool IsImmediatelyWorseThan(Card otherCard)
    {
        return _immediatelyBetterCards.Contains(otherCard);
    }

    public bool IsWorseThan(Card otherCard)
    {
        if (IsImmediatelyWorseThan(otherCard))
            return true;
        return _immediatelyBetterCards.Any(x => x.IsWorseThan(otherCard));
    }

    public bool IsBetterThan(Card otherCard)
    {
        if (IsImmediatelyBetterThan(otherCard))
            return true;
        return _immediatelyWorseCards.Any(x => x.IsBetterThan(otherCard));
    }

    public bool IsRelated(Card otherCard, HashSet<Card> testedCards = null)
    {
        var immediatelyRelatedCards = RetrieveAlLImmediatelyRelatedCards();
        if (SetContainsCard(immediatelyRelatedCards, otherCard))
            return true;

        testedCards = LazyInitializeTestedCards(testedCards);

        testedCards.Add(this);

        foreach (var relatedCard in immediatelyRelatedCards)
            if (!testedCards.Any(x => x.MatchesId(relatedCard)))
                if (relatedCard.IsRelated(otherCard, testedCards))
                    return true;
        return false;
    }

    private void RecursivelyUnsilenceRelevantManualUpgrades()
    {
        foreach (var manualUpgrade in _manuallyRegisteredBetterCards)
            if (!IsWorseThan(manualUpgrade))
                AddSingleUpgrade(manualUpgrade);

        foreach (var card in _immediatelyWorseCards)
            card.RecursivelyUnsilenceRelevantManualUpgrades();
    }

    private void RemoveManualUpgrade(Card upgrade)
    {
        ManuallyRegisteredBetterCardsIds.Remove(upgrade.Id);
        _manuallyRegisteredBetterCards.Remove(upgrade);
    }

    private void RemoveImmediateUpgrade(Card upgrade)
    {
        _immediatelyBetterCards.Remove(upgrade);
    }

    private void TryAddSingleUpgrade(Card upgrade)
    {
        if (AddingUpgradeWouldCreateLoop(upgrade))
            throw new ModificationWouldCreateLoopException();

        RecursivelyRemoveRedundantUpgrades(upgrade);
        AddSingleUpgradeIfNew(upgrade);
        upgrade.AddDowngrade(this);
    }

    private void RecursivelyRemoveRedundantUpgrades(Card upgrade)
    {
        RecursivelyRemoveImmediateRedundantUpgrades(upgrade);
        foreach (var card in _immediatelyWorseCards)
            card.RecursivelyRemoveRedundantUpgrades(upgrade);
    }

    private void RecursivelyRemoveImmediateRedundantUpgrades(Card upgrade)
    {
        RemoveImmediateUpgrade(upgrade);
        foreach (var card in upgrade._immediatelyBetterCards)
            RecursivelyRemoveImmediateRedundantUpgrades(card);
    }

    private bool AddingUpgradeWouldCreateLoop(Card upgrade)
    {
        return UpgradeIsDirectlyInferior(upgrade) || UpgradeIsIndirectlyInferior(upgrade);
    }

    private bool UpgradeIsDirectlyInferior(Card upgrade)
    {
        return _immediatelyWorseCards.Any(x => x.MatchesId(upgrade));
    }

    private bool UpgradeIsIndirectlyInferior(Card upgrade)
    {
        return _immediatelyWorseCards.Any(x => x.AddingUpgradeWouldCreateLoop(upgrade));
    }

    private bool IsImmediatelyBetterThan(Card otherCard)
    {
        return _immediatelyWorseCards.Contains(otherCard);
    }

    private HashSet<Card> LazyInitializeTestedCards(HashSet<Card> testedCards)
    {
        var currentValue = testedCards ?? new HashSet<Card>();
        return currentValue;
    }

    private bool SetContainsCard(HashSet<Card> list, Card otherCard)
    {
        return list.Any(x => x.MatchesId(otherCard));
    }

    private HashSet<Card> RetrieveAlLImmediatelyRelatedCards()
    {
        var set = new HashSet<Card>();
        set.AddNewElementsFrom(_immediatelyWorseCards);
        set.AddNewElementsFrom(_immediatelyBetterCards);
        return set;
    }

    private void AddSingleUpgradeIfNew(Card upgrade)
    {
        if (!AlreadyHasUpgradeRelationship(upgrade))
            RegisterUpgrade(upgrade);
    }

    private void RegisterUpgrade(Card upgrade)
    {
        if (!ManuallyRegisteredBetterCardsIds.Contains(upgrade.Id))
        {
            ManuallyRegisteredBetterCardsIds.Add(upgrade.Id);
        }
        _manuallyRegisteredBetterCards.Add(upgrade);
        _immediatelyBetterCards.Add(upgrade);
    }


    private void AddDowngrade(Card otherCard)
    {
        _immediatelyWorseCards.Add(otherCard);
    }

    private bool AlreadyHasUpgradeRelationship(Card otherCard)
    {
        return _immediatelyBetterCards.Contains(otherCard);
    }

    public bool MatchesId(Card otherCard)
    {
        return MatchesId(otherCard.Id);
    }
}