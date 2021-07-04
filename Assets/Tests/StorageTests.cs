using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[ExcludeFromCodeCoverage]
public class StorageTests
{
    private string LBOLT = "Lightning Bolt";
    private string SHOCK = "Shock";
    private Storage _storage;
    private string TEST_FOLDER = "testStorage";

    [SetUp]
    public void CreateCleanCards()
    {
        _storage = new Storage();
    }
    
    private Card AddCard(string id)
    {
        var newCard = new Card(id);
        _storage.AddCard(newCard);
        return newCard;
    }
    
    private Card AddShock()
    {
        return AddCard(SHOCK);
    }
    
    private Card AddLightningBolt()
    {
        return AddCard(LBOLT);
    }
    
    [Test]
    public void NewStorageIsEmpty()
    {
        Assert.IsTrue(_storage.IsEmpty());
    }
    
    [Test]
    public void AddingCardToStorageNoLongerEmpty()
    {
        AddLightningBolt();
        Assert.IsFalse(_storage.IsEmpty());
    }

    [Test]
    public void RemovingAddedCardRemakesStorageEmpty()
    {
        AddLightningBolt();
        _storage.RemoveCard(LBOLT);
        Assert.IsTrue(_storage.IsEmpty());
    }
    
    [Test]
    public void RemovingOneOfTwoCardKeepsStorageNotEmpty()
    {
        AddLightningBolt();
        AddShock();
        _storage.RemoveCard(LBOLT);
        Assert.IsFalse(_storage.IsEmpty());
    }
    
    [Test]
    public void CantAddAlreadyExistingCard()
    {
        AddLightningBolt();
        AddLightningBolt();
        Assert.AreEqual(1, _storage.Count);
    }

    [Test]
    public void StorageDoesntContainMissingCard()
    {
        var missingCard = new Card("missing");
        Assert.IsFalse(_storage.Contains(missingCard));
    }
    
    [Test]
    public void LoadingMissingFileFails()
    {
        LogAssert.Expect(LogType.Error, "Provided storage file does not exist.");
        _storage.Load("MissingFile");
    }
    
    [Test]
    public void StorageCanSaveAndLoadCard()
    {
        var bolt = AddLightningBolt();
        Assert.IsTrue(_storage.Contains(bolt));
        SaveStorage();
        Assert.IsTrue(_storage.Contains(bolt));
        
        _storage = new Storage();
        LoadStorage();
        Assert.IsTrue(_storage.Contains(bolt));
    }

    private void SaveStorage()
    {
        _storage.Save(TEST_FOLDER);
    }
    
    private void LoadStorage()
    {
        _storage.Load(TEST_FOLDER);
    }
}