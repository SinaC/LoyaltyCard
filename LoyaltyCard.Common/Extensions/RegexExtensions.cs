using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoyaltyCard.Common.Extensions
{
    //https://stackoverflow.com/questions/6005609/replace-only-some-groups-with-regex
    public static class RegexExtensions
    {
        public static string ReplaceGroup(this Regex regex, string input, string groupName, string replacement)
        {
            return regex.ReplaceGroup(input, groupName, s => s);
        }

        public static string ReplaceGroup(this Regex regex, string input, string groupName, Func<string, string> replacementFunc)
        {
            return regex.Replace(
                input,
                m =>
                {
                    Group group = m.Groups[groupName];
                    StringBuilder sb = new StringBuilder();
                    int previousCaptureEnd = 0;
                    foreach (Capture capture in group.Captures.Cast<Capture>())
                    {
                        int currentCaptureEnd = capture.Index + capture.Length - m.Index;
                        int currentCaptureLength = capture.Index - m.Index - previousCaptureEnd;
                        sb.Append(m.Value.Substring(previousCaptureEnd, currentCaptureLength));
                        string appliedReplacement = replacementFunc(group.Value);
                        sb.Append(appliedReplacement);
                        previousCaptureEnd = currentCaptureEnd;
                    }
                    sb.Append(m.Value.Substring(previousCaptureEnd));

                    return sb.ToString();
                });
        }
    }
}
