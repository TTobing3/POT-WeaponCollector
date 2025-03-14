using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public GameObject battleScreen;

    public int[] turn = new int[2] { 0, 0 }; // turn[0] 0�̸� ������, 0���� ũ�� ����  / turn[1] = { ������, �÷��̾�, �� }
    public bool interectLock = false;
    public bool enemyFocus = false;

    public Player player;
    public Enemy enemy;
    public Hand hand;
    public Deck deck;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void AttackButton()
    {
        if (turn[1] != 1 || interectLock) return;

        if (!enemyFocus)
        {
            enemyFocus = true;
            return;
        }
        else
        {
            ActPlayerSkill();

            enemy.enemyInterect.FocusOn(false);
        }

    }

    #region Battle Cycle
    public void StartBattle()
    {
        if (turn[0] != 0) return;

        battleScreen.SetActive(true);
        UIManager.instance.SetEnemyUI(true, null);

        deck.GetComponent<RectTransform>().DOAnchorPosX(500, 1);
        hand.GetComponent<RectTransform>().DOAnchorPosY(-230, 1);

        turn[0] = 1;

        StartPlayerTurn();
    }

    #region Player Turn
    void StartPlayerTurn()
    {
        turn[1] = 1;
    }

    void ActPlayerSkill()
    {
        EventManager.instance.StartPlayerAct(() =>
        {
            interectLock = true;
            player.ActSkill(enemy);
        });
    }

    public void FinishPlayerSkill()
    {
        interectLock = false;
        Hand.instance.ClearHand();

        EventManager.instance.FinishPlayerTurn(() =>
        {
            EndPlayerTurn();
        });
    }

    void EndPlayerTurn()
    {
        player.EraseEmptyBuff();
        Deck.instance.ResetTempUpgrade(true);

        if (CheckFinishGame()) return;
        //�� ���ʷ�
        StartEnemyTurn();
    }

    #endregion

    #region EnemyTurn

    void StartEnemyTurn()
    {
        turn[1] = 2;
        ActEnemySkill();
    }

    void ActEnemySkill()
    {
        EventManager.instance.StartEnemyAct(() =>
        {
            enemy.ActSkill(player);
        });
    }

    public void FinishEnemySkill()
    {
        interectLock = false;
        EventManager.instance.FinishEnemyTurn(() =>
        {
            EndEnemyTurn();
        });
    }

    void EndEnemyTurn()
    {
        enemy.EraseEmptyBuff();

        if (CheckFinishGame()) return;

        //�÷��̾� ���ʷ�
        turn[1] = 0;
        PassTurn();
    }

    #endregion

    void PassTurn()
    {
        //���� ��
        turn[0] += 1;

        StartPlayerTurn();
    }

    #endregion

    bool CheckFinishGame()
    {
        if(player.onDie || enemy.onDie)
        {
            EndBattle();
            return true;
        }

        return false;
    }

    void EndBattle()
    {
        //�� ���� �ʱ�ȭ
        turn = new int[2] { 0, 0 };

        Deck.instance.ResetTempUpgrade(false);

        deck.GetComponent<RectTransform>().DOAnchorPosX(-500, 1);
        hand.GetComponent<RectTransform>().DOAnchorPosY(-500, 1);

        UIManager.instance.SetEnemyUI(false, ()=> battleScreen.SetActive(false));

        player.ResetState();

        if (player.onDie)
            Defeat();
        else if (enemy.onDie)
            Victory();
    }

    void Victory()
    {
        if(StageManager.instance.roundData.type == "����")
        {
            StageManager.instance.FinishBattleRound();
        }
        else if(StageManager.instance.roundData.type == "����")
        {
            StageManager.instance.FinishBossRound();
        }
    }

    void Defeat()
    {
        print("�й�!");
    }
}
