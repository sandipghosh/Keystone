
namespace Keystone.Web.Utilities.PaymentGetway
{
    using System;
    using System.Collections.Specialized;
    using System.Text;
    using System.Web;

    public class NVPCodec : NameValueCollection
    {
        private const string AMPERSAND = "&";
        private const string EQUALS = "=";
        private static readonly char[] AMPERSAND_CHAR_ARRAY = AMPERSAND.ToCharArray();
        private static readonly char[] EQUALS_CHAR_ARRAY = EQUALS.ToCharArray();

        /// <summary>
        /// Encodes this instance.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            bool firstPair = true;
            foreach (string kv in AllKeys)
            {
                string name = HttpUtility.UrlEncode(kv);
                string value = this[kv].StartsWith("http") ? this[kv] : HttpUtility.UrlEncode(this[kv]);
                if (!firstPair)
                {
                    sb.Append(AMPERSAND);
                }
                sb.Append(name).Append(EQUALS).Append(value);
                firstPair = false;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Decodes the specified nvpstring.
        /// </summary>
        /// <param name="nvpstring">The nvpstring.</param>
        public void Decode(string nvpstring)
        {
            Clear();
            foreach (string nvp in nvpstring.Split(AMPERSAND_CHAR_ARRAY))
            {
                string[] tokens = nvp.Split(EQUALS_CHAR_ARRAY);
                if (tokens.Length >= 2)
                {
                    string name = HttpUtility.UrlDecode(tokens[0]);
                    string value = HttpUtility.UrlDecode(tokens[1]);
                    Add(name, value);
                }
            }
        }

        public void Decode(NameValueCollection nvpCollection)
        {
            Clear();

            foreach (var nvp in nvpCollection.AllKeys)
            {
                string name = HttpUtility.UrlDecode(nvp);
                string value = HttpUtility.UrlDecode(nvpCollection[nvp]);
                Add(name, value);
            }
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public void Add(string name, string value, int index)
        {
            this.Add(GetArrayName(index, name), value);
        }

        /// <summary>
        /// Removes the specified array name.
        /// </summary>
        /// <param name="arrayName">Name of the array.</param>
        /// <param name="index">The index.</param>
        public void Remove(string arrayName, int index)
        {
            this.Remove(GetArrayName(index, arrayName));
        }

        /// <summary>
        /// Gets or sets the <see cref="string" /> with the specified name.
        /// </summary>
        /// <value></value>
        public string this[string name, int index]
        {
            get
            {
                return this[GetArrayName(index, name)];
            }
            set
            {
                this[GetArrayName(index, name)] = value;
            }
        }

        /// <summary>
        /// Gets the name of the array.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static string GetArrayName(int index, string name)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index cannot be negative : " + index);
            }
            return name + index;
        }
    }
}