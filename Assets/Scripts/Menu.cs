using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public InputField usernameInput;
    public Button goButton;


    // Start is called before the first frame update
    void Start()
    {
        var input = gameObject.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGoPress()
    {
        PlayerPrefs.SetString("username", usernameInput.text);
        PlayerPrefs.Save();


        Debug.Log("username " + PlayerPrefs.GetString("username"));
    }
}
