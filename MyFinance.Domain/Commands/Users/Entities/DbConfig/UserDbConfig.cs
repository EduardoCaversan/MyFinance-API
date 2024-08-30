using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyFinance.Domain.Commands.Users.Entities.DbConfig
{
    public class UserDbConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users", schema: "dbo");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").HasMaxLength(8).HasColumnType("char(8)").UseCollation("Latin1_General_CS_AS").HasDefaultValueSql("[dbo].[new_id]()").IsRequired();
            builder.Property(p => p.Name).HasColumnName("name").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
            builder.Property(p => p.Email).HasColumnName("email").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired().IsUnicode();
            builder.HasIndex(p => p.Email).IsUnique();
            builder.Property(p => p.MobileNumber).HasColumnName("mobile_number").HasColumnType("varchar(15)").HasMaxLength(15);
            builder.Property(p => p.CreationDate).HasColumnName("creation_date").HasColumnType("datetimeoffset").IsRequired().HasDefaultValue(DateTimeOffset.Now);
            builder.Property(p => p.LastModified).HasColumnName("last_modified").HasColumnType("datetimeoffset");
            builder.Property(p => p.BirthdayDate).HasColumnName("birthday_date").HasColumnType("datetimeoffset");
            builder.Property(p => p.IsSysAdmin).HasColumnName("is_sys_admin").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.Property(p => p.Password).HasColumnName("password").HasColumnType("varchar(300)").IsRequired();
            builder.Property(p => p.AskNewPassword).HasColumnName("ask_new_password").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.Property(p => p.Removed).HasColumnName("removed").HasColumnType("bit").IsRequired().HasDefaultValue(false);
            builder.HasQueryFilter(p => !p.Removed);
        }
    }
}