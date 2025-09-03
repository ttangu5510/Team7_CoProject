using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace JYL
{
    public static class Util
    {
        private static TimeZoneInfo kst;

        private static TimeZoneInfo KST
        {
            get
            {
                if (kst != null) return kst;
                try { kst = TimeZoneInfo.FindSystemTimeZoneById("Asia/Seoul"); return kst; }   // IANA
                catch { }
                try { kst = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"); return kst; } // Windows
                catch { }
                // KST는 1988년 이후 DST 없음 → 고정 오프셋 폴백 OK
                kst = TimeZoneInfo.CreateCustomTimeZone("KST", TimeSpan.FromHours(9), "KST", "KST");
                return kst;
            }
        }

        // "o" 포멧 형식으로 저장된 UTC 문자열 파싱
        private static DateTime ParseUtcString(string utcStr)
        {
            return  DateTime.Parse(utcStr, null, DateTimeStyles.RoundtripKind);
        }

        public static string UtcToKst(string utcStr, string format = "G")
        {
            var utc = ParseUtcString(utcStr);
            var kst = TimeZoneInfo.ConvertTimeFromUtc(utc, KST);
            return kst.ToString(format);
        }
    }
}


