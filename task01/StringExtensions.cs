using System;

namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
                
            string lowerInput = input.ToLower();

            string cleanString = "";
            foreach (char c in lowerInput)
            {
                if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
                {
                    cleanString += c; 
                }
            }

            
            if (cleanString.Length == 0)
                return false;

            
            int left = 0;
            int right = cleanString.Length - 1;
            while (left < right)
            {
                if (cleanString[left] != cleanString[right])
                    return false;
                left++;
                right--;
            }
            return true;
        }
    }
}