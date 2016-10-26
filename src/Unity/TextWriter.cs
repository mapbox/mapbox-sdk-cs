//-----------------------------------------------------------------------
// <copyright file="TextWriter.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Unity
{
    using UnityEngine;

    /// <summary>
    ///     Redirects System.Console to Unity's console.
    /// </summary>
    public class TextWriter : System.IO.TextWriter
    {
        internal TextWriter() : base()
        {
        }

        /// <summary> Gets the data encoding. </summary>
        /// <value> The data encoding. </value>
        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        /// <summary>
        ///     Register an instance of this class for redirecting
        ///     messages from Console.Write*() to Debug.Log().
        /// </summary>
        public static void Register()
        {
            System.Console.SetOut(new TextWriter());
        }

        /// <summary> Write the specified value. </summary>
        /// <param name="value"> Console message. </param>
        public override void Write(string value)
        {
            base.Write(value);
            Debug.Log(value);
        }
    }
}