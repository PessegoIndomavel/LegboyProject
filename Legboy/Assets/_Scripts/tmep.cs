using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmep : MonoBehaviour
{
    [System.Flags]
    public enum eGameState
    {
        // Decimal      // Binary
        none = 0,       // 00000000
        mainMenu = 1,   // 00000001
        preLevel = 2,   // 00000010
        level = 4,      // 00000100
        postLevel = 8,  // 00001000
        gameOver = 16,  // 00010000
        all = 0xFFFFFFF // 11111111111111111111111111111111
    }
    
    [EnumFlags] // This uses the EnumFlagsAttribute from EnumFlagsAttributePropertyDrawer
    public eGameState   activeStates = eGameState.all;
}
