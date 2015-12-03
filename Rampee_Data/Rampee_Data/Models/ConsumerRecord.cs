using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rampee_Data.Models
{
    public class ConsumerRecord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Destination { get; set; }
        public bool Active { get; set; }

        public virtual ConnectionRecord Connection { get; set; }
        public virtual ICollection<MessageRecord> Messages { get; set; }

    }
}