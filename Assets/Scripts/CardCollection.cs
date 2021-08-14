using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardCollection : ICardCollection
{
    [SerializeField]
    protected List<Card> _cards = new List<Card>();
    public bool Remove(Card item)
    {
        return RemoveCardById(item.Id);
    }

    public int Count => _cards.Count;
    public bool IsReadOnly { get; }

    public bool IsEmpty()
    {
        return Count == 0;
    }

    public void Add(Card card)
    {
        AddCardIfNew(card);
    }

    public void Clear() => _cards.Clear();

    public bool Contains(Card card)
    {
        return _cards.Any(x => x.MatchesId(card));
    }

    public void CopyTo(Card[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    private bool RemoveCardById(string id)
    {
        int qtyRemoved = _cards.RemoveAll(x => x.MatchesId(id));
        return qtyRemoved > 0;
    }

    #region private functions
    private void AddCardIfNew(Card card)
    {
        if (CardIsNew(card))
            AddNewCard(card);
    }
    
    private void AddNewCard(Card card)
    {
        _cards.Add(card);
    }

    private bool CardIsNew(Card card)
    {
        return !Contains(card);
    }
    #endregion

    public IEnumerator<Card> GetEnumerator()
    {
        return _cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Card FindCardById(string id)
    {
        return _cards.FirstOrDefault(x => x.MatchesId(id));
    }
}