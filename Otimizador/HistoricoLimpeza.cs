public class HistoricoLimpeza
{
    public DateTime DataHora { get; set; }
    public int ArquivosLimpados { get; set; }
    public long EspacoLiberado { get; set; } // em bytes

    public override string ToString()
    {
        return $"Data/Hora: {DataHora:dd/MM/yyyy HH:mm:ss}\n" +
               $"Arquivos Removidos: {ArquivosLimpados}\n" +
               $"Espaço Liberado: {EspacoLiberado / 1024.0 / 1024.0:F2} MB\n" +
               "----------------------------------------";
    }
} 