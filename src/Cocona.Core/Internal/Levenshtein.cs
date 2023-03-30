namespace Cocona.Internal
{
    public static class Levenshtein
    {
        /// <summary>
        /// Get levenshtein distance of two string values.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static float GetDistance(string s1, string s2)
        {
            var dp = new float[s1.Length + 1, s2.Length + 1];
            for (var i = 0; i < s1.Length + 1; i++)
            {
                dp[i, 0] = i;
            }
            for (var i = 0; i < s2.Length + 1; i++)
            {
                dp[0, i] = i;
            }

            for (var i = 1; i < s1.Length + 1; i++)
            {
                for (var j = 1; j < s2.Length + 1; j++)
                {
                    var cost = s1[i - 1] == s2[j - 1] ? 0 : 1f;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 3f, dp[i, j - 1] + 0.25f), dp[i - 1, j - 1] + cost);
                }
            }

            return dp[s1.Length, s2.Length];
        }
    }
}
