using UnityEngine;

public static class PlayerCharacterData
{
    public static bool IsFemale()
    {
        int index = PlayerPrefs.GetInt("JugadorIndex");

        //  0 = masculino | 1 = femenino
        return index == 1;
    }


}


