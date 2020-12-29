namespace BetterTTD.FOAN

open System

module Network =
    
    type Packet = {
        Size : uint16
        Position : int
        Buffer : byte array
    }
    
    let private defaultSize = 2us
    let private defaultPos = 0
    let private defaultArray = Array.zeroCreate<byte> 1460
    
    
    let readU16 packet =
        let { Size = _; Position = position; Buffer = buffer } = packet;
        ( BitConverter.ToUInt16 (buffer, (position) - 2),
          { packet with Position = position + 2 } )

    let readU32 packet =
        let { Size = _; Position = position; Buffer = buffer } = packet;
        ( BitConverter.ToUInt32 (buffer, (position) - 4),
          { packet with Position = position + 4 } )
        
    let readU64 packet =
        let { Size = _; Position = position; Buffer = buffer } = packet;
        ( BitConverter.ToUInt64 (buffer, (position) - 8),
          { packet with Position = position + 8 } )
        
    let readI64 packet =
        let { Size = _; Position = position; Buffer = buffer } = packet;
        ( BitConverter.ToInt64 (buffer, (position) - 8),
          { packet with Position = position + 8 } )

    let readByte packet =
        let { Size = _; Position = position; Buffer = buffer } = packet
        ( BitConverter.ToBoolean (buffer, position),
          { packet with Position = position + 1 } )
    
    let createPacket =
        { Size = defaultSize; Position = defaultPos; Buffer = defaultArray }

    let createPacketWithBuf buf =
        let (size, pac) = readU16 {createPacket with Buffer = buf }
        { pac with Size = size }
