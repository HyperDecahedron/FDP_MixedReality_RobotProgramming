// <copyright file="IModelOptionsReceiver.cs" company="ABB">
// Copyright (c) ABB. All rights reserved.
// </copyright>

namespace RK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface IModelOptionsReceiver
    {
        void SetOption(ModelOptionType modelOption, bool value);
    }
}
