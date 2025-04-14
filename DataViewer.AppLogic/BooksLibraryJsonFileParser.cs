using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DataViewer.Interfaces;
using DataViewer.Models.Data;
using DataViewer.Models.Exceptions;

namespace DataViewer.AppLogic
{
    public sealed class BooksLibraryJsonFileParser : IDataParser<BooksLibrary>
    {
        private readonly FileInfo _sourceFileInfo;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;

        public BooksLibraryJsonFileParser(string booksFileFullPath)
        {
            _sourceFileInfo = ValidateFilePath(booksFileFullPath);

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                // one can argue what the plausible defaults for handling JSON files may be, but since
                // it was not defined from the beginning, i simply chose some plausible defaults
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip, // JSONC like
                WriteIndented = true, // while useless during deserialization, this may be useful when serializer options are used during debug
                PropertyNameCaseInsensitive = true,
            };
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

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
                throw new DataParseException($"Unable to read file `{_sourceFileInfo.FullName}`", ex);
            }
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
