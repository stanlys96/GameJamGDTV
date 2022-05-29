using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Portal : Collidable
{
    public string[] sceneNames;
    public float cooldown = 4.0f;
    public bool winPortal = false;
    private float lastShout = -4.0f;

    protected override void OnCollide(Collider2D coll)
    {
        int count = 0;
        GameObject[] fighters = GameObject.FindGameObjectsWithTag("Fighter");
        foreach (GameObject fighter in fighters)
        {
            if (fighter.name != "Player")
            {
                count++;
            }
        }
        if (coll.gameObject.name == "Player")
        {
            if (count != 0)
            {
                if (Time.time - lastShout > cooldown)
                {
                    lastShout = Time.time;
                    GameManager.instance.ShowText("Kill all the enemies before moving to the next level!", 25, Color.white, transform.position + new Vector3(0, 0.16f, 0), Vector3.zero, cooldown);
                }
            } 
            else
            {
                if (!winPortal)
                {
                    StartCoroutine(Transition());
                }
                else
                {
                    GameManager.instance.deathMenu.GetComponent<Image>().enabled = false;
                    GameManager.instance.winMenuAnim.SetTrigger("Show");
                }
            }
        }
    }

    private IEnumerator Transition()
    {
        string sceneName = sceneNames[Random.Range(0, sceneNames.Length)];
        DontDestroyOnLoad(gameObject);
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        wrapper.Save();
        yield return SceneManager.LoadSceneAsync(sceneName);
        wrapper.Load();
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        GameObject player = GameObject.Find("Player");
        player.transform.position = spawnPoint.transform.position;
        wrapper.Save();
        Destroy(gameObject);
    }
}
