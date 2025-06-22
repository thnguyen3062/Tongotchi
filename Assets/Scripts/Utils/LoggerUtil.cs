using System.Drawing;
using UnityEngine;

public enum TextColor
{
    Red,
    Yellow,
    Green,
    Gray,
    None,
}

public static class LoggerUtil 
{
    public static void Logging(string message, TextColor color)
    {
        Debug.Log($"<color={GetColor(color)}>{message}</color>");
    }

    public static void Logging(string title, string message = null, TextColor titleColor = TextColor.Green, TextColor textColor = TextColor.None)
    {
        Debug.Log($"<color={GetColor(titleColor)}>-----[{title}]-----</color>\nMessage: <color={GetColor(textColor)}>{message}</color>\n");
    }

    private static string GetColor(TextColor color)
    {
        return color switch
        {
            TextColor.Red => "red",
            TextColor.Yellow => "yellow",
            TextColor.Green => "green",
            TextColor.Gray => "grey",
            _ => "white"
        };
    }
}
