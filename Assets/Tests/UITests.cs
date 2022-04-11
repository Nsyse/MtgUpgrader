using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class UITests
{
    private string _gargoyleName = "Granite Gargoyle";
    private Card _gargoyle;

    [SetUp]
    public void Setup()
    {
        LazyFindGraniteGargoyle();
    }

    [Test]
    public void CanFindGraniteGargoyle()
    {
        Assert.IsTrue(_gargoyle != null);
        Assert.IsTrue(_gargoyle.Name == _gargoyleName);
    }

    [Test]
    public void CanLazyDownloadGraniteGargoyleTexture()
    {
        var task = Task.Run(async () =>
        {
            return await _gargoyle.LoadImage();
        });
        

        Assert.IsNotNull(task.Result);
    }

    private void LazyFindGraniteGargoyle()
    {
        if (_gargoyle != null)
            return;
        
        ScryfallDb db = new ScryfallDb();

        Card result = db.FindWithFullName(_gargoyleName);
        _gargoyle = result;
    }
}