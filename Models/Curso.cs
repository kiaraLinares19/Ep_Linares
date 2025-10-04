using System.ComponentModel.DataAnnotations;

namespace Ep_Linares.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)] // Definir un tamaño razonable para el código
        public string Codigo { get; set; } // Único por restricción

        [Required]
        public string Nombre { get; set; }

        // Restricción: Créditos > 0 (Se manejará con Data Annotations y/o validación de negocio)
        [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores a cero.")]
        public int Creditos { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El cupo máximo debe ser mayor a cero.")]
        public int CupoMaximo { get; set; }

        [Required]
        public TimeSpan HorarioInicio { get; set; }

        [Required]
        public TimeSpan HorarioFin { get; set; }
        
        public bool Activo { get; set; } = true;

        // Propiedad de navegación
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}