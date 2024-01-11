using Empleados.Core.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empleados.Infraestructura.Configuracion
{
    public class CompaniaConfiguracion : IEntityTypeConfiguration<Compania>
    {
        public void Configure(EntityTypeBuilder<Compania> builder)
        {
            builder.Property(c => c.Id).IsRequired();
            builder.Property(c => c.NombreCompania).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Direccion).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Telefono).IsRequired().HasMaxLength(40);
            builder.Property(c => c.Telefono2).IsRequired(false).HasMaxLength(40);
        }
    }
}
