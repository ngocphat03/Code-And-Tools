using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Random = UnityEngine.Random;

public class JsonFileSaveAndLoadTemplate : MonoBehaviour
{
    ListModel _listModel = new ();
    public void Test()
    {
        this.LoadDataFromFile();
        _listModel.PlayerModel.Add(new Model
        {
            id = Random.Range(1, 100),
            name = "AXit",
            age = 18,
            sex = "Male"
        });

        this.SaveDataToFile();
    }
    public void SaveDataToFile()
    {
        try
        {
            //Save data to directory
            var serializer = new JsonSerializer();
            using var sw = new StreamWriter(Application.persistentDataPath + "/dataUser.json");
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, _listModel);
        }
        catch 
        { 
            //Create empty data when there is no file
            this.CreateEmptyFile(); 
        }
    }

    public void LoadDataFromFile()
    {
        try
        {
            //Get data from directory and assign it to model
            var outputJson = File.ReadAllText(Application.persistentDataPath + "/dataUser.json");
            var loadedUserData = JsonConvert.DeserializeObject<ListModel>(outputJson);
            _listModel.PlayerModel = loadedUserData.PlayerModel;
        }
        catch
        {
            //Create empty data when there is no file
            this.CreateEmptyFile();
        }
    }

    public void CreateEmptyFile()
    {
        ListModel __listModel = new();
        var serializer = new JsonSerializer();
        using var sw = new StreamWriter(Application.persistentDataPath + "/dataUser.json");
        using JsonWriter writer = new JsonTextWriter(sw);
        serializer.Serialize(writer, __listModel);
    }
}

public class Model
{
    public int id;
    public string name;
    public int age;
    public string sex;
}

public class ListModel
{
    public List<Model> PlayerModel = new List<Model>();
}
