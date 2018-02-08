using System.Collections.Generic;
using System.Data;

namespace FuelDataSysClient.Tool
{
    public class DataCheckVINHelper
    {
        private Dictionary<char, int> HgzCodeMap = new Dictionary<char, int>();
        private Dictionary<int, int> HgzIndexMap = new Dictionary<int, int>();
        private Dictionary<int, int> VinBitMap = new Dictionary<int, int>();
        public DataCheckVINHelper()
        {
            HgzCodeMap.Add('A', 1);
            HgzCodeMap.Add('B', 2);
            HgzCodeMap.Add('C', 3);
            HgzCodeMap.Add('D', 4);
            HgzCodeMap.Add('E', 5);
            HgzCodeMap.Add('F', 6);
            HgzCodeMap.Add('G', 7);
            HgzCodeMap.Add('H', 8);
            HgzCodeMap.Add('J', 1);
            HgzCodeMap.Add('K', 2);
            HgzCodeMap.Add('L', 3);
            HgzCodeMap.Add('M', 4);
            HgzCodeMap.Add('N', 5);
            HgzCodeMap.Add('P', 7);
            HgzCodeMap.Add('R', 9);
            HgzCodeMap.Add('S', 2);
            HgzCodeMap.Add('T', 3);
            HgzCodeMap.Add('U', 4);
            HgzCodeMap.Add('V', 5);
            HgzCodeMap.Add('W', 6);
            HgzCodeMap.Add('X', 7);
            HgzCodeMap.Add('Y', 8);
            HgzCodeMap.Add('Z', 9);

            HgzIndexMap.Add(1, 5);
            HgzIndexMap.Add(2, 4);
            HgzIndexMap.Add(3, 3);
            HgzIndexMap.Add(4, 2);
            HgzIndexMap.Add(6, 9);
            HgzIndexMap.Add(7, 8);
            HgzIndexMap.Add(8, 7);
            HgzIndexMap.Add(9, 6);
            HgzIndexMap.Add(10 ,5);
            HgzIndexMap.Add(11, 4);
            HgzIndexMap.Add(12, 3);
            HgzIndexMap.Add(13, 2);
            HgzIndexMap.Add(14, 8);
            HgzIndexMap.Add(15, 7);

            VinBitMap.Add(1, 8);
            VinBitMap.Add(2, 7);
            VinBitMap.Add(3, 6);
            VinBitMap.Add(4, 5);
            VinBitMap.Add(5, 4);
            VinBitMap.Add(6, 3);
            VinBitMap.Add(7, 2);
            VinBitMap.Add(8, 10);

            VinBitMap.Add(10, 9);
            VinBitMap.Add(11, 8);
            VinBitMap.Add(12, 7);
            VinBitMap.Add(13, 6);
            VinBitMap.Add(14, 5);
            VinBitMap.Add(15, 4);
            VinBitMap.Add(16, 3);
            VinBitMap.Add(17, 2);

        }

        const string wzhgzbhChar = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
        public bool CheckWzhgzbhCode(string wzhgzbh, out char bi)
        {
   
            bool checkeds = false;
            char bit5;
            char bit5CStr;
            int sum = 0;
            
            try
            {
                if (!string.IsNullOrEmpty(wzhgzbh) && wzhgzbh.Length == 15)
                {
                    bit5 = wzhgzbh[4];
                    for (int i = 0; i < 15; i++)
                    {
                        char c = wzhgzbh[i];
                        if (wzhgzbhChar.IndexOf(c) == -1)
                        {
                            break;
                        }
                        if (i != 4)
                        {
                            int bv = -1;
                            int r = -1;
                            if ("0123456789".IndexOf(c) == -1)
                            {
                                bv = HgzCodeMap[c];
                                r = HgzIndexMap[i + 1];
                            }
                            else
                            {
                                bv = int.Parse(c.ToString());
                                r = HgzIndexMap[i + 1];
                            }
                            sum += bv * r;
                        }
                    }
                    int bit5C = sum % 11;
                    
                    if (bit5C == 10)
                    {
                        bit5CStr = 'X';
                        if (bit5 == bit5CStr)
                        {
                            checkeds = true;
                        }
                        bi = 'X';
                    }
                    else
                    {
                        bit5CStr = char.Parse(bit5C.ToString());
                        if (bit5 == bit5CStr)
                        {
                            checkeds = true;
                        }
                        bi = char.Parse(bit5C.ToString());
                    }

                }
                else
                {
                    bi = '-';
                }
            }
            catch { bi = '-'; }
            
            return checkeds;
		
        }

        public bool CheckCLSBDH(string clsbdh, out char bi)
        {
            bool checkeds = false;
            char bit9CStr ;
            char bit9;
            int sum = 0;
            try
            {
                if (!string.IsNullOrEmpty(clsbdh) && clsbdh.Length == 17)
                {
                    if (clsbdh.ToUpper().IndexOf("L") != 0)
                    {
                        bi = 'J';
                        return true;
                    }

                    bit9 = clsbdh[8];
                    for (int i = 0; i < 17; i++)
                    {
                        char c = clsbdh[i];
                        if (wzhgzbhChar.IndexOf(c) == -1)
                        {
                            break;
                        }
                        if (i != 8)
                        {
                            int bv = -1;
                            int r = -1;
                            if ("0123456789".IndexOf(c) == -1)
                            {
                                bv = HgzCodeMap[c];
                                r = VinBitMap[i + 1];
                            }
                            else
                            {
                                bv = int.Parse(c.ToString());
                                r = VinBitMap[i + 1];
                            }
                            sum += bv * r;
                        }
                    }
                    int bit9C = sum % 11;
                    
                    if (bit9C == 10)
                    {
                        bit9CStr = 'X';
                        if (bit9 == bit9CStr)
                        {
                            checkeds = true;
                        }
                        bi = 'X';
                    }
                    else
                    {
                        bit9CStr = char.Parse(bit9C.ToString());
                        if (bit9 == bit9CStr)
                        {
                            checkeds = true;
                        }
                        bi = char.Parse(bit9C.ToString());
                    }
                }
                else
                {
                    bi = '-';
                }
            }
            catch { bi = '-'; }
            return checkeds;
        }

    }
}
