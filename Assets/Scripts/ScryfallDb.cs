using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ScryfallDb
{
    public Card FindWithFullName(string name)
    {
        string json = File.ReadAllText("Assets/ScryfallDB/Scryfall.json");
        var parsedScryfall = JsonUtility.FromJson<ScryfallArray>("{\"Cards\":" + json + "}");
        var foundCard = parsedScryfall.Cards.FirstOrDefault(x => x.name == name);
        return new Card(foundCard);
    }
}

[Serializable]
public class ScryfallArray
{
    [SerializeField] public ScryfallCard[] Cards;
}

[Serializable]
public class ScryfallCard
{
    [SerializeField] public string name;

    [SerializeField] public string id;
    [SerializeField] private ScryfallImageUris image_uris;

    public string ImageLink => image_uris.normal;
}

[Serializable]
public class ScryfallImageUris
{
    [SerializeField] public string normal;
}