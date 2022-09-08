using System.Text;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    /*
     *this is helper class is responsible generate nanoid.
     *GenericId
     *GenerateDeviceId
     *GenerateOrganisationId
     *Can be configured by parameter;
     */
    public static class IdHelper
    {
        private const string ALPHABET = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NUMBER = "0123456789";
        private const string ID_SEPARATOR = "::";

        public static string NewNanoId(int size = 7) => Nanoid.Nanoid.Generate(ALPHABET, size);
        public static string NewNumberNanoId(int size = 7) => Nanoid.Nanoid.Generate(NUMBER, size);

        public static T GenerateGenericId<T>(int size = 7)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)NewNumberNanoId(size);
            if (typeof(T) == typeof(string))
                return (T)(object)NewNanoId(size);
            return default(T);
        }

        public static int GenerateDeviceId(int size = 7) => GenerateGenericId<int>(size);
        public static int GenerateOrganisationId(int size = 7) => GenerateGenericId<int>(size);

        /// <summary>
        /// Hash ID
        /// </summary>
        /// <param name="prefix">To be added before hashed ID</param>
        /// <param name="combinations">Combinations of strings for hashing</param>
        /// <returns>Hashed ID of 7 Characters</returns>
        public static string GenerateHashId(string prefix = "", params string[] combinations)
        {
            //Update this with new Hashing Algorithm
            string combinationsString = "";
            foreach(var combination in combinations)
            {
                combinationsString += $"{ID_SEPARATOR}{combination}";
            }
            var hashedId = Nanoid.Nanoid.Generate(alphabet: combinationsString, size: 7);
            return $"{prefix}{hashedId}";
        }

        /// <summary>
        /// Split ID and get the whole ID except Splitter Counter at the end
        /// Splits where "::" is found
        /// </summary>
        /// <param name="id">ID string to be splitted</param>
        /// <returns>Returns the whole ID except Splitter Counter at the end</returns>
        public static string IdSplitter(string id)
        {
            var splittedId = id.Split($"{ID_SEPARATOR}");
            return $"{splittedId[0]}{ID_SEPARATOR}{splittedId[1]}";
        }

        /// <summary>
        /// Get the splitter counter at the end of an ID
        /// Splits where "::" is found
        /// </summary>
        /// <param name="id">ID string to be splitted</param>
        /// <returns>Returns the splitter counter at the end of an ID</returns>
        public static string IdSplitterCounter(string id)
        {
            return id.Split($"{ID_SEPARATOR}").LastOrDefault();
        }

        /// <summary>
        /// Get the array of splitted ID
        /// Splits where "::" is found
        /// </summary>
        /// <param name="id">ID string to be splitted</param>
        /// <returns>Returns the array of splitted ID</returns>
        public static string[] IdSplitterArray(string id)
        {
            return id.Split(ID_SEPARATOR);
        }

        /// <summary>
        /// Hash string with MD5
        /// </summary>
        /// <param name="s">String to be hashed</param>
        /// <returns>Hashed String</returns>
        public static string Md5(string s)
        {
            using var provider = System.Security.Cryptography.MD5.Create();
            StringBuilder builder = new StringBuilder();

            foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }
    }
}
