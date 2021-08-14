using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Storage : CardCollection
{
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
            var upgrade = FindCardById(manualUpgrade);
            card.AddSingleUpgrade(upgrade);
        }
    }

    #endregion
}