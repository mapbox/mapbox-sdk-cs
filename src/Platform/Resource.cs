//-----------------------------------------------------------------------
// <copyright file="Resource.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Platform
{
    using System;
    using System.Collections.Generic;

    /// <summary> Abstract class representing a Mapbox resource URL. </summary>
    public abstract class Resource
    {
        /// <summary> Gets the API endpoint, which is a partial URL path. </summary>
        public abstract string ApiEndpoint { get; }

        /// <summary>Builds a complete, valid URL string.</summary>
        /// <returns>Returns URL string.</returns>
        public abstract string GetUrl();

        /// <summary>Gets a string from a List of options.</summary>
        /// <param name="list"> List of URL query options. </param>
        /// <returns>Returns URL query options string.</returns>
        protected static string GetOptsString(List<string> list)
        {
            string str = string.Empty;

            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    str += "?";
                }
                else 
                {
                    str += "&";
                }

                str += list[i];
            }

            return str;
        }

        /// <summary>Builds a string from an array of options for use in URLs.</summary>
        /// <param name="items"> Array of option strings. </param>
        /// <param name="separator"> Character to use for separating items in arry. Defaults to ",". </param>
        /// <returns>Comma-separated string of options.</returns>
        /// <typeparam name="U">Type in the array.</typeparam>
        protected static string GetUrlQueryFromArray<U>(U[] items, string separator = ",")
        {
            string str = string.Empty;

            for (int i = 0; i < items.Length; i++)
            {
                str = str + items[i].ToString() + (i + 1 == items.Length ? string.Empty : separator);
            }

            return str;
        }
    }
}
