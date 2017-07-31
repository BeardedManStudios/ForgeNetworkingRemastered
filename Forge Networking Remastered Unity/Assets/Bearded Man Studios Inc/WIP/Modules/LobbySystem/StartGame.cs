using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
public void startGame (int Levelnum)
    { 
        Application.LoadLevel(Levelnum);
    }
}
