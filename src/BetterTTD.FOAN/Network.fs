namespace BetterTTD.FOAN

open System
open System.Text

module Network =
    
    type Packet = {
        Size : uint16
        Position : int
        Buffer : byte array
    }
    
    let private defaultSize = 2us
    let private defaultPos = 0
    let private defaultBuf = Array.zeroCreate<byte> 1460
    
    
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
        ( buffer.[position], { packet with Position = position + 1 } )
    
    let readBool packet =
        let (byte, pac) = readByte packet
        (byte <> 0uy, pac )
    
    let readString packet =
        let { Size = _; Position = position; Buffer = buffer } = packet
        
        let rec read (bytes : byte array, pos, buf : byte array) =
            if pos < buf.Length && buf.[pos] <> 0uy then
                let byte = [| buf.[pos] |]
                read (Array.concat [bytes; byte], pos + 1, buf)
            else
                (bytes, pos, buf)
            
        let (bytes, pos, _) = read (Array.zeroCreate<byte> 0, position, buffer)
        (Encoding.Default.GetString bytes, { packet with Position = pos })
    
    let createPacket =
        { Size = defaultSize
          Position = defaultPos
          Buffer = defaultBuf }

    let createPacketWithBuf buf =
        let (size, pac) = readU16 {createPacket with Buffer = buf }
        { pac with Size = size }
