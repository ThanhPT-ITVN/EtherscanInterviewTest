//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EtherscanTest
{
    using System;
    using System.Collections.Generic;
    
    public partial class block
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public block()
        {
            this.transactions = new HashSet<transaction>();
        }
    
        public int blockID { get; set; }
        public int blockNumber { get; set; }
        public string hash { get; set; }
        public string parentHash { get; set; }
        public string miner { get; set; }
        public decimal blockReward { get; set; }
        public decimal gasLimit { get; set; }
        public decimal gasUsed { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<transaction> transactions { get; set; }
    }
}
