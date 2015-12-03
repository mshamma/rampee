using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rampee_Data.Models
{
    public class ConnectionRecord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public string Uri { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        public byte[] Password { get; set; }

        [StringLength(50)]
        public string ClearPass { get; set; }

        public byte[] Salt { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual ICollection<ConsumerRecord> ConsumerRecords { get; set; }
    }
}