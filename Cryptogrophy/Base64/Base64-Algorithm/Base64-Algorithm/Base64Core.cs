using System;

namespace KnowledgeBase.Crytography.Base64
{
    /// <summary>
    /// This is the Base64 algorithm implementation with source code
    /// </summary>
    /// <remarks>
    /// Base64 is a 6-bits based encoding method, since 64 == 2^6, so it is a 6 bit based encoding, since each 6-bit will occupied as an independent 8-bit byte, the encoded byte length will be 4/3 times than the original byte length
    /// </remarks>
    public class Base64Algorithm
    {
        /// <summary>
        /// Base64 Encoder
        /// </summary>
        /// <remarks>
        /// Base64 encoding is to transfer 8-bit bytes array to 6-bit bytes array, so every 3 8-bit bytes will be transfered to 4 6-bit blocks
        /// </remarks>
        /// <param name="data">The un-encoded byte array of the object</param>
        /// <returns>a character array of Base64 encoded object</returns>
        public static char[] Base64Encoding(byte[] data)
        {
            int length, length2;
            int blockCount;
            int paddingCount;

            length = data.Length;   //length is the length of the byte array being encoded

            if ((length % 3) == 0)  //no padding needed
            {
                paddingCount = 0;
                blockCount = length / 3;    //number of 8-bit bytes
            }
            else
            {
                paddingCount = 3 - (length % 3);    
                blockCount = (length + paddingCount) / 3;
            }

            length2 = length + paddingCount;    //length2 is the final length of the byte array being encoded

            byte[] source2; //source2 is the final byte array being encoded
            source2 = new byte[length2];

            for (int x = 0; x < length2; x++)   //add 0 as padding into the final byte array being encoded
            {
                if (x < length)
                {
                    source2[x] = data[x];
                }
                else
                {
                    source2[x] = 0;
                }
            }

            byte b1, b2, b3;
            byte temp, temp1, temp2, temp3, temp4;
            byte[] buffer = new byte[blockCount * 4];
            char[] result = new char[blockCount * 4];

            for (int x = 0; x < blockCount; x++)
            {
                b1 = source2[x * 3];    // b1, b2, b3 are 3 8-bit bytes to be encoded into 4 6-bit bytes Base64 code
                b2 = source2[x * 3 + 1];
                b3 = source2[x * 3 + 2];

                temp1 = (byte)((b1 & 252) >> 2);    // temp1 is the first 6-bit of the 4 Base64 bytes, 252 = 1111 1100, used to get the high 6-bit of b1, then right shift 2 bits to make it a 6-bit block

                temp = (byte)((b1 & 3) << 4);   //temp is used to get the last 2-bit of b1, 3 == 11 and left shift 4 bits to make it a 6-bit block
                temp2 = (byte)((b2 & 240) >> 4);    //temp2 is the second 6-bit of the 4 Base64 bytes, 240 == 1111 0000, used to the the high 4-bit of b2, then right shift 4 bits and combine with the first 2 bits of temp
                temp2 += temp;

                temp = (byte)((b2 & 15) << 2);  //temp is now used ot get the last 4-bit of b2, 15 == 1111
                temp3 = (byte)((b3 & 192) >> 6);    //temp3 is the 3rd 6-bit of the 4 Base64 bytes, 192 == 1100 0000, right shift 6 bits to combine with the first 4 bits of temp
                temp3 += temp;

                temp4 = (byte)(b3 & 63);    // temp4 is the last 4th 6-bit block of the 4 Base64 bytes, 63 = 0011 1111

                buffer[x * 4] = temp1;  //put the 4 Base64 at this block into the final bytes array, buffer[] is an array of 8-bit bytes, so here a 6-bit block gets converted back to 8-bit byte
                buffer[x * 4 + 1] = temp2;
                buffer[x * 4 + 2] = temp3;
                buffer[x * 4 + 3] = temp4;

            }

            for (int x = 0; x < blockCount * 4; x++)
            {
                result[x] = SixBitToChar(buffer[x]);    //convert from byte[] to char[] for display and network transfer
            }

            switch (paddingCount)   //insert character '=' as padding, maximum padding is 2 '='
            {
                case 0:
                    break;
                case 1:
                    result[blockCount * 4 - 1] = '=';
                    break;
                case 2:
                    result[blockCount * 4 - 1] = '=';
                    result[blockCount * 4 - 2] = '=';
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Base64 Decoder
        /// </summary>
        /// <remarks>
        /// Base64 decoding is to transfer 6-bit bytes array back to 8-bit bytes array, so every 4 6-bit block will be transfered to 3 8-bit bytes
        /// </remarks>
        /// <param name="data">The Base64 encoded character array</param>
        /// <returns>The original un-encoded object byte array</returns>
        public static byte[] Base64Decoding(char[] data)
        {
            int length, length2, length3;
            int blockCount;
            int paddingCount = 0;

            length = data.Length;   //length is Base64 code length
            blockCount = length / 4;    //blockCount is the 6-bit block count
            length2 = blockCount * 3;   //length2 is the 8-bit block count

            for (int x = 0; x < 2; x++) //check whether there are paddings, maximum paddings are 2
            {
                if (data[length - x - 1] == '=')
                    paddingCount++;
            }

            byte[] buffer = new byte[length];   //buffer is used to stored Base64 code
            byte[] buffer2 = new byte[length2]; //buffer2 is used to stored the original un-encoded 8-bit code

            for (int x = 0; x < length; x++)    //convert the Base64 character array back to 8-bit byte array (with only 6-bit valid information)
            {
                buffer[x] = CharToSixBit(data[x]);
            }

            byte b, b1, b2, b3; //3 un-encoded 8-bit bytes
            byte temp1, temp2, temp3, temp4;    //4 Base64 6-bit bytes

            for (int x = 0; x < blockCount; x++)
            {
                temp1 = buffer[x * 4];
                temp2 = buffer[x * 4 + 1];
                temp3 = buffer[x * 4 + 2];
                temp4 = buffer[x * 4 + 3];

                b = (byte)(temp1 << 2); //first 6 bits left shifted 2 bits to become a 8-bit bytes
                b1 = (byte)((temp2 & 48) >> 4); //48 == 110000, get the first 2-bit from 2nd Base64 6-bit block, right shifted 2 bits in order to combine with the high 6 bits of b
                b1 += b;

                b = (byte)((temp2 & 15) << 4);  //15 = 001111, get the last 4-bit from 2nd Base64 6-bit block
                b2 = (byte)((temp3 & 60) >> 2); //60 === 111100, get the first 4-bit from 3rd Base64 6-bit block
                b2 += b;

                b = (byte)((temp3 & 3) << 6);   //3 == 11, get the last 2-bit from 3rd Base64 6-bit block
                b3 = temp4; //combined with 4th Base64 6-bit block
                b3 += b;

                buffer2[x * 3] = b1;
                buffer2[x * 3 + 1] = b2;
                buffer2[x * 3 + 2] = b3;
            }

            length3 = length2 - paddingCount;   //remove padding and construct the final 8-bit un-encoded 8-bit byte array
            byte[] result = new byte[length3];

            for (int x = 0; x < length3; x++)
            {
                result[x] = buffer2[x];
            }

            return result;
        }

        /// <summary>
        /// Lookup Base64 character codes based on 6-bit value
        /// </summary>
        /// <param name="b"></param>
        /// <returns>a 8-bit character</returns>
        private static char SixBitToChar(byte b)
        {
            char[] lookupTable = new char[64] {
        'A','B','C','D','E','F','G','H','I','J','K','L','M',
        'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        'a','b','c','d','e','f','g','h','i','j','k','l','m',
        'n','o','p','q','r','s','t','u','v','w','x','y','z',
        '0','1','2','3','4','5','6','7','8','9','+','/'
    };

            if ((b >= 0) && (b <= 63))
            {
                return lookupTable[(int)b];
            }
            else
            {
                return ' '; //in theory this condition will be never hit
            }
        }

        /// <summary>
        /// Covert Base64 character code back to a 8-bit byte (since it's Base64 char so only has 6-bit valid information)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static byte CharToSixBit(char c)
        {
            char[] lookupTable = new char[64] {
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
        'O','P','Q','R','S','T','U','V','W','X','Y', 'Z',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n',
        'o','p','q','r','s','t','u','v','w','x','y','z',
        '0','1','2','3','4','5','6','7','8','9','+','/'
    };

            if (c == '=')
            {
                return 0;
            }
            else
            {
                for (int x = 0; x < 64; x++)
                {
                    if (lookupTable[x] == c)
                        return (byte)x;
                }

                return 0;
            }
        }
    }
}
