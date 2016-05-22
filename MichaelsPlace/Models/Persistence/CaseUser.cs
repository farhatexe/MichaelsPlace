using System.ComponentModel.DataAnnotations;

namespace MichaelsPlace.Models.Persistence
{
    /// <summary>
    ///     Represents a user's relationship to a case.
    /// </summary>
    public class CaseUser
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual Case Case { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public virtual Situation Situation { get; set; }
    }
}