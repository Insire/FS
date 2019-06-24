using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FS
{
    // Source: https://stackoverflow.com/questions/398518/how-to-implement-glob-in-c-sharp
    public static class Globbing
    {
        /// <summary>
        /// return a list of folders that matches some wildcard pattern, e.g.
        /// C:\p4\software\dotnet\tools\*\*.sln to get all tool solution files
        /// </summary>
        /// <param name="glob">pattern to match</param>
        /// <returns>all matching paths</returns>
        public static IEnumerable<string> GlobFolders(string glob)
        {
            foreach (string path in GlobFolders(PathHead(glob) + _dirSep, PathTail(glob)))
                yield return path;
        }

        /// <summary>
        /// return a list of files that matches some wildcard pattern, e.g.
        /// C:\p4\software\dotnet\tools\*\*.sln to get all tool solution files
        /// </summary>
        /// <param name="glob">pattern to match</param>
        /// <returns>all matching paths</returns>
        public static IEnumerable<string> GlobFiles(string glob)
        {
            foreach (string path in GlobFiles(PathHead(glob) + _dirSep, PathTail(glob)))
                yield return path;
        }

        /// <summary>
        /// uses 'head' and 'tail' -- 'head' has already been pattern-expanded
        /// and 'tail' has not.
        /// </summary>
        /// <param name="head">wildcard-expanded</param>
        /// <param name="tail">not yet wildcard-expanded</param>
        /// <returns></returns>
        public static IEnumerable<string> GlobFolders(string head, string tail)
        {
            if (PathTail(tail) == tail)
            {
                foreach (string path in Directory.GetDirectories(head, tail).OrderBy(s => s))
                {
                    yield return path;
                }
            }
            else
            {
                foreach (string dir in Directory.GetDirectories(head, PathHead(tail)).OrderBy(s => s))
                {
                    foreach (string path in GlobFolders(Path.Combine(head, dir), PathTail(tail)))
                    {
                        yield return path;
                    }
                }
            }
        }

        /// <summary>
        /// uses 'head' and 'tail' -- 'head' has already been pattern-expanded
        /// and 'tail' has not.
        /// </summary>
        /// <param name="head">wildcard-expanded</param>
        /// <param name="tail">not yet wildcard-expanded</param>
        /// <returns></returns>
        public static IEnumerable<string> GlobFiles(string head, string tail)
        {
            if (PathTail(tail) == tail)
            {
                foreach (string path in Directory.GetFiles(head, tail).OrderBy(s => s))
                {
                    yield return path;
                }
            }
            else
            {
                foreach (string dir in Directory.GetDirectories(head, PathHead(tail)).OrderBy(s => s))
                {
                    foreach (string path in GlobFiles(Path.Combine(head, dir), PathTail(tail)))
                    {
                        yield return path;
                    }
                }
            }
        }

        /// <summary>
        /// shortcut
        /// </summary>
        private static readonly char _dirSep = Path.DirectorySeparatorChar;

        /// <summary>
        /// return the first element of a file path
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>first logical unit</returns>
        private static string PathHead(string path)
        {
            // handle case of \\share\vol\foo\bar -- return \\share\vol as 'head'
            // because the dir stuff won't let you interrogate a server for its share list
            // FIXME check behavior on Linux to see if this blows up -- I don't think so
            if (path.StartsWith("" + _dirSep + _dirSep))
            {
                return path.Substring(0, 2) + path.Substring(2).Split(_dirSep)[0] + _dirSep + path.Substring(2).Split(_dirSep)[1];
            }

            return path.Split(_dirSep)[0];
        }

        /// <summary>
        /// return everything but the first element of a file path
        /// e.g. PathTail("C:\TEMP\foo.txt") = "TEMP\foo.txt"
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>all but the first logical unit</returns>
        private static string PathTail(string path)
        {
            if (!path.Contains(_dirSep))
            {
                return path;
            }

            return path.Substring(1 + PathHead(path).Length);
        }
    }
}
