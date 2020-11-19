namespace MediaPlayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("mp.album")]
    public partial class album
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public album()
        {
            media = new HashSet<medium>();
        }

        [Column(TypeName = "uint")]
        public long id { get; set; }

        [StringLength(45)]
        public string name { get; set; }

        [StringLength(255)]
        public string cover { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<medium> media { get; set; }
    }
}
