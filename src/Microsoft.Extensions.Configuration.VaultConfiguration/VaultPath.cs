using System.Linq;

namespace Microsoft.Extensions.Configuration.VaultConfiguration {

    public static class VaultPath {
        public static readonly string PathDelimiter = "/";

        public static string Combine(params string[] segments) {
            return string.Join(PathDelimiter, segments.Select(Clean));
        }

       public static string Clean(string path) {
            if (string.IsNullOrWhiteSpace(path)) {
                return path;
            }
            var trimChars = PathDelimiter.ToCharArray();
            return path.TrimStart(trimChars).TrimEnd(trimChars);
        }
    }

}
