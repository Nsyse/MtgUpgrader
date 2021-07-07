using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[ExcludeFromCodeCoverage]
public class StorageTests
{
    private const string STORAGE_NAME = "TestStorage";
    private string LBOLT = "Lightning Bolt";
    private string SHOCK = "Shock";
    private Storage _storage;
    private string TEST_FOLDER = Application.persistentDataPath + @"/testStorage";
    private Card _bolt;
    private Card _shock;

    [SetUp]
    public void CreateCleanCards()
    {
        _storage = new Storage();
        _bolt = AddLightningBolt();
        _shock = AddShock();
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
    public void ClearedStorageIsEmpty()
    {
        _storage.Clear();
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
        _storage.Clear();
        AddLightningBolt();
        _storage.RemoveCardById(LBOLT);
        Assert.IsTrue(_storage.IsEmpty());
    }

    [Test]
    public void RemovingOneOfTwoCardKeepsStorageNotEmpty()
    {
        AddLightningBolt();
        AddShock();
        _storage.RemoveCardById(LBOLT);
        Assert.IsFalse(_storage.IsEmpty());
    }
    
    [Test]
    public void CantAddAlreadyExistingCard()
    {
        AddLightningBolt();
        Assert.AreEqual(2, _storage.Count);
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
        _storage.Load("MissingPath","MissingFile");
    }
    
    [Test]
    public void StorageCanSaveAndLoadCard()
    {
        _bolt = AddLightningBolt();
        Assert.IsTrue(_storage.Contains(_bolt));
        SaveStorage();
        Assert.IsTrue(_storage.Contains(_bolt));
        
        _storage = new Storage();
        LoadStorage();
        Assert.IsTrue(_storage.Count == 2);
        Assert.IsTrue(_storage.Contains(_bolt));
    }
    
    [Test]
    public void StorageCanRestoreUpgradeRelationships()
    {
        AddShockToBolt();
        CheckContainsBoltChain();
        SaveStorage();
        CheckContainsBoltChain();
        
        _storage = new Storage();
        LoadStorage();
        CheckContainsBoltChain();
    }

    private void AddShockToBolt()
    {
        _shock.AddSingleUpgrade(_bolt);
    }

    private void CheckContainsBoltChain()
    {
        var bolt = _storage.FindCardById(LBOLT);
        var shock = _storage.FindCardById(SHOCK);
        Assert.IsTrue(_storage.Contains(bolt));
        Assert.IsTrue(_storage.Contains(shock));
        Assert.IsTrue(shock.IsWorseThan(bolt));
    }

    private void SaveStorage()
    {
        DirectoryInfo di = Directory.CreateDirectory(TEST_FOLDER);
        _storage.Save(TEST_FOLDER, STORAGE_NAME);
    }
    
    private void LoadStorage()
    {
        _storage.Load(TEST_FOLDER, STORAGE_NAME);
    }
}