using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[ExcludeFromCodeCoverage]
public class CardRelationTests
{
    private Card _shock;
    private Card _lightningBolt;
    private Card _breathOfFire;

    [SetUp]
    public void CreateCleanCards()
    {
        _breathOfFire = new Card("Breath of Fire");
        _shock = new Card("Shock");
        _lightningBolt = new Card("Lightning Bolt");
    }

    public void CreateShockToBolt()
    {
        _shock.AddSingleUpgrade(_lightningBolt);
    }
    
    public void CreateBreathToBoltChain()
    {
        _breathOfFire.AddSingleUpgrade(_shock);
        _shock.AddSingleUpgrade(_lightningBolt);
    }

    [Test]
    public void AddingRelationshipMakesBothCardsNotOrphaned()
    {
        CreateShockToBolt();
        
        Assert.IsFalse(_shock.IsOrphaned());
        Assert.IsFalse(_lightningBolt.IsOrphaned());
    }
    
    [Test]
    public void UpgradeRelationshipIsMutual()
    {
        CreateShockToBolt();
        
        Assert.IsTrue(_shock.IsWorseThan(_lightningBolt));
        Assert.IsFalse(_lightningBolt.IsWorseThan(_shock));
        
        Assert.IsTrue(_lightningBolt.IsBetterThan(_shock));
        Assert.IsFalse(_shock.IsBetterThan(_lightningBolt));
    }

    [Test]
    public void OneCardCanHaveManyUpgrades()
    {
        var cardWithTwoUpgrades = new Card("upgradable");
        var upgrade1 = new Card("upgrade1");
        var upgrade2 = new Card("upgrade2");
        
        cardWithTwoUpgrades.AddSingleUpgrade(upgrade1);
        cardWithTwoUpgrades.AddSingleUpgrade(upgrade2);
        Assert.True(cardWithTwoUpgrades.IsWorseThan(upgrade1));
        Assert.True(cardWithTwoUpgrades.IsWorseThan(upgrade2));
        
        Assert.True(upgrade1.IsBetterThan(cardWithTwoUpgrades));
        Assert.True(upgrade2.IsBetterThan(cardWithTwoUpgrades));
    }
    
    [Test]
    public void OneCardCanHaveManyDowngrades()
    {
        var cardWithTwoUpgrades = new Card("best");
        var downgrade1 = new Card("downgrade1");
        var downgrade2 = new Card("downgrade2");
        
        downgrade1.AddSingleUpgrade(cardWithTwoUpgrades);
        downgrade2.AddSingleUpgrade(cardWithTwoUpgrades);
        Assert.True(downgrade1.IsWorseThan(cardWithTwoUpgrades));
        Assert.True(downgrade2.IsWorseThan(cardWithTwoUpgrades));
        
        Assert.True(cardWithTwoUpgrades.IsBetterThan(downgrade1));
        Assert.True(cardWithTwoUpgrades.IsBetterThan(downgrade1));
    }


    [Test]
    public void UpgradeRelationshipIsRecursive()
    {
        CreateBreathToBoltChain();
        Assert.IsTrue(_breathOfFire.IsWorseThan(_lightningBolt));
        Assert.IsTrue(_breathOfFire.IsWorseThan(_shock));
        
        Assert.IsTrue(_lightningBolt.IsBetterThan(_shock));
        Assert.IsTrue(_lightningBolt.IsBetterThan(_breathOfFire));
    }
    
    [Test]
    public void UpgradeRelationshipIsRetroactive()
    {
        _shock.AddSingleUpgrade(_lightningBolt);
        _breathOfFire.AddSingleUpgrade(_shock);
        Assert.IsTrue(_breathOfFire.IsWorseThan(_lightningBolt));
        Assert.IsTrue(_breathOfFire.IsWorseThan(_shock));
    }
    
    [Test]
    public void CardWithUpgradesIsOutclassed()
    {
        CreateBreathToBoltChain();
        Assert.IsTrue(_breathOfFire.IsOutclassed());
    }
    
    [Test]
    public void BestOfChainIsNotOutclassed()
    {
        CreateBreathToBoltChain();
        Assert.IsFalse(_lightningBolt.IsOutclassed());
    }
    
    [Test]
    public void SeparateCardsAreUnrelated()
    {
        var ancestrallRecall = new Card("Ancestral Recall");
        Assert.IsFalse(ancestrallRecall.IsRelated(_lightningBolt));
    }
    
    [Test]
    public void UpgradeIsRelated()
    {
        CreateBreathToBoltChain();
        Assert.IsTrue(_shock.IsRelated(_lightningBolt));
    }
    
    [Test]
    public void IndirectUpgradeIsRelated()
    {
        CreateBreathToBoltChain();
        Assert.IsTrue(_breathOfFire.IsRelated(_lightningBolt));
    }
    
    [Test]
    public void CantCreateLoop()
    {
        CreateBreathToBoltChain();
        LogAssert.Expect(LogType.Error, "Can't add upgrade, would create a cycle.");
        _lightningBolt.AddSingleUpgrade(_breathOfFire);
    }
    
    [Test]
    public void RemovingDirectUpgradeRemovesRelationship()
    {
        CreateShockToBolt();
        _shock.RemoveSingleUpgrade(_lightningBolt);
        Assert.IsFalse(_shock.IsRelated(_lightningBolt));
    }
    
    [Test]
    public void BreathIsNotImmediatelyWorseThanBolt()
    {
        CreateBreathToBoltChain();
        Assert.IsFalse(_breathOfFire.IsImmediatelyWorseThan(_lightningBolt));
    }
    
    [Test]
    public void RemovingDirectUpgradeRemovesIndirectUpgrades()
    {
        CreateBreathToBoltChain();
        _breathOfFire.RemoveSingleUpgrade(_shock);
        Assert.IsFalse(_breathOfFire.IsRelated(_lightningBolt));
    }
    
    [Test]
    public void SimplifyRedundantUpgrades()
    {
        _breathOfFire.AddSingleUpgrade(_lightningBolt);
        Assert.IsTrue(_breathOfFire.IsImmediatelyWorseThan(_lightningBolt));
        CreateBreathToBoltChain();
        Assert.IsFalse(_breathOfFire.IsImmediatelyWorseThan(_lightningBolt));
    }
    
    [Test]
    public void RestoreUpgradesAfterUndoingSimplification()
    {
        _breathOfFire.AddSingleUpgrade(_lightningBolt);
        CreateBreathToBoltChain();
        _breathOfFire.RemoveSingleUpgrade(_shock);
        Assert.IsTrue(_breathOfFire.IsImmediatelyWorseThan(_lightningBolt));
    }
    
    [Test]
    public void RestoreUpgradesAfterUndoingRecursiveSimplification()
    {
        var absoluteGarbageCard = new Card("Absolute Garbage");
        absoluteGarbageCard.AddSingleUpgrade(_lightningBolt);
        absoluteGarbageCard.AddSingleUpgrade(_shock);
        Assert.IsTrue(absoluteGarbageCard.IsImmediatelyWorseThan(_lightningBolt));
        Assert.IsTrue(absoluteGarbageCard.IsImmediatelyWorseThan(_shock));
        
        CreateBreathToBoltChain();
        
        absoluteGarbageCard.AddSingleUpgrade(_breathOfFire);
        Assert.IsFalse(absoluteGarbageCard.IsImmediatelyWorseThan(_lightningBolt));
        Assert.IsFalse(absoluteGarbageCard.IsImmediatelyWorseThan(_shock));
        
        _shock.RemoveSingleUpgrade(_lightningBolt);
        Assert.IsFalse(absoluteGarbageCard.IsImmediatelyWorseThan(_shock));
        Assert.IsTrue(absoluteGarbageCard.IsImmediatelyWorseThan(_lightningBolt));
    }
}