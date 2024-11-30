using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ‹¤’Êˆ—‚ğ‚Ü‚Æ‚ß‚½•Ö—˜ƒNƒ‰ƒX
public static class Utils
{
    // •b”‚ğ0:00‚Ì•¶š—ñ‚É•ÏŠ·
    public static string GetTextTimer(float timer)
    {
        int seconds = (int)timer % 60;
        int minutes = (int)timer / 60;
        return minutes.ToString() + ":" + seconds.ToString("00");
    }


}
