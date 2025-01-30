using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Buffers.Text;
using System.IO;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System;

public class airtable : MonoBehaviour
{
    // Airtable API information
    private string apiKey = "patJoVBHS8XCwyLGK.4bcef627416ad3eaf1bae3848e41eff881ba948917aa3e1368d2d2a07787cc45";
    private string baseID = "appitujjKf4JFBgiI";
    private string tableName = "Prices";

    private string apiUrl;

    public string name;

    public TMP_InputField inputField;
    public GameObject inputMenu;
    public GameObject buttonMenu;

    // Start is called before the first frame update
    void Start()
    {
        apiUrl = $"https://api.airtable.com/v0/{baseID}/{tableName}";
        // Start coroutine to fetch data from Airtable
        StartCoroutine(GetRecordsFromAirtable());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputMenu.activeInHierarchy)
        {
            name = inputField.text;
            EnterUser(inputField.text);
            Debug.Log(inputField.text);
        }
    }

    public class Airtable
    {
        public Fields fields;
    }

    public class Fields
    {
        public string Customer;
        public string Prices;
    }


    public void EnterUser(string _name)
    {
        StartCoroutine(CreateNewAirtableItem(_name));
    }

    public void EnterPrice(int _price)
    {
        StartCoroutine(AddNewItemPrice(_price));
    }

    public IEnumerator AddNewItemPrice(int _price)
    {
        int currPrice = 0;

        //using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        //{
        //    request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        //    request.SetRequestHeader("Content-Type", "application/json");

        //    yield return request.SendWebRequest();

        //    if (request.result == UnityWebRequest.Result.Success) ;
        //}


            Airtable newOrder = new Airtable
            {
                fields = new Fields {
                    Customer = name,
                    Prices = "Fifty"
                }
            };

        string jsonData = JsonConvert.SerializeObject(newOrder);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        //Test comment

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Record successfully updated: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error updating record: " + request.responseCode + " - " + request.downloadHandler.text);
            }
        }
    }

    public IEnumerator CreateNewAirtableItem(string _name)
    {
        Airtable airtable = new Airtable
        {
            fields = new Fields
            {
                Customer = _name,
                Prices = "zero"
            }
        };

        string jsonData = JsonConvert.SerializeObject(airtable);


        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // Wait for the response
            yield return webRequest.SendWebRequest();

            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + apiKey);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("New record added successfully!");
                    Debug.Log("Response: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Error adding record: " + request.error);
                    Debug.LogError("Response: " + request.downloadHandler.text);
                }

                buttonMenu.SetActive(true);
                inputMenu.SetActive(false);
            }
        }
    }

    // Function to GET records from Airtable
    IEnumerator GetRecordsFromAirtable()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // Wait for the response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }
}
