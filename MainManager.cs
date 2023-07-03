using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public float slices;
    public float lThresh;
    public float hThresh;
    public float trans;
    public float transP;
    public float contrast;
    public float contrastP;
    public float scale;
    public float shift;
    public float rotate;
    public float relSize;
    public float tileSize;
    public float separation;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/userInfo.dat");

        UserData data = new UserData();
        data.slices = slices;
        data.lThresh = lThresh;
        data.hThresh = hThresh;
        data.trans = trans;
        data.transP = transP;
        data.contrast = contrast;
        data.contrastP = contrastP;
        data.scale = scale;
        data.shift = shift;
        data.rotate = rotate;
        data.relSize = relSize;
        data.tileSize = tileSize;
        data.separation = separation;
        bf.Serialize(file, data);
        file.Close();

    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/userInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/userInfo.dat", FileMode.Open);
            UserData data = (UserData)bf.Deserialize(file);
            file.Close();
            slices = data.slices;
            lThresh = data.lThresh;
            hThresh = data.hThresh;
            trans = data.trans;
            transP = data.transP;
            contrast = data.contrast;
            contrastP = data.contrastP;
            scale = data.scale;
            shift = data.shift;
            rotate = data.rotate;
            relSize = data.relSize;
            tileSize = data.tileSize;
            separation = data.separation;
        }
    }

}

[Serializable]
class UserData
{
    public float slices;
    public float lThresh;
    public float hThresh;
    public float trans;
    public float transP;
    public float contrast;
    public float contrastP;
    public float scale;
    public float shift;
    public float rotate;
    public float relSize;
    public float tileSize;
    public float separation;
}
