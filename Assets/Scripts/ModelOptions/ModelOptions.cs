// <copyright file="ModelOptions.cs" company="ABB">
// Copyright (c) ABB. All rights reserved.
// </copyright>

namespace RK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public enum ModelOptionType
    {
        /// <summary>
        /// Joint jog option 
        /// </summary>
        JointJog,

        /// <summary>
        /// Safe move zone option
        /// </summary>
        SafeMoveZone
    }

    public class ModelOptions : MonoBehaviour, IModelOptionsReceiver
    {
        [SerializeField]
        private Dictionary<ModelOptionType, bool> modelOptions = new Dictionary<ModelOptionType, bool>();

        private IModelOptionsNotifier optionsListener = null;

        public void SetModelOptions(HashSet<ModelOptionType> modelOptions)
        {
            this.modelOptions.Clear();

            foreach (var option in modelOptions)
            {
                this.modelOptions.Add(option, false);
            }

            this.optionsListener = this.transform.root.GetComponentInChildren<IModelOptionsNotifier>(true);

            if (this.optionsListener != null)
            {
                this.optionsListener.OnModelOptionsDiscovered(this.modelOptions);
            }
        }

        void IModelOptionsReceiver.SetOption(ModelOptionType modelOption, bool optionEnabled)
        {
            this.modelOptions[modelOption] = optionEnabled;
            this.optionsListener.OnModelOptionsChanged(this.modelOptions);
        }
    }
}
