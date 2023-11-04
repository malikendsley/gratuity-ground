using System;
using System.Collections;
using System.Collections.Generic;
using Endsley;
using UnityEngine;
using UnityEngine.UI;

// Subscribes to a Main weapon fire and main weapon reload event
// Manages the UI for the ammo count
// The ammo count is a semicircle that contains small bullet icons
// As the weapon is fired, the bullet icons bounce out of the semicircle
// They are replaced at the other end of the semicircle if there are still bullets left
// If the weapon is reloaded, the "cyclinder" will spin until the weapon is reloaded'
// Bullet icons will have a subtle trail behind them (in screen space)
// Bullet icons will manage their own appearance but be moved by this script
public class AmmoUIController : MonoBehaviour
{
    [SerializeField] GameObject bulletIconPrefab;
    [SerializeField] int maxBulletIcons = 10;
    [SerializeField] RectTransform bulletIconStart;
    [SerializeField] float bulletIconSpacingPx = 10f;
    // HACK: requiring that the player's weapon be referenced here is not ideal
    // We should be able to subscribe to the player's weapon without knowing the mech
    // Or at least be able to subscribe at the mech level rather than the gameobject level
    [SerializeField] GameObject playerWeapon;
    private WeaponsBus weaponsBus;
    private Action<WeaponEventData> HandleAmmoDecrease;
    private Action<WeaponEventData> HandleReloadStart;
    private Action<WeaponEventData> HandleReloadFinish;

    readonly List<GameObject> bulletIcons = new();

    void Start()
    {
        weaponsBus = WeaponsBusManager.Instance.GetOrCreateBus(playerWeapon);
        if (weaponsBus == null)
        {
            Debug.LogError("Could not find weapons bus for player mech");
            return;
        }
        else
        {
            HandleAmmoDecrease = (WeaponEventData data) => HandleBulletFired(data.RemainingAmmo);
            HandleReloadStart = (WeaponEventData data) => HandleWeaponStartReload();
            HandleReloadFinish = (WeaponEventData data) => HandleWeaponFinishReload();
            weaponsBus.Subscribe(WeaponEventType.OnAmmoDecrease, HandleAmmoDecrease);
            weaponsBus.Subscribe(WeaponEventType.OnReloadStart, HandleReloadStart);
            weaponsBus.Subscribe(WeaponEventType.OnReloadFinish, HandleReloadFinish);
        }
        InitializeAmmo();
    }

    void HandleBulletFired(int remaining)
    {

        // Grab the first bullet in the queue and "fire" it (play its animation and then destroy it)
        // If there are more than maxBulletIcons in the player's magazine, add a new bullet to the queue
        Debug.Log("UI: Bullet Fired");
        // Fire the first bullet in the queue
        if (bulletIcons.Count > 0)
        {
            var bullet = bulletIcons[0];
            bulletIcons.RemoveAt(0);
            bullet.GetComponent<BulletUIController>().Fire(BulletUIController.BulletDirection.Right);
        }
        for (int i = 0; i < bulletIcons.Count; i++)
        {
            // Move each bullet icon to the left by bulletIconSpacingPx
            var bullet = bulletIcons[i];
            bullet.transform.position = bullet.transform.position + new Vector3(bulletIconSpacingPx, 0, 0);
        }
        if (remaining > maxBulletIcons)
        {
            // Add a new one at the bullet icon start
            var bullet = Instantiate(bulletIconPrefab, bulletIconStart.position, Quaternion.identity);
            // set a random color
            bullet.GetComponent<Image>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            bullet.transform.SetParent(transform, true);
            bulletIcons.Add(bullet);
        }

    }

    void HandleWeaponStartReload()
    {
        Debug.Log("UI: Start Reload");
        foreach (var bullet in bulletIcons)
        {
            bullet.GetComponent<BulletUIController>().Eject();
        }
        bulletIcons.Clear();
    }

    void HandleWeaponFinishReload()
    {
        Debug.Log("UI: Finish Reload");
        InitializeAmmo();
    }

    void InitializeAmmo()
    {
        // Starting at the bulletIconStart, instantiate maxBulletIcons bullet icons and shift right by bulletIconSpacingPx
        // Make them all a child of this object
        for (int i = maxBulletIcons; i >= 0; i--)
        {
            var bullet = Instantiate(bulletIconPrefab, bulletIconStart.position + new Vector3(bulletIconSpacingPx * i, 0, 0), Quaternion.identity);
            bullet.GetComponent<BulletUIController>().Inject();
            bullet.GetComponent<Image>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            bullet.transform.SetParent(transform, true);
            bulletIcons.Add(bullet);
        }
    }
}
