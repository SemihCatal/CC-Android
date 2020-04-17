﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static HttpRequest;

namespace PrimeTech.Core
{
    public class AddObject : MonoBehaviour
    {
        int index = 0;
        public GameObject itemTemplate;
        public GameObject content;
        public GameObject downloaded;

        private WelcomeController welcome;
        public int userId;

        [SerializeField]
        private string mediaPath = "C://Users//User//Desktop//test2.json";
        [SerializeField]
        private List<Media> mediaList;

        private void loadMedia()
        {
            /*string url = "http://localhost:64021/mediaItems/"+userId;
            byte[] array = null;
            string downloadData;
            HttpResponseHandler myHandler1 = (int statusCode, string responseText, byte[] responseData) =>
            {
                if (statusCode == 200)
                {
                    if (responseData != null)
                    {
                        downloadData = Encoding.UTF8.GetString(responseData, 0, responseData.Length);
                        mediaList = JsonConvert.DeserializeObject<List<Media>>(downloadData);
                        foreach (var item in mediaList)
                        {
                           // Debug.Log(item.name);
                            addItem(item.name, item.tumbnail, item.id, item.type);
                        }
                    }
                }
            };
            HttpRequest.Send(this, "GET", url, null, array, myHandler1);*/
           
            using (StreamReader r = new StreamReader(mediaPath))
            {
                string json = r.ReadToEnd();
                mediaList = JsonConvert.DeserializeObject<List<Media>>(json);
                foreach (var item in mediaList)
                {
                    Debug.Log(item.name);
                    addItem(item.name, item.tumbnail, item.id, item.type);
                }
            } 
            
        }
        void Start()
        {
            /*welcome = GameObject.FindObjectOfType<WelcomeController>();
            userId = int.Parse(welcome.id.text);
            Debug.Log(userId);*/
            loadMedia();
        } 

        public void addItem(string name, string image, string id, string type)
        {
            /*AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
            string path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", jc.GetStatic<string>("DIRECTORY_DCIM")).Call<string>("getAbsolutePath");
            path = Path.Combine(path, "CommunicaptionMedias");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string path2 = path + name;
            if (type == "image")
            {
                path2 = path2 + ".jpg";
            }
            else if (type == "video")
            {
                path2 = path2 + ".mp4";
            }
            if (!File.Exists(path2))
            {
                downloaded.GetComponent<Image>().enabled = true;
            }
            else
            {
                downloaded.GetComponent<Image>().enabled = false;
            }*/

            var copy = Instantiate(itemTemplate);
            copy.transform.parent = content.transform;
            copy.transform.localPosition = Vector3.zero;

            copy.GetComponentInChildren<Text>().text = name;
            int copyOfIndex = index;
            
            byte[] imageBytes = Convert.FromBase64String(image);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            copy.GetComponent<Image>().sprite = sprite;

            copy.GetComponent<Button>().onClick.AddListener(() => { 
                Debug.Log("Index number " + mediaList[copyOfIndex].name + copyOfIndex);
                downloadMedia(copyOfIndex);
            });
            index++;
        }
        private void downloadMedia(int i)
        {
            string url = "http://localhost:64021/media/"+ userId + "/" + mediaList[i].id;
            byte[] array = null;
            HttpResponseHandler myHandler1 = (int statusCode, string responseText, byte[] responseData) =>
            {
                if (statusCode == 200)
                {
                    if (responseData != null)
                    {
                        byte[] itemBGBytes = responseData;
                        AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
                        string path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", jc.GetStatic<string>("DIRECTORY_DCIM")).Call<string>("getAbsolutePath");
                        path = Path.Combine(path, "CommunicaptionMedias");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string path2 = path + mediaList[i].name;
                        if (mediaList[i].type == "image"){
                            path2 = path2 + ".jpg";
                        }
                        else if (mediaList[i].type == "video")
                        {
                            path2 = path2 + ".mp4";
                        }
                        if (!File.Exists(path2))
                        {
                            File.WriteAllBytes(path2, itemBGBytes);
                            showAndroidToastMessage("Media saved successfully.");
                        }
                    }
                }
            };
            HttpRequest.Send(this, "GET", url, null, array, myHandler1);
        }
        private void showAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}