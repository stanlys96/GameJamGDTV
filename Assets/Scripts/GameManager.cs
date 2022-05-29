using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Saving;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private void Awake()
    {
        savingWrapper = FindObjectOfType<SavingWrapper>();
        if (GameManager.instance != null) {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            return;
        }
        instance = this;
    }

    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<RuntimeAnimatorController> playerAnimators;
    public List<int> weaponPrices;
    public List<int> xpTable;
    public Sprite powerPlayerSprite;
    public RuntimeAnimatorController powerPlayerAnimatorController;
    public GameObject livesContainer;
    public SavingWrapper savingWrapper;

    // References
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public int pesos;
    public int experience;
    public RectTransform hitpointBar;
    public Animator deathMenuAnim;
    public Animator winMenuAnim;
    public GameObject hud;
    public GameObject menu;
    public GameObject winMenu;
    public GameObject deathMenu;
    public int playerLives = 3;
    public bool alreadyLoad = false;
    private int asd = 0;

    // Floating text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    public void DropOnDead(GameObject objectToInstantiate, Transform transform)
    {
        GameObject instantiatedGameObject = Instantiate(objectToInstantiate, transform.position, transform.rotation);
        instantiatedGameObject.transform.SetParent(GameObject.FindWithTag("Drops").transform);
    }

    // Upgrade Weapon
    public bool TryUpgradeWeapon()
    {
        // Is the weapon at max level?
        if (weaponPrices.Count <= weapon.weaponLevel)
        {
            return false;
        }

        if (pesos >= weaponPrices[weapon.weaponLevel])
        {
            pesos -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }

        return false;
    }

    public void OnHitpointChange()
    {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    // Experience System
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count)
            {
                return r;
            }
        }

        return r;
    }

    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0;

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }

        return xp;
    }

    public void GrantXp(int xp)
    {
        int currentLevel = GetCurrentLevel();
        experience += xp;
        if (currentLevel < GetCurrentLevel())
        {
            OnLevelUp();
        }
    }

    public void OnLevelUp()
    {
        player.OnLevelUp();
        OnHitpointChange();
    }

    public void Save()
    {
        savingWrapper.Save();
    }

    public void Load()
    {
        savingWrapper.Load();
    }

    public void Delete()
    {
        player.BeNormalPlayer();
        player.powerPlayerCounter = Mathf.Infinity;
        savingWrapper = FindObjectOfType<SavingWrapper>();
        savingWrapper.Delete();
        SceneManager.LoadScene("Main");
        deathMenuAnim.SetTrigger("Hide");
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
        pesos = 0;
        experience = 0;
        playerLives = 3;
        player.Respawn();
        weapon.weaponLevel = 0;
        for (int i = 0; i < livesContainer.transform.childCount; i++)
        {
            livesContainer.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    // Death Menu and Respawn
    public void Respawn()
    {
        player.BeNormalPlayer();
        player.powerPlayerCounter = Mathf.Infinity;
        savingWrapper = FindObjectOfType<SavingWrapper>();
        savingWrapper.Load();
        deathMenuAnim.SetTrigger("Hide");
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
        pesos = 0;
        experience = 0;
        playerLives = 3;
        player.Respawn();
        for (int i = 0; i < livesContainer.transform.childCount; i++)
        {
            livesContainer.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        player.BeNormalPlayer();
        savingWrapper = FindObjectOfType<SavingWrapper>();
        savingWrapper.Delete();
        winMenuAnim.SetTrigger("Hide");
        SceneManager.LoadScene("Main");
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
        pesos = 0;
        experience = 0;
        playerLives = 3;
        player.Respawn();
        for (int i = 0; i < livesContainer.transform.childCount; i++)
        {
            livesContainer.transform.GetChild(i).gameObject.SetActive(true);
        }
        alreadyLoad = true;
    }
}
