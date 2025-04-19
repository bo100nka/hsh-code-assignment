using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DataViewer.Data.Entities;
using DataViewer.Data.Exceptions;
using DataViewer.Interfaces;

namespace DataViewer.AppLogic
{
    /// <summary>
    /// A specific variant of a data parser, namely for JSON file that represents <see cref="BooksLibrary"/> data structure.
    /// </summary>
    public class BooksLibraryJsonFileParser : IDataParser<BooksLibrary>, IDisposable
    {
        private readonly FileInfo _sourceFileInfo;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;

        /// <summary>
        /// Constructs a new instance of the parser.
        /// Plausible defaults have been assumed about JSON parsing options: AllowTrailCommas, comment skipping an property name is case agnostic.
        /// </summary>
        /// <param name="booksFileFullPath"></param>
        /// <exception cref="ArgumentException">When unable to instantiate <see cref="FileInfo"/> out of the <paramref name="booksFileFullPath"/> value.</exception>
        public BooksLibraryJsonFileParser(string booksFileFullPath)
        {
            _sourceFileInfo = ValidateFilePath(booksFileFullPath);

            // one can argue what the plausible defaults for handling JSON files may be, but since
            // it was not defined from the beginning, i simply chose some plausible defaults
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true, // while useless during deserialization, this may be useful when serializer options are used during debug
                PropertyNameCaseInsensitive = true,
            };
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        /// <summary>
        /// Attempts to parse the JSON file name the parser was constructed with.
        /// </summary>
        /// <returns>A valid instance of <see cref="BooksLibrary"/>.</returns>
        /// <exception cref="DataParseException">When an exception occured while parsing the underlying JSON file.</exception>
        public BooksLibrary Parse()
        {
            try
            {
                if (!_sourceFileInfo.Exists)
                    throw new InvalidOperationException($"File does not exist: `{_sourceFileInfo.FullName}`");

                string? fileContents = File.ReadAllText(_sourceFileInfo.FullName, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(fileContents))
                    throw new InvalidOperationException($"Invalid file contents of `{_sourceFileInfo.FullName}`");

                BooksLibrary? instance = JsonSerializer.Deserialize<BooksLibrary>(fileContents, _jsonSerializerOptions);
                return instance ?? throw new InvalidOperationException($"Deserialized instance of {nameof(BooksLibrary)} was null.");
            }
            catch (Exception ex)
            {
                // we want to propagate a single exception type that wraps all others when attempting to read the file and failing at that
                throw new DataParseException($"Unable to parse json file `{_sourceFileInfo.Name}` to `{nameof(BooksLibrary)}`", ex);
            }
        }

        /// <summary>
        /// Maybe useless in this class, but i keep the same outline for all other classes used in app logic.
        /// </summary>
        public void Dispose()
        {
        }

        private static FileInfo ValidateFilePath(string sourceFileFullPath)
        {
            try
            {
                return new FileInfo(sourceFileFullPath);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Unable to instantiated a valid file path out of `{sourceFileFullPath}`", nameof(sourceFileFullPath));
            }
        }
    }
}
