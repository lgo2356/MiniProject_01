using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRuleManager : MonoBehaviour
{
    [SerializeField]
    private Text GameOverText;

    private static GameRuleManager instance;

    public static GameRuleManager Instance => instance;

    private Dictionary<int, Enemy> enemyTable;
    private Player player;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);

        enemyTable = new();
    }

    public void RegisterPlayer(Player player)
    {
        this.player = player;
        this.player.OnDead += OnPlayerDead;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemyTable.ContainsKey(enemy.gameObject.GetInstanceID()) == false)
        {
            enemyTable.Add(enemy.gameObject.GetInstanceID(), enemy);

            enemy.OnDead += OnEnemyDead;
        }
    }

    private void OnPlayerDead(Player player)
    {
        Debug.Log("You Died.");

        StaggerFrameManager.Instance.DelayAndSlow(25, 0.7f);
        //StaggerFrameManager.Instance.Delay(20);

        GameOverText.gameObject.SetActive(true);
        GameOverText.text = "You Died.";
        GameOverText.color = Color.red;
    }

    private void OnEnemyDead(Enemy enemy)
    {
        enemyTable.Remove(enemy.gameObject.GetInstanceID());
        
        if (enemyTable.Count == 0)
        {
            Debug.Log("You Win.");

            GameOverText.gameObject.SetActive(true);
            GameOverText.text = "You Win.";
            GameOverText.color = Color.blue;
        }
    }
}
