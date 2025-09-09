using System.Text.RegularExpressions;
using System.Linq;

public static class NameRules
{
    // 한글/영문/숫자만 허용 (공백/특수문자/이모지 불가)
    static readonly Regex allowed = new Regex(@"^[가-힣a-zA-Z0-9]+$", RegexOptions.Compiled);

    // 금칙어 목록
    static readonly string[] blacklist = {
        "씨발","개새끼","보지","자지","Fuck","년","놈"
    };

    public static bool TryValidate(string name, out string failReason)
    {
        failReason = null;
        if (string.IsNullOrWhiteSpace(name)) { failReason = "이름을 입력하세요."; return false; }

        var trimmed = name.Trim();

        // 길이
        if (trimmed.Length < 2 || trimmed.Length > 8)
        {
            failReason = "이름 길이는 2~8자여야 합니다.";
            return false;
        }

        // 허용 문자
        if (!allowed.IsMatch(trimmed))
        {
            failReason = "공백/특수문자/이모지는 사용할 수 없습니다.";
            return false;
        }

        // 금칙어
        var lower = trimmed.ToLowerInvariant();
        if (blacklist.Any(b => lower.Contains(b.ToLowerInvariant())))
        {
            failReason = "금칙어가 포함되어 있습니다.";
            return false;
        }

        return true;
    }
}
