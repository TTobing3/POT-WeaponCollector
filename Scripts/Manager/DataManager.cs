using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

/*
 검 창 도끼 둔기 활 석궁 총 대포 책 오브 완드 지팡이 방패 악기 물약 투척
*/

public enum WeaponType { Sword, Spear, Axe, Blunt, Bow, CrossBow, Gun, Cannon, Book, Orb, Wand, Staff, Shield, Instrument, Potion, Guitar }

public static class SkillCaculator
{

    // 확률

    public static bool Chance(int chance)
    {
        if (Random.Range(0, 100) < chance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 무기

    public static string Caculate(string type, float power, float skillPower)
    {
        switch (type)
        {
            case "피해": return NormalAttack(power, skillPower);
            case "갑옷": return Armor(power, skillPower);
            case "공포": return Panic(power, skillPower);
            default: return NormalAttack(power, skillPower);
        }
    }

    static string NormalAttack(float power, float skillPower)
    {


        int totalPower = (int)(power * skillPower);
        return totalPower + "";
    }

    static string Panic(float power, float skillPower)
    {
        int totalPower = (int)(power * skillPower);
        return totalPower + "";
    }

    static string Armor(float power, float skillPower)
    {
        int totalPower = (int)(power * skillPower);
        return totalPower + "";
    }

}

#region Data

[System.Serializable]
public class WeaponData
{
    public int number, health, magic, power;
    public string name, rank, type,  backstory; // groun : 계열 table : 소속
    public string[] alignment, group;
    public List<string> skills;

    public WeaponData()
    {
        this.number = -1;
        this.name = "";
        this.rank = "";
        this.health = 0;
        this.magic = 0;
        this.power = 0;
        this.backstory = "";
    }
        
    // 생성자
    public WeaponData(int number, string name, string rank, string[] group, string[] alignment, string type, int power, int health, int magic,  List<string> skills, string backstory)
    {
        this.number = number;
        this.name = name;
        this.rank = rank;
        this.group = group;
        this.alignment = alignment;
        this.type = type;
        this.power = power;
        this.health = health;
        this.magic = magic;
        this.skills = skills;
        this.backstory = backstory;
    }
}
public class SkillData
{
    public int number;
    public string name;
    public int chance;
    public string effect, effectSprite;
    public string description;

    public int[] effectRange
    {
        get
        {
            var value = effect.Split(':')[1];
            var range = new int[2];

            if(effect.Split(':')[1].Contains('~'))
            {
                range = value.Split('~').Select(x => int.Parse(x)).ToArray();
            }
            else
            {

                range = new int[2] { int.Parse(value), int.Parse(value) };
            }

            return range;
        }
    }

    public float effectPower
    {
        get
        {
            float power = EventManager.instance.SetEffectRange(effectRange, getEffectPower);

            return power;

            float getEffectPower()
            {
                return Random.Range((float)effectRange[0], effectRange[1]);
            }
        }

    }

    public string effectType
    {
        get
        {
            return effect.Split(':')[0];
        }
    }

    // 생성자
    public SkillData(int number, string name, int chance, string effect, string effectSprite, string description)
    {
        this.number = number;
        this.name = name;
        this.chance = chance;
        this.effect = effect;
        this.effectSprite = effectSprite;
        this.description = description;
    }
}
[System.Serializable]
public class EnemyData
{
    public int number;
    public string name, description;
    public string[] group, alignment;
    public int armor, health;
    public List<string> pattern;

    // 생성자
    public EnemyData(int number, string name, string[] group, string[] alignment, int armor, int health, string description, List<string> pattern)
    {
        this.number = number;
        this.name = name;
        this.group = group;
        this.alignment = alignment;
        this.armor = armor;
        this.health = health;
        this.description = description;
        this.pattern = pattern;
    }
}

public class TalkData
{
    public int number;
    public string name;
    public List<string[]> talkList;
    public TalkData(int number, string name, List<string[]> talkList)
    {
        this.number = number;
        this.name = name;
        this.talkList = talkList;
    }
}

[System.Serializable]
public class EnemySkillData
{
    public int number;
    public string name, effect;
    public List<string> effects;
    public string description;

    // 생성자
    public EnemySkillData(int number, string name, string effect, List<string> effects, string description)
    {
        this.number = number;
        this.name = name;
        this.effect = effect;
        this.effects = effects;
        this.description = description;
    }

}

[System.Serializable]
public class RoundData
{
    public int number, tier;
    public string name, type, target;

    // 생성자
    public RoundData(int number, string name, string type, string target, int tier)
    {
        this.number = number;
        this.type = type;
        this.target = target;
        this.name = name;
        this.tier = tier;
    }
}

public class StageRewardData
{
    public int level;
    public string name;
    public int[] coinRewardRange;

    public int lost;
    public int train, heard;

    public float[] weaponRankProbability;

    public StageRewardData(int level, string name, int[] coinRewardRange, int lost, int train, int heard, float[] weaponRankProbability)
    {
        this.level = level;
        this.name = name;
        this.coinRewardRange = coinRewardRange;
        this.lost = lost;
        this.train = train;
        this.heard = heard;
        this.weaponRankProbability = weaponRankProbability;
    }
}

public class GroupSynergyData
{
    public int number;
    public string name, description;
    public List<string> effects;

    public GroupSynergyData(int number, string name, string description, List<string> effects)
    {
        this.number = number;
        this.name = name;
        this.description = description;
        this.effects = effects;
    }
}

public class BuffData
{
    public int number;
    public string name, user, effect, description;
    public BuffData(int number, string name, string user, string effect, string description)
    {
        this.number = number;
        this.name = name;
        this.user = user;
        this.effect = effect;
        this.description = description;
    }
}

[System.Serializable]
public class MysteryData
{
    public int number;
    public string name;
    public string description;

    // 생성자
    public MysteryData(int number, string name, string description)
    {
        this.number = number;
        this.name = name;
        this.description = description;
    }
}

public class NpcData
{
    public int number;
    public string name, eventName;
    public string[] icons;
    public string[] options;
    public string[] effects;

    // 생성자
    public NpcData(int number, string name, string eventName, string[] icons, string[] options, string[] effects)
    {
        this.number = number;
        this.name = name;
        this.eventName = eventName;
        this.icons = icons;
        this.options = options;
        this.effects = effects;
    }
}

#endregion

public class TrainLevelData
{
    public string name;
    public List<string> possibleWeapon;
    public int requireTrain;
    public float power;

    public TrainLevelData(string name, List<string> possibleWeapon, int requireTrain, float power)
    {
        this.name = name;
        this.possibleWeapon = possibleWeapon;
        this.requireTrain = requireTrain;
        this.power = power;
    }
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public string[] TextData = new string[10];

    // Data

    public Dictionary<string, WeaponData> AllWeaponDatas = new Dictionary<string, WeaponData>();
    public List<WeaponData> AllWeaponDataList = new List<WeaponData>();
    public Dictionary<string, List<string>> AllWeaponDataByRank = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> AllWeaponDataByType = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> AllWeaponDataByGroup = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> AllWeaponDataByAlignment = new Dictionary<string, List<string>>();

    public Dictionary<string, EnemyData> AllEnemyDatas = new Dictionary<string, EnemyData>();
    public List<EnemyData> AllEnemyDataList = new List<EnemyData>();

    public Dictionary<string, EnemySkillData> AllEnemySkillDatas = new Dictionary<string, EnemySkillData>();
    public List<EnemySkillData> AllEnemySkillDataList = new List<EnemySkillData>();

    public Dictionary<string, SkillData> AllSkillDatas = new Dictionary<string, SkillData>();
    public List<SkillData> AllSkillDataList = new List<SkillData>();

    public Dictionary<string, TalkData> AllTalkDatas = new Dictionary<string, TalkData>();
    public List<TalkData> AllTalkDataList = new List<TalkData>();

    public Dictionary<string, RoundData> AllRoundDatas = new Dictionary<string, RoundData>();
    public Dictionary<string, List<string>> AllRoundDataByType = new Dictionary<string, List<string>>();
    public List<RoundData> AllRoundDataList = new List<RoundData>();
    public List<RoundData> AllNormalNpcRoundList = new List<RoundData>();

    public Dictionary<string, GroupSynergyData> AllGroupSynergyDatas = new Dictionary<string, GroupSynergyData>();
    public List<GroupSynergyData> AllGroupSynergyDataList = new List<GroupSynergyData>();

    public Dictionary<string, BuffData> AllBuffDatas = new Dictionary<string, BuffData>();
    public List<BuffData> AllBuffDataList = new List<BuffData>();

    public Dictionary<string, MysteryData> AllMysteryDatas = new Dictionary<string, MysteryData>();
    public List<MysteryData> AllMysteryDataList = new List<MysteryData>();

    public Dictionary<string, NpcData> AllNpcDatas = new Dictionary<string, NpcData>();
    public List<NpcData> AllNpcDataList = new List<NpcData>();

    // Table

    public List<string[]> StageMap = new List<string[]>();
    public List<StageRewardData> StageRewardTable = new List<StageRewardData>();

    public List<TrainLevelData> TrainLevelTable = new List<TrainLevelData>();

    public Dictionary<string, int[]> StoreTable = new Dictionary<string, int[]>();


    public string GetTypeText(int type)
    {
        return GetTypeText((WeaponType)type);
    }
    public string GetTypeText(WeaponType type)
    {

        switch (type)
        {
            case WeaponType.Sword: return "검";
            case WeaponType.Spear: return "창";
            case WeaponType.Axe: return "도끼";
            case WeaponType.Blunt: return "둔기";
            case WeaponType.Bow: return "활";
            case WeaponType.CrossBow: return "석궁";
            case WeaponType.Gun: return "총";
            case WeaponType.Cannon: return "대포";
            case WeaponType.Book: return "책";
            case WeaponType.Orb: return "오브";
            case WeaponType.Wand: return "완드";
            case WeaponType.Staff: return "지팡이";
            case WeaponType.Shield: return "방패";
            case WeaponType.Instrument: return "악기";
            case WeaponType.Potion: return "물약";
            case WeaponType.Guitar: return "기타";
            default: return "???";
        }
    }
    public WeaponType GetWeaponType(string typeText)
    {
        switch (typeText)
        {
            case "검": return WeaponType.Sword;
            case "창": return WeaponType.Spear;
            case "도끼": return WeaponType.Axe;
            case "둔기": return WeaponType.Blunt;
            case "활": return WeaponType.Bow;
            case "석궁": return WeaponType.CrossBow;
            case "총": return WeaponType.Gun;
            case "대포": return WeaponType.Cannon;
            case "책": return WeaponType.Book;
            case "오브": return WeaponType.Orb;
            case "완드": return WeaponType.Wand;
            case "지팡이": return WeaponType.Staff;
            case "방패": return WeaponType.Shield;
            case "악기": return WeaponType.Instrument;
            case "물약": return WeaponType.Potion;
            case "기타": return WeaponType.Guitar;
            default: return WeaponType.Sword;
        }
    }

    public string GetWeaponReward()
    {
        StageRewardData stage = StageRewardTable[StageManager.instance.curStage ];
        float randomValue = Random.Range(0f, 100f);

        string[] itemTypes = { "고물", "일반", "고급", "보물" };

        // 선택된 아이템 반환
        string itemType = itemTypes
            .Zip(stage.weaponRankProbability, (item, prob) => new { item, prob })
            .First(x => (randomValue -= x.prob) < 0)
            .item;

        string item = AllWeaponDataByRank[itemType][Random.Range(0, AllWeaponDataByRank[itemType].Count)];

        return item;
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        // WeaponData
        string[] line = TextData[0].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            // WeaponData 객체 생성
            var weaponData = new WeaponData(
                int.Parse(e[0]),  // number
                e[1],             // name
                e[2],             // rank
                e[3].Split("/"),             // group
                e[4].Split("/"),             // alignment
                e[5],             // type
                int.Parse(e[6]),             // power
                int.Parse(e[7]),             // heath
                int.Parse(e[8]),             // magic
                e[9].Split("/").ToList(),    // skill
                e[10]              // backstory
            );

            // Dictionary 및 List에 추가
            AllWeaponDatas.Add(e[1], weaponData); // name을 키로 사용
            AllWeaponDataList.Add(weaponData);

            //등급별
            if (!AllWeaponDataByRank.ContainsKey(e[2]))
            {
                AllWeaponDataByRank[e[2]] = new List<string>();
            }

            AllWeaponDataByRank[e[2]].Add(e[1]);

            //소속별
            foreach(string j in e[3].Split("/"))
            {
                if (!AllWeaponDataByGroup.ContainsKey(j))
                {
                    AllWeaponDataByGroup[j] = new List<string>();
                }

                AllWeaponDataByGroup[j].Add(e[1]);
            }

            //성향별
            foreach (string j in e[4].Split("/"))
            {
                if (!AllWeaponDataByAlignment.ContainsKey(j))
                {
                    AllWeaponDataByAlignment[j] = new List<string>();
                }

                AllWeaponDataByAlignment[j].Add(e[1]);
            }

            //계열별
            foreach (string j in e[5].Split("/"))
            {
                if (!AllWeaponDataByType.ContainsKey(j))
                {
                    AllWeaponDataByType[j] = new List<string>();
                }

                AllWeaponDataByType[j].Add(e[1]);
            }
        }

        // EnemyData
        line = TextData[1].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            // EnemyData 객체 생성
            var enemyData = new EnemyData(
                int.Parse(e[0]),  // number
                e[1],             // name
                e[2].Split("/"),             // group
                e[3].Split("/"),             // alignment
                int.Parse(e[4]),  // armor
                int.Parse(e[5]),  // health
                e[6],             // description
                e[7].Split('/').ToList()         // pattern
            );

            // Dictionary 및 List에 추가
            AllEnemyDatas.Add(e[1], enemyData); // name을 키로 사용
            AllEnemyDataList.Add(enemyData);
        }

        // EnemySkillData
        line = TextData[2].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            // EnemySkillData 객체 생성
            var enemySkillData = new EnemySkillData(
                int.Parse(e[0]),  // number
                e[1],             // name
                e[2],             // effect
                e[3].Split("/").ToList(),      // effects
                e[4]   // description
            );

            // Dictionary 및 List에 추가
            AllEnemySkillDatas.Add(e[1], enemySkillData); // name을 키로 사용
            AllEnemySkillDataList.Add(enemySkillData);
        }

        // SkillData
        line = TextData[3].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            // SkillData 객체 생성
            var skillData = new SkillData(
                int.Parse(e[0]),  // number
                e[1],             // name
                int.Parse(e[2]),  // chance
                e[3],             // effects
                e[4],             // effect 스프라이트
                e[5]              // description
            );

            // Dictionary 및 List에 추가
            AllSkillDatas.Add(e[1], skillData); // name을 키로 사용
            AllSkillDataList.Add(skillData);
        }

        // StageMap
        line = TextData[4].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            StageMap.Add(e); // 번호 제외 부분 참조 [0..e.Length] 부분 참조

        }

        // TalkData
        line = TextData[5].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            // 문자열을 List<string[]>로 변환
            List<string[]> talks = new List<string[]>();
            for (int j = 2; j < e.Length; j++)
            {
                string[] splitTalks = e[j].Split(':');
                talks.Add(splitTalks);
            }

            // EventTalkData 객체 생성
            var eventTalkData = new TalkData(
                int.Parse(e[0]),  // number
                e[1],             // name
                talks             // talkList
            );

            // Dictionary 및 List에 추가
            AllTalkDatas.Add(eventTalkData.name, eventTalkData); // name을 키로 사용
            AllTalkDataList.Add(eventTalkData);
        }

        // RoundData
        var lines = TextData[6].Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
            string[] e = lines[i].Split('\t');

            // RoundData 객체 생성
            var roundData = new RoundData(
                int.Parse(e[0]),  // number
                e[1],             // name
                e[2],             // type
                e[3],             // target
                int.Parse(e[4])  // number
            );

            // Dictionary 및 List에 추가
            AllRoundDatas.Add(e[1], roundData); // type을 키로 사용
            AllRoundDataList.Add(roundData);

            if (!AllRoundDataByType.ContainsKey(e[2]))
            {
                AllRoundDataByType[e[2]] = new List<string>();
            }

            AllRoundDataByType[e[2]].Add(e[1]);

            if(e[2] == "NPC" && e[3] != "드워프" && e[3] != "엘프")
            {
                AllNormalNpcRoundList.Add(roundData);
            }
        }

        // StageReward
        lines = TextData[7].Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
            string[] e = lines[i].Split('\t');

            var stageReward = new StageRewardData(
                int.Parse(e[0]),
                e[1],
                e[2].Split('~').Select(x => int.Parse(x)).ToArray(),
                int.Parse(e[3]),
                int.Parse(e[4]),
                int.Parse(e[5]),
                new float[] { float.Parse(e[6]), float.Parse(e[7]), float.Parse(e[8]), float.Parse(e[9]) }
                );

            StageRewardTable.Add(stageReward);
        }

        // StoreTable
        lines = TextData[8].Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
            string[] e = lines[i].Split('\t');

            StoreTable.Add(e[1], e[3].Split("~").Select(x=>int.Parse(x)).ToArray());
        }

        // GroupSynergy
        lines = TextData[9].Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
            string[] e = lines[i].Split('\t');

            var groupSynergy = new GroupSynergyData(
                int.Parse(e[0]),
                e[1],
                e[2],
                e.ToList().GetRange(3, 9)
                );

            AllGroupSynergyDatas.Add(e[1], groupSynergy);
            AllGroupSynergyDataList.Add(groupSynergy);
        }

        // TrainTable
        lines = TextData[10].Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
            string[] e = lines[i].Split('\t');

            var trainLevel = new TrainLevelData(
                e[0],
                e[1].Split(',').ToList(),
                int.Parse(e[2]),
                float.Parse(e[3])
                );

            TrainLevelTable.Add(trainLevel);
        }

        // Buff
        lines = TextData[11].Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
            string[] e = lines[i].Split('\t');

            // BuffData 객체 생성
            var buffData = new BuffData(
                int.Parse(e[0]),  // number
                e[1],             // name
                e[2],             // user
                e[3],             // effect
                e[4]              // description
            );

            // Dictionary 및 List에 추가
            AllBuffDatas.Add(buffData.name, buffData); // name을 키로 사용
            AllBuffDataList.Add(buffData);
        }

        // Mystery
        line = TextData[12].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            // MysteryData 객체 생성
            var mysteryData = new MysteryData(
                int.Parse(e[0]),  // number
                e[1],             // name
                e[2]              // description
            );

            // Dictionary 및 List에 추가
            AllMysteryDatas.Add(e[1], mysteryData); // name을 키로 사용
            AllMysteryDataList.Add(mysteryData);
        }

        // Npc
        line = TextData[13].Split('\n');
        for (int i = 1; i < line.Length; i++)
        {
            line[i] = line[i].Trim();
            string[] e = line[i].Split('\t');

            // NpcData 객체 생성
            var npcData = new NpcData(
                int.Parse(e[0]),       // number
                e[1],                  // name
                e[2],                  // eventName
                e[3].Split('/'),       // icons
                e[4].Split('/'),       // options
                e[5].Split('/')        // effects
            );

            // Dictionary 및 List에 추가
            AllNpcDatas.Add(e[1], npcData); // name을 키로 사용
            AllNpcDataList.Add(npcData);
        }
    }


    #region Data Load

    const string 
        weaponURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=0",
        enemyURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=102327615",
        enemySkillURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=2098886253",
        skillURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=92247646",
        stageMapURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=427129891",
        talkURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=2076054968",
        roundDataURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=172777023",
        rewardTableURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=2016275089",
        storeTableURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=1222725420",
        groupSynergyURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=994382768",
        trainTableURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=159523360",
        buffURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=1075815587",
        mysteryURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=1719480955",
        npcURL = "https://docs.google.com/spreadsheets/d/1W4KM2JjqeIpNWDZQwzXyp98ERuUomFWG9s7j2mr-dFY/export?format=tsv&gid=2044722103";

    [ContextMenu("Data Load")]
    void GetLang()
    {
        StartCoroutine(GetLangCo());
    }

    IEnumerator GetLangCo()
    {
        UnityWebRequest www = UnityWebRequest.Get(weaponURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 0);

        www = UnityWebRequest.Get(enemyURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 1);

        www = UnityWebRequest.Get(enemySkillURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 2);

        www = UnityWebRequest.Get(skillURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 3);

        www = UnityWebRequest.Get(stageMapURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 4);

        www = UnityWebRequest.Get(talkURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 5);

        www = UnityWebRequest.Get(roundDataURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 6);

        www = UnityWebRequest.Get(rewardTableURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 7);

        www = UnityWebRequest.Get(storeTableURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 8);

        www = UnityWebRequest.Get(groupSynergyURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 9);

        www = UnityWebRequest.Get(trainTableURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 10);

        www = UnityWebRequest.Get(buffURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 11);

        www = UnityWebRequest.Get(mysteryURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 12);

        www = UnityWebRequest.Get(npcURL);
        yield return www.SendWebRequest();
        SetDataList(www.downloadHandler.text, 13);
        Debug.Log("Clear");
    }

    void SetDataList(string tsv, int i)
    {
        TextData[i] = tsv;
    }

    #endregion
}