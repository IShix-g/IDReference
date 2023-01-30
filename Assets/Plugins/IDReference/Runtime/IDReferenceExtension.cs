#if UNITY_EDITOR
using System.Text;
using UnityEngine;

public static class IDReferenceExtension
{
    public static IDRef.IDReference ToIDReferenceEditorOnly(this string id)
    {
        foreach (var table in IDRef.IDReferenceProvider.GetTables())
        {
            var idReference = table.IDToIDReference(id);
            if (idReference.IsValid())
            {
                return idReference;
            }
        }
        Debug.LogError($"[IDReference] Is not ID : {id}");
        return default;
    }

    public static string ToIDReferenceValueEditorOnly(this string id)
    {
        var idReference = ToIDReferenceEditorOnly(id);
        if (idReference.IsValid())
        {
            return idReference.Name;
        }
        return string.Empty;
    }

    public static Encoding GetEncodingCode(this byte[] bytes )
    {
        const byte bEscape = 0x1B;
        const byte bAt = 0x40;
        const byte bDollar = 0x24;
        const byte bAnd = 0x26;
        const byte bOpen = 0x28;
        const byte bB = 0x42;
        const byte bD = 0x44;
        const byte bJ = 0x4A;
        const byte bI = 0x49;

        var len = bytes.Length;
        byte b1, b2, b3, b4;

        var isBinary = false;
        for (var i = 0; i < len; i++)
        {
            b1 = bytes[i];
            if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
            {
                isBinary = true;
                if (b1 == 0x00 && i < len - 1 && bytes[i + 1] <= 0x7F)
                {
                    return Encoding.Unicode;
                }
            }
        }
        if (isBinary)
        {
            return null;
        }

        var notJapanese = true;
        for (var i = 0; i < len; i++)
        {
            b1 = bytes[i];
            if (b1 == bEscape || 0x80 <= b1)
            {
                notJapanese = false;
                break;
            }
        }
        if (notJapanese)
        {
            return Encoding.ASCII;
        }

        for (var i = 0; i < len - 2; i++)
        {
            b1 = bytes[i];
            b2 = bytes[i + 1];
            b3 = bytes[i + 2];

            if (b1 == bEscape)
            {
                if (b2 == bDollar && b3 == bAt)
                {
                    //JIS_0208 1978
                    //JIS
                    return Encoding.GetEncoding(50220);
                }
                else if (b2 == bDollar && b3 == bB)
                {
                    //JIS_0208 1983
                    //JIS
                    return Encoding.GetEncoding(50220);
                }
                else if (b2 == bOpen && (b3 == bB || b3 == bJ))
                {
                    //JIS_ASC
                    //JIS
                    return Encoding.GetEncoding(50220);
                }
                else if (b2 == bOpen && b3 == bI)
                {
                    //JIS_KANA
                    //JIS
                    return Encoding.GetEncoding(50220);
                }
                if (i < len - 3)
                {
                    b4 = bytes[i + 3];
                    if (b2 == bDollar && b3 == bOpen && b4 == bD)
                    {
                        //JIS_0212
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    if (i < len - 5 &&
                        b2 == bAnd && b3 == bAt && b4 == bEscape &&
                        bytes[i + 4] == bDollar && bytes[i + 5] == bB)
                    {
                        //JIS_0208 1990
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                }
            }
        }

        var sjis = 0;
        var euc = 0;
        var utf8 = 0;
        for (var i = 0; i < len - 1; i++)
        {
            b1 = bytes[i];
            b2 = bytes[i + 1];
            if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
            {
                //SJIS_C
                sjis += 2;
                i++;
            }
        }
        for (var i = 0; i < len - 1; i++)
        {
            b1 = bytes[i];
            b2 = bytes[i + 1];
            if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
                (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
            {
                //EUC_C
                //EUC_KANA
                euc += 2;
                i++;
            }
            else if (i < len - 2)
            {
                b3 = bytes[i + 2];
                if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) &&
                    (0xA1 <= b3 && b3 <= 0xFE))
                {
                    //EUC_0212
                    euc += 3;
                    i += 2;
                }
            }
        }
        for (var i = 0; i < len - 1; i++)
        {
            b1 = bytes[i];
            b2 = bytes[i + 1];
            if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
            {
                //UTF8
                utf8 += 2;
                i++;
            }
            else if (i < len - 2)
            {
                b3 = bytes[i + 2];
                if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
                    (0x80 <= b3 && b3 <= 0xBF))
                {
                    //UTF8
                    utf8 += 3;
                    i += 2;
                }
            }
        }
        
        if (euc > sjis && euc > utf8)
        {
            //EUC
            return Encoding.GetEncoding(51932);
        }
        if (sjis > euc && sjis > utf8)
        {
            //SJIS
            return Encoding.GetEncoding(932);
        }
        if (utf8 > euc && utf8 > sjis)
        {
            return Encoding.UTF8;
        }

        return Encoding.Default;
    }
}
#endif