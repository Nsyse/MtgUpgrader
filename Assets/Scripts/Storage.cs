using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Storage
{
    private HashSet<Card> _cards = new HashSet<Card>();
    private bool _isEmpty = true;
    public int Count => _cards.Count;

    public bool IsEmpty()
    {
        return _cards.IsEmpty();
    }

    public void AddCard(Card card)
    {
        AddCardIfNew(card);
    }

    public void RemoveCard(string id)
    {
        _cards.RemoveWhere(x => x.MatchesId(id));
    }

    public void Save(string storageName)
    {
        throw new NotImplementedException();
    }

    public void Load(string storageName)
    {
        throw new NotImplementedException();
    }
    
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

    public bool Contains(Card card)
    {
        return _cards.Any(x=> x.MatchesId(card));
    }
}
