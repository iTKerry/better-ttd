﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Network
{
    public class Protocol
    {
        public int Version { get; set; } = -1;
        protected Dictionary<AdminUpdateType, ArrayList> supportedFrequencies;

        public Protocol()
        {
            supportedFrequencies = new Dictionary<AdminUpdateType, ArrayList>();
        }

        public void addSupport(int typeIndex, int freqIndex)
        {
            AdminUpdateFrequency freq = (AdminUpdateFrequency)freqIndex;
            AdminUpdateType type = (AdminUpdateType)typeIndex;

            if (supportedFrequencies.Keys.Contains(type) == false)
            {
                supportedFrequencies.Add(type, new ArrayList());
            }
            supportedFrequencies[type].Add(freq);

        }

        public bool isSupported(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            return supportedFrequencies[type].Contains(freq);
        }
    }
}