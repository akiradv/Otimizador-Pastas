using System.Text.Json;

namespace OtimizadorDePastas
{
    public class Settings
    {
        private static Settings? _current;
        public static Settings Current => _current ??= Load();

        public bool IsDarkMode { get; set; }
        public bool LimpezaAutomaticaAtivada { get; set; }
        public int IntervaloLimpeza { get; set; } = 60;
        public int UnidadeTempo { get; set; } = 0; // 0 = Minutos, 1 = Horas, 2 = Dias

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OtimizadorDePastas",
            "settings.json"
        );

        private static string HistoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OtimizadorDePastas",
            "history.json"
        );

        public static void Save()
        {
            var folder = Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder!);

            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }

        private static Settings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar configurações: {ex.Message}");
            }
            return new Settings();
        }

        public static void SaveHistory(List<HistoricoLimpeza> historico)
        {
            try
            {
                var folder = Path.GetDirectoryName(HistoryPath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder!);

                var json = JsonSerializer.Serialize(historico, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(HistoryPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar histórico: {ex.Message}");
            }
        }

        public static List<HistoricoLimpeza> LoadHistory()
        {
            try
            {
                if (File.Exists(HistoryPath))
                {
                    var json = File.ReadAllText(HistoryPath);
                    return JsonSerializer.Deserialize<List<HistoricoLimpeza>>(json) ?? new List<HistoricoLimpeza>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar histórico: {ex.Message}");
            }
            return new List<HistoricoLimpeza>();
        }
    }
} 