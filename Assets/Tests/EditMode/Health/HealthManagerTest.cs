using NUnit.Framework;
using UnityEngine;
using Endsley;

public class HealthManagerTest
{

    GameObject testObject;
    HealthManager healthManager;
    bool wasEventFired;
    MockDie mockDie;
    // A Test behaves as an ordinary method
    [SetUp]
    public void SetUp(){
        testObject = new GameObject();
        healthManager = testObject.AddComponent<HealthManager>();
        mockDie = testObject.AddComponent<MockDie>();
        HealthConfig testHealthConfig = ScriptableObject.CreateInstance<HealthConfig>();
        wasEventFired = false;
        healthManager.OnDamageTaken += OnEventFired;  // Subscribe to the event here

        testHealthConfig.maxHealth = 100;
        testHealthConfig.maxShields = 100;
        healthManager.healthConfig = testHealthConfig;

        healthManager.Start();
    }

    void OnEventFired(int damage)
    {
        wasEventFired = true;
    }


    [TearDown]
    public void TearDown(){
        healthManager.OnDamageTaken -= OnEventFired;  // Unsubscribe to avoid memory leaks
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void TakeDamage_ReducesShields(){
        healthManager.TakeDamage(50);
        Assert.AreEqual(50, healthManager.GetCurrentShields());
    }
    [Test]
    public void TakeDamage_WontOverkillShields(){
        healthManager.TakeDamage(150);
        //health intact, shields 0
        Assert.AreEqual(100, healthManager.GetCurrentHealth());
        Assert.AreEqual(0, healthManager.GetCurrentShields());
    }
    [Test]
    public void TakeDamage_ReducesHealth(){
        healthManager.TakeDamage(100);
        healthManager.TakeDamage(50);
        Assert.AreEqual(50, healthManager.GetCurrentHealth());
        Assert.AreEqual(0, healthManager.GetCurrentShields());
    }
    [Test]
    public void TakeDamage_WontOverkillHealth(){
        healthManager.TakeDamage(200);
        healthManager.TakeDamage(200);
        Assert.AreEqual(0, healthManager.GetCurrentShields());
        Assert.AreEqual(0, healthManager.GetCurrentHealth());
    }
    [Test]
    public void TakeDamage_ZeroDamage(){
        healthManager.TakeDamage(0);
        Assert.AreEqual(100, healthManager.GetCurrentShields());
    }
    [Test]
    public void TakeDamage_TriggersEvents(){
        Assert.False(wasEventFired);
        Assert.False(mockDie.DieCalled);
        healthManager.TakeDamage(100);
        healthManager.TakeDamage(100);
        Assert.True(wasEventFired);
        Assert.True(mockDie.DieCalled);
    }
}