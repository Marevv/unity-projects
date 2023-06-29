using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.XR;

public class QuestManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    private Dictionary<string, Quest> _questMap;

    private int currentPlayerLevel;

    private void Awake()
    {
        _questMap = CreateQuestMap();
        Quest quest = GetQuestById("CollectCoinsQuest");
    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.OnStartQuest += StartQuest;
        GameEventsManager.instance.questEvents.OnAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvents.OnFinishQuest += FinishQuest;
        GameEventsManager.instance.questEvents.OnQuestStepStateChange += QuestStepStateChange; 

        GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.OnStartQuest -= StartQuest;
        GameEventsManager.instance.questEvents.OnAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvents.OnFinishQuest -= FinishQuest;
        GameEventsManager.instance.questEvents.OnQuestStepStateChange -= QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
    }

    private void Start()
    {
        foreach (Quest quest in _questMap.Values)
        {
            if(quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;

        if (currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }

        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }

        }
        return meetsRequirements;
    }

    private void Update()
    {
        foreach(Quest quest in _questMap.Values)
        {
            if(quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning($"Duplicate ID foud when creating quest map: {questInfo.id}");
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = _questMap[id];
        if (quest == null)
        {
            Debug.LogError($"ID not found in the Quest Map: {id}");
        }

        return quest;
    }
    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id,QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.id,QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

    private void ClaimRewards(Quest quest)
    {
        GameEventsManager.instance.goldEvents.GoldGained(quest.info.goldReward);
        GameEventsManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private void OnApplicationQuit()
    {
        foreach(Quest quest in _questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);

            PlayerPrefs.SetString(quest.info.id, serializedData);
        }
        catch (System.Exception e) 
        {
            Debug.LogError($"Failed to save quest with id {quest.info.id}: {e}");
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try
        {
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);

            }
            else
            {
                quest = new Quest(questInfo);
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Failed to load quest with id {quest.info.id}: {e}");

        }

        return quest;
    }
}
