// <copyright file="IModelOptionsNotifier.cs" company="ABB">
// Copyright (c) ABB. All rights reserved.
// </copyright>

namespace RK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface IModelOptionsNotifier
    {
        /// <summary>
        /// Model options discovery event
        /// </summary>
        /// <param name="modelOptions">List of options</param>
        void OnModelOptionsDiscovered(Dictionary<ModelOptionType, bool> modelOptions);

        /// <summary>
        /// Model options has changed
        /// </summary>
        /// <param name="modelOptions"></param>
        void OnModelOptionsChanged(Dictionary<ModelOptionType, bool> modelOptions);
    }
}
