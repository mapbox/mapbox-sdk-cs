//-----------------------------------------------------------------------
// <copyright file="IAsyncRequest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox
{
    /// <summary> A handle to an asynchronous request. </summary>
    public interface IAsyncRequest
    {
        /// <summary> Cancel the ongoing request, preventing it from firing a callback. </summary>
        void Cancel();
    }
}