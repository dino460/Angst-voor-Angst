using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Profile
{
    private int chosenProfile;

    public Profile(int pChosenProfile)
    {
        chosenProfile = pChosenProfile;
    }

    public int getChosenProfile()
    {
        return chosenProfile;
    }
}
