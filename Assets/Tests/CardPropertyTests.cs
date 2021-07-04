using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

[ExcludeFromCodeCoverage]
public class CardPropertyTests
{
    private Card _shock;
    private Card _lightningBolt;

    [Test]
    public void CardStoresId()
    {
        var id = "newCard";
        Card newCard = new Card(id);
        Assert.IsTrue(newCard.MatchesId(id));
    }
    
    [Test]
    public void NewCardIsOrphaned()
    {
        Card orphan = new Card("orphan");
        Assert.IsTrue(orphan.IsOrphaned());
    }
}