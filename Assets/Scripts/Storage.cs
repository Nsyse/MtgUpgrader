using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Storage : ICardCollection
{
    [SerializeField]
    private CardCollection _cards = new CardCollection();

    public void Save(string path, string storageName)
    {
        var jsonStorage = JsonUtility.ToJson(this);
        string filePath = System.IO.Path.Combine(path, $"{storageName}.json");
        SaveStorageJsonAtPath(filePath, jsonStorage);
    }

    private void SaveStorageJsonAtPath(string path, string jsonStorage)
    {
        File.WriteAllText( path, jsonStorage);
    }

    public void Load(string path, string storageName)
    {
        try
        {
            TryLoadCards(path + $"/{storageName}.json");
        }
        catch (DirectoryNotFoundException e)
        {
            LogCantFindStorageFile();
        }
        
    }

    #region ICardCollection

    public bool Remove(Card item)
    {
        throw new NotImplementedException();
    }

    public int Count => _cards.Count;
    public bool IsReadOnly { get; }
    public bool IsEmpty() => _cards.IsEmpty();
    public void RemoveCardById(string id) => _cards.RemoveCardById(id);
    public void Add(Card item)
    {
        throw new NotImplementedException();
    }

    public void Clear() => _cards.Clear();

    public bool Contains(Card card) => _cards.Contains(card);
    public void CopyTo(Card[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public void AddCard(Card newCard) => _cards.Add(newCard);
    
    public IEnumerator<Card> GetEnumerator() => _cards.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
    
    #region private
    private void LogCantFindStorageFile()
    {
        Debug.LogError("Provided storage file does not exist.");
    }

    private void TryLoadCards(string filePath)
    {
        var jsonRead = File.ReadAllText(filePath);
        _cards = JsonUtility.FromJson<Storage>(jsonRead)._cards;
        foreach (var card in _cards)
        {
            RebuildRelationships(card);
        }
    }

    private void RebuildRelationships(Card card)
    {
        foreach (string manualUpgrade in card.ManuallyRegisteredBetterCardsIds)
        {
            var upgrade = _cards.FindCardById(manualUpgrade);
            card.AddSingleUpgrade(upgrade);
        }
    }

    #endregion

    public Card FindCardById(string id)
    {
        return _cards.FindCardById(id);
    }
}