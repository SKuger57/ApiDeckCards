using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cards : MonoBehaviour
{
    public ApiCall manager;
    [SerializeField] private RawImage img;
    [SerializeField] private TMP_Text id, cardName;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        manager.charactersLoaded.AddListener(SetImage);
    }

    void SetImage()
    {
        manager.SetImage(index, img);
        id.text = manager.characters.ElementAt(index).id.ToString();
        cardName.text = manager.characters.ElementAt(index).name;
        
    }
}