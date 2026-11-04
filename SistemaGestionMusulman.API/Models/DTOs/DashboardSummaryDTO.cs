namespace SistemaGestionMusulman.API.Models.DTOs
{
    public class DashboardSummaryDTO
    {
        public int TotalPerfiles { get; set; }
        public int TotalItemsKardex { get; set; }
        public int TotalAsistencias { get; set; }
        public int TotalMaterias { get; set; }

        public class ActualizarRolesDto
        {
            public string UserId { get; set; } = string.Empty;
            public List<string> RolesNuevos { get; set; } = new List<string>();
        }
    }
}