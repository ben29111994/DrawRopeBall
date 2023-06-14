using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenerateLevelEditor : MonoBehaviour
{
    public int levelIndex;

    //[Header("Input Dot")]
    //[Range(3,5)] public int dotColorAmount;
    //[Range(15, 40)] public int dotAmount;

    //[Header("Input Multiply")]
    //[Range(0, 2)] public int multiplyIndex;
    //[Range(1, 4)] public int multiplyAmount;

    //[Header("Input Obstacle")]
    //[Range(0, 3)] public int obstacleAmount;

    public GeneratePhase generatePhase;

    [Header("Input")]
    public int inputDotAmount_1;
    public int inputDotAmount_2;
    public int inputDotAmount_3;
    public int inputMultiplyAmount;
    public int inputObstacleAmount;

    [Header("References")]
    public Dot dotPrefab;
    public Multiply multiplyPrefab;
    public Obstacle obstalcePrefab;

    private List<Dot> listDot = new List<Dot>();
    private List<Multiply> listMultiply = new List<Multiply>();
    private List<Obstacle> listObstacle = new List<Obstacle>();

    private const int LevelMax = 60;
    private Vector2 sizeScreen = new Vector2(6.0f, 9.0f);

    public enum GeneratePhase
    {
        None,
        Generating_Dot,
        Done_Dot,
        Generating_Multiply,
        Done_Multiply,
        Generating_Obstacle,
        Done_Obstacle,
        Done
    }

    public class LevelData
    {
        public List<DataDot> listDataDot = new List<DataDot>();
        public List<DataMultiply> listDataMultiply = new List<DataMultiply>();
        public List<DataObstacle> listDataObstacle = new List<DataObstacle>();
    }


    [System.Serializable]
    public class DataDot
    {
        public Vector3 _position;
        public int _colorID;
    }

    [System.Serializable]
    public class DataMultiply
    {
        public Vector3 _position;
        public int _multiplyIndex;
        public int _multiplyNumber;
    }

    [System.Serializable]
    public class DataObstacle
    {
        public Vector3 _position;
    }

    [NaughtyAttributes.Button]
    public void AutoGenerateLevel()
    {
        StartCoroutine(C_AutoGenerateLevel());
    }

    private IEnumerator C_AutoGenerateLevel()
    {
        levelIndex = 0;
        
        while(levelIndex < LevelMax)
        {
            generatePhase = GeneratePhase.None;
            SpawnRandom(); 

            while (generatePhase != GeneratePhase.Done)
            {
                yield return null;
            }

            SaveData();
            levelIndex++;
            Debug.Log("AutoGenerateLevel -> Done");

            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("AutoGenerateLevel -> Done");
    }

    [NaughtyAttributes.Button]
    public void SpawnRandom()
    {
        ResetReference();
        if (generatePhase != GeneratePhase.None) return;

        StartCoroutine(C_SpawnObstacle());
    }

    public void Stop_Coroutine()
    {
        StopAllCoroutines();
    }

    public void ResetReference()
    {
        generatePhase = GeneratePhase.None;

        for (int i = 0; i < listDot.Count; i++) DestroyImmediate(listDot[i].gameObject);
        listDot.Clear();
        for (int i = 0; i < listMultiply.Count; i++) DestroyImmediate(listMultiply[i].gameObject);
        listMultiply.Clear();
        for (int i = 0; i < listObstacle.Count; i++) DestroyImmediate(listObstacle[i].gameObject);
        listObstacle.Clear();
    }

    [NaughtyAttributes.Button]
    public void SaveData()
    {
        LevelData levelData = new LevelData();

        for(int i = 0; i < listDot.Count; i++)
        {
            Vector3 posData = listDot[i].transform.position;
            int colorIDData = listDot[i].myID;

            DataDot dataDot = new DataDot();
            dataDot._position = posData;
            dataDot._colorID = colorIDData - 1;
            levelData.listDataDot.Add(dataDot);
        }

        for (int i = 0; i < listMultiply.Count; i++)
        {
            Vector3 posData = listMultiply[i].transform.position;
            int multiplyIndex = listMultiply[i].caculateIndex;
            int multiplyNumber = listMultiply[i].multiplyNumber;

            DataMultiply dataMultiply = new DataMultiply();
            dataMultiply._position = posData;
            dataMultiply._multiplyIndex = multiplyIndex;
            dataMultiply._multiplyNumber = multiplyNumber;
            levelData.listDataMultiply.Add(dataMultiply);
        }

        for (int i = 0; i < listObstacle.Count; i++)
        {
            Vector3 posData = listObstacle[i].transform.position;

            DataObstacle dataObstacle = new DataObstacle();
            dataObstacle._position = posData;
            levelData.listDataObstacle.Add(dataObstacle);
        }

        string jsonData = JsonUtility.ToJson(levelData, true);
        int levelIndexFixed = levelIndex + 0;
        string jsonSavePath = Application.dataPath + "/Resources/Levels/Level " + levelIndexFixed + ".json";
        Debug.Log(jsonSavePath);
        try
        {
            File.WriteAllText(jsonSavePath, jsonData);
        }
        catch
        {
            Debug.Log("Save data error .");
        }

        ResetReference();
    }

    [NaughtyAttributes.Button]
    public void LoadData()
    {
        ResetReference();
        string _path = Resources.Load<TextAsset>("Levels/Level " + levelIndex).text;
        LevelData levelData = JsonUtility.FromJson<LevelData>(_path);

        for (int i = 0; i < levelData.listDataDot.Count; i++)
        {
            Vector3 pos = levelData.listDataDot[i]._position;
            int colorID = levelData.listDataDot[i]._colorID;
            SpawnDot(pos, colorID);
        }

        for (int i = 0; i < levelData.listDataMultiply.Count; i++)
        {
            Vector3 pos = levelData.listDataMultiply[i]._position;
            int multiplyIndex = levelData.listDataMultiply[i]._multiplyIndex;
            int multiplyNumber = levelData.listDataMultiply[i]._multiplyNumber;
            SpawnMultiply(pos, multiplyIndex, multiplyNumber);
        }

        for (int i = 0; i < levelData.listDataObstacle.Count; i++)
        {
            Vector3 pos = levelData.listDataObstacle[i]._position;
            SpawnObstacle(pos);
        }
    }

    private IEnumerator C_SpawnDotFunc()
    {
        generatePhase = GeneratePhase.Generating_Dot;

        int dotAmount = GetDotAmount();
        int dotColorAmount = GetDotColorAmount();
        float ratio = GetRatioScale(dotAmount);

        bool isNotRandom = inputDotAmount_1 + inputDotAmount_2 + inputDotAmount_3 > 0 ? true : false;
        if (isNotRandom) dotAmount = inputDotAmount_1 + inputDotAmount_2 + inputDotAmount_3;

        for (int i = 0; i < dotAmount; i++)
        {
            float radius = dotPrefab.transform.localScale.x;
            Vector3 randomPosition = RandomPosition(radius, ratio);
            int a = 0;
            while (randomPosition == Vector3.zero)
            {
                a++;
                if (a <= 50) ratio = ratio;
                else if (a <= 60) ratio = 0.8f;
                else if (a <= 80) ratio = 0.9f;
                else if (a <= 100) ratio = 1.0f;
                randomPosition = RandomPosition(radius, ratio);
                if (a >= 100) break;
            }
            int rDotColor = Random.Range(0, dotColorAmount);
            if (isNotRandom)
            {
                if(inputDotAmount_1 > 0)
                {
                    rDotColor = 0;
                    inputDotAmount_1--;
                }
                else if(inputDotAmount_2 > 0)
                {
                    rDotColor = 1;
                    inputDotAmount_2--;
                }
                else if(inputDotAmount_3 > 0)
                {
                    rDotColor = 2;
                    inputDotAmount_3--;
                }
            }
            if(randomPosition != Vector3.zero) SpawnDot(randomPosition, rDotColor);

            yield return null;
            yield return null;
        }

        generatePhase = GeneratePhase.Done_Dot;
        yield return null;
        generatePhase = GeneratePhase.Done;
    }

    private IEnumerator C_SpawnMultiply()
    {
        generatePhase = GeneratePhase.Generating_Multiply;

        int multiplyAmount = GetMultiplyAmount();
        float ratio = GetRatioScale(multiplyAmount);

        multiplyAmount = inputMultiplyAmount != 0 ? inputMultiplyAmount : multiplyAmount;

        for (int i = 0; i < multiplyAmount; i++)
        {
            float radius = multiplyPrefab.transform.localScale.x;
            Vector3 randomPosition = RandomPosition(radius, ratio);
            int a = 0;
            while (randomPosition == Vector3.zero)
            {
                a++;
                if (a <= 50) ratio = ratio;
                else if (a <= 60) ratio = 0.8f;
                else if (a <= 80) ratio = 0.9f;
                else if (a <= 100) ratio = 1.0f;
                randomPosition = RandomPosition(radius, ratio);

                if (a >= 100) break;
            }
            int multiplyIndex = GetMultiplyIndex();
            if (randomPosition != Vector3.zero) SpawnMultiply(randomPosition, multiplyIndex);

            yield return null;
            yield return null;
        }

        generatePhase = GeneratePhase.Done_Multiply;
        StartCoroutine(C_SpawnDotFunc());
    }

    private IEnumerator C_SpawnObstacle()
    {
        generatePhase = GeneratePhase.Generating_Obstacle;

        int obstacleAmount = GetObstacleAmount();
        float ratio = GetRatioScale(obstacleAmount);

        obstacleAmount = inputObstacleAmount != 0 ? inputObstacleAmount : obstacleAmount;

        for (int i = 0; i < obstacleAmount; i++)
        {
            float radius = obstalcePrefab.transform.localScale.x;
            Vector3 randomPosition = RandomPosition(radius, ratio);
            int a = 0;
            while (randomPosition == Vector3.zero)
            {
                a++;
                if (a <= 50) ratio = ratio;
                else if (a <= 60) ratio = 0.8f;
                else if (a <= 80) ratio = 0.9f;
                else if (a <= 100) ratio = 1.0f;
                randomPosition = RandomPosition(radius, ratio);
                if (a >= 100) break;
            }
            if (randomPosition != Vector3.zero) SpawnObstacle(randomPosition);

            yield return null;
            yield return null;
        }

        generatePhase = GeneratePhase.Done_Obstacle;

        StartCoroutine(C_SpawnMultiply());
    }

    private void SpawnDot(Vector3 pos,int colorID)
    {
        Dot dot = Instantiate(dotPrefab,transform.GetChild(0).GetChild(colorID));
        dot.transform.position = pos;
        dot.myID = colorID + 1;
        listDot.Add(dot);
        dot.ResetColor();
    }

    private void SpawnMultiply(Vector3 pos, int caculateIndex)
    {
        Multiply multiply = Instantiate(multiplyPrefab, transform.GetChild(1));
        multiply.transform.position = pos;
        while (caculateIndex == 1) caculateIndex = Random.Range(0, 3);
        multiply.caculateIndex = caculateIndex;
        multiply.ResetVariable();
        listMultiply.Add(multiply);
    }

    private void SpawnMultiply(Vector3 pos, int caculateIndex,int multiplyNumber)
    {
        Multiply multiply = Instantiate(multiplyPrefab, transform.GetChild(1));
        multiply.transform.position = pos;
        multiply.caculateIndex = caculateIndex;
        multiply.SetMultiplyNumber(multiplyNumber);
        listMultiply.Add(multiply);
    }

    private void SpawnObstacle(Vector3 pos)
    {
        Obstacle obstacle = Instantiate(obstalcePrefab, transform.GetChild(2));
        obstacle.transform.position = pos;
        listObstacle.Add(obstacle);
    }

    private Vector3 RandomPosition(float radius,float ratio)
    {
        Vector2 result = Vector3.zero;
        result.x = Random.Range(-sizeScreen.x, sizeScreen.x) * ratio;
        result.y = Random.Range(-sizeScreen.y, sizeScreen.y) * ratio;
        Collider2D hit = Physics2D.OverlapCircle(result, radius * 0.7f);
        if (hit != null) result = Vector2.zero;
        return result;
    }

    private float GetRatioScale(int dotAmount)
    {
        float cDot = dotAmount - 30.0f;
        float tDot = 65.0f - 30.0f;
        float ratio = Mathf.Lerp(0.6f, 1.0f, cDot / tDot);
        return ratio;
    }

    private int GetDotAmount()
    {
        int lvl = levelIndex;
        if (lvl < 4) return Random.Range(30, 35);
        else if (lvl < 15) return Random.Range(35, 40);
        else if (lvl < 25) return Random.Range(40, 45);
        else if (lvl < 35) return Random.Range(45, 50);
        else if (lvl < 45) return Random.Range(50, 55);
        else if (lvl < 55) return Random.Range(55, 60);
        else return Random.Range(60, 65);
    }

    private int GetDotColorAmount()
    {
        int lvl = levelIndex;
        if (lvl < 5) return 2;
        else if (lvl < 30) return 3;
        else if (lvl < 55) return 4;
        else return 5;
    }

    private int GetMultiplyAmount()
    {
        int lvl = levelIndex;
        if (lvl < 4) return Random.Range(1, 2);
        else if (lvl < 20) return Random.Range(2, 4);
        else if (lvl < 45) return Random.Range(2, 5);
        else return Random.Range(3, 5);
    }

    private int GetMultiplyIndex()
    {
        int lvl = levelIndex;
        if (lvl < 4) return Random.Range(0, 1);
        else return Random.Range(0, 3);
    }

    private int GetObstacleAmount()
    {
        int lvl = levelIndex;
        if (lvl < 6) return 0;
        else if (lvl < 20) return 1;
        else if (lvl < 40) return 2;
        else return 3;
    }
}
