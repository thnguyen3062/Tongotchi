using UnityEngine;

public static class NumericUtils 
{
    public static string FormatNumber(long number)
    {
        if (number >= 1_000_000_000)
            return $"{number / 1_000_000_000.0:#.#}B";
        if (number >= 1_000_000)
            return $"{number / 1_000_000.0:#.#}M";
        if (number >= 1_000)
            return $"{number / 1_000.0:#.#}K";

        return number.ToString();
    }
}
