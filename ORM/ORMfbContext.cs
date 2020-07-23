
namespace ORMModel
{
    using System;
    using Microsoft.EntityFrameworkCore;
    

    public partial class ORMdbContext : DbContext
    {
        public ORMdbContext(DbContextOptions<ORMdbContext> options)
           : base(options)
        {}

        public virtual DbSet<Company> Company { get; set; }

        public virtual DbSet<Import> Import { get; set; }

        public virtual DbSet<ImportError> ImportError { get; set; }

        public virtual DbSet<MasterMap> MasterMap { get; set; }

        public virtual DbSet<MasterMapColumn> MasterMapColumn { get; set; }

        public virtual DbSet<Person> Person { get; set; }

        public virtual DbSet<PersonMap> PersonMap { get; set; }

        public virtual DbSet<PersonMapColumn> PersonMapColumn { get; set; }

        public virtual DbSet<Sale> Sale { get; set; }

    }
}
