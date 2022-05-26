using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (GameManager.instance != null) {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            return;
        }

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<RuntimeAnimatorController> playerAnimators;
    public List<int> weaponPrices;
    public List<int> xpTable;
    public Sprite powerPlayerSprite;
    public RuntimeAnimatorController powerPlayerAnimatorController;
    public GameObject livesContainer;

    // References
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public int pesos;
    public int experience;
    public RectTransform hitpointBar;
    public Animator deathMenuAnim;
    public GameObject hud;
    public GameObject menu;
    public int playerLives = 3;

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

    // Save state
    // INT preferred skin
    // INT pesos
    // INT experience
    // INT weaponLevel

    // On Scene Loaded
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        if (data.Length > 1)
        {
            string updateTransformData = data[4].Replace("(", "");
            string anotherUpdateTransformData = updateTransformData.Replace(")", "");
            string[] splitted = anotherUpdateTransformData.Split(',');

            player.savedPosition = new Vector2(float.Parse(splitted[0].Trim()), float.Parse(splitted[1].Trim()));
            player.savedScene = data[5];

            if (player.savedScene == SceneManager.GetActiveScene().name && (player.savedPosition != null))
            {
                player.transform.position = player.savedPosition;
            }
            else
            {
                player.transform.position = GameObject.Find("SpawnPoint").transform.position;
            }
        }
    }

    // Death Menu and Respawn
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        SceneManager.LoadScene("Main");
        player.Respawn();
    }

    public void SaveGame()
    {
        string s = "";
        player.savedPosition = player.transform.position;
        player.savedScene = SceneManager.GetActiveScene().name;

        s += "0" + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString() + "|";
        s += player.savedPosition.ToString() + "|";
        s += player.savedScene;

        PlayerPrefs.SetString("SaveState", s);
    }

    // Load state
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState")) return;
        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        SceneManager.LoadScene(data[5]);

        // Change player skin
        pesos = int.Parse(data[1]);

        // Experience
        experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1)
        {
            player.SetLevel(GetCurrentLevel());
        }

        string updateTransformData = data[4].Replace("(", "");
        string anotherUpdateTransformData = updateTransformData.Replace(")", "");
        string[] splitted = anotherUpdateTransformData.Split(',');

        // Change the weapon level
        weapon.SetWeaponLevel(int.Parse(data[3]));
        player.transform.position = new Vector2(float.Parse(splitted[0].Trim()), float.Parse(splitted[1].Trim()));
    }

    public void DeleteState()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("Main");
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
        pesos = 0;
        experience = 0;
        player.Respawn();
    }
}
