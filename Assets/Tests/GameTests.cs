using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void SimpleMathTest()
    {
        // Use the Assert class to test conditions
        Assert.AreEqual(2, 1 + 1);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator GameObjectCreationTest()
    {
        // Setup
        var go = new GameObject("TestObject");
        
        // Assert
        Assert.IsNotNull(go);
        Assert.AreEqual("TestObject", go.name);

        // Cleanup
        yield return null;
        Object.DestroyImmediate(go);
    }
}
