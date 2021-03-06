﻿using System.Collections.Generic;

namespace V2RayGCon.Models.VeeShareLinks
{
    public class BasicSettings
    {
        // max string length is 256 bytes

        public string alias, description;
        public string address;
        public int port; // 16 bit each

        public bool isUseTls, isSecTls;
        public string streamType, streamParam1, streamParam2, streamParam3;

        public BasicSettings()
        {
            alias = string.Empty;
            description = string.Empty;
            port = 0;
            address = string.Empty;

            isUseTls = false;
            isSecTls = true;

            streamType = string.Empty;
            streamParam1 = string.Empty;
            streamParam2 = string.Empty;
            streamParam3 = string.Empty;
        }

        #region protected 
        protected readonly List<string> strTable = new List<string>{
            "ws", "tcp", "kcp", "h2", "quic",
            "none", "srtp", "utp", "wechat-video",
            "dtls", "wireguard", "",
        };

        #endregion

        #region public methods

        #endregion

        public void CopyFrom(BasicSettings streamSettings)
        {
            if (streamSettings == null)
            {
                return;
            }

            alias = streamSettings.alias;
            description = streamSettings.description;
            port = streamSettings.port;
            address = streamSettings.address;

            isUseTls = streamSettings.isUseTls;
            isSecTls = streamSettings.isSecTls;
            streamType = streamSettings.streamType;
            streamParam1 = streamSettings.streamParam1;
            streamParam2 = streamSettings.streamParam2;
            streamParam3 = streamSettings.streamParam3;
        }

        public bool EqTo(BasicSettings target)
        {
            if (target == null
                || alias != target.alias
                || description != target.description
                || port != target.port
                || address != target.address
                || isUseTls != target.isUseTls
                || isSecTls != target.isSecTls
                || streamType != target.streamType
                || streamParam1 != target.streamParam1
                || streamParam2 != target.streamParam2
                || streamParam3 != target.streamParam3)
            {
                return false;
            }
            return true;
        }
    }
}
