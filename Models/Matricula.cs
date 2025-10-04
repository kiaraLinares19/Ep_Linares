using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Ep_Linares.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        [EnumDataType(typeof(EstadoMatricula))]
        public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;

        public Curso Curso { get; set; }
        public IdentityUser Usuario { get; set; }
    }

    public enum EstadoMatricula
    {
        Pendiente,
        Confirmada,
        Cancelada
    }
}
