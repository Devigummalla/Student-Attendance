namespace Student_Att.Security
{
    public static class SecureCompare
    {
        // Compares two byte arrays.
        // It always checks every byte (no early "return false"), so an attacker
        // cannot learn how many bytes were correct by measuring the time taken.
        public static bool AreEqual(byte[] first, byte[] second)
        {
            if (first == null || second == null || first.Length != second.Length)
            {
                return false;
            }

            int difference = 0;
            for (int i = 0; i < first.Length; i++)
            {
                difference = difference | (first[i] ^ second[i]);
            }

            return difference == 0;
        }
    }
}
