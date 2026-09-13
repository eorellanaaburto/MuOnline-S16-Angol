using MU.Network.Game;
using System;
using System.Collections.Generic;
using System.Text;
using WebZen.Serialization;
using WebZen.Util;

namespace MU.Network.MuunSystem
{
    [WZContract]
    public class SMuunRideVP : IGameMessage
    {
        [WZMember(0, typeof(ArrayWithScalarSerializer<byte>))] 
        public MuunRideVPDto[] ViewPort { get; set; }
    }

    [WZContract]
    public class MuunRideVPDto
    {
        [WZMember(0)] public ushortle wzNumber { get; set; }
        [WZMember(1)] public ushortle wzMuunRideItem { get; set; }
        [WZMember(2)] public byte data { get; set; }
        [WZMember(3)] public byte junk { get; set; }

        public MuunRideVPDto()
        {

        }
        public MuunRideVPDto(ushortle Number, ushortle MuunItem, byte opt)
        {
            wzNumber = Number;
            wzMuunRideItem = MuunItem;
            data = opt;
        }
    }

    [WZContract(Serialized = true)]
    public class SMuunItemGet : IGameMessage
    {
        [WZMember(0)] public byte Result { get; set; }
        [WZMember(1, 12)] public byte[] Item { get; set; }
    }
}
