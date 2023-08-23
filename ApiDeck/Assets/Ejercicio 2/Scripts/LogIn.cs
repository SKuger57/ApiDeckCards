using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Linq;
using System;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField userNameInputField, passwordInputField;
    public string apiUrl = "https://sid-restapi.onrender.com/api/";

    string token;

    [Header("Score list references")]
    [SerializeField] TextMeshProUGUI[] scoreUsernames;
    [SerializeField] TextMeshProUGUI[] scoreValues;

    public void Register()
    {
        AuthData authData = new AuthData();
        authData.username = userNameInputField.text;
        authData.password = passwordInputField.text;

        string json = JsonUtility.ToJson(authData);

        StartCoroutine(SendRegister(json));
    }

    public void Login()
    {
        AuthData authData = new AuthData();
        authData.username = userNameInputField.text;
        authData.password = passwordInputField.text;

        string json = JsonUtility.ToJson(authData);

        StartCoroutine(SendLogin(json));
    }

    IEnumerator SendRegister(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put($"{apiUrl}usuarios", json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.method = "POST";
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR" + request.error);
        }

        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Debug.Log("Usuario registrado con el ID " + data.usuario._id);
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator SendLogin(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put($"{apiUrl}auth/login", json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.method = "POST";
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR" + request.error);
        }

        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);

                token = data.token;
                PlayerPrefs.SetString("Token", token);

                Debug.Log("Inicio de sesion con el usuario " + data.usuario.username + " y su token " + data.token);

                StartCoroutine(RetrieveAndSetScores());
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator RetrieveAndSetScores()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{apiUrl}usuarios");
        www.SetRequestHeader("x-token", token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {
                Userlist jsonList = JsonUtility.FromJson<Userlist>(www.downloadHandler.text);
                Debug.Log(jsonList.usuarios.Count);

                foreach (User a in jsonList.usuarios)
                {
                    Debug.Log(a.username);
                }

                List<User> lista = jsonList.usuarios;
                List<User> listaOrdenada = lista.OrderByDescending(u => u.data.score).ToList<User>();

                //int puesto = 0;
                //foreach (User a in listaOrdenada)
                //{
                //    if (puesto > 4)
                //    {

                //    }
                //    else
                //    {
                //        string nombre = puesto + 1 + "." + "Usuario:" + a.username + ",Puntaje:" + a.data.score;
                //        Puestos[puesto].text = nombre;
                //        puesto++;

                //    }
                //}

                int len = scoreUsernames.Length;
                for (int i = 0; i < len; i++)
                {
                    scoreUsernames[i].text = listaOrdenada[i].username;
                    scoreValues[i].text = listaOrdenada[i].data.score.ToString();
                }
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }
        }
    }
}

[System.Serializable]
public class AuthData
{
    public string username, password, token;
    public UserData usuario;
}

[System.Serializable]
public class UserData
{
    public string _id, username;
    public int score;
    public bool estado;
}

[System.Serializable]
public class Userlist
{
    public List<User> usuarios;
}