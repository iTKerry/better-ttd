﻿// <auto-generated />
using IdentityServer.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IdentityServer.DataAccess.Migrations
{
    [DbContext(typeof(IdentityContext))]
    [Migration("20210326083212_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IdentityServer.DataAccess.Entities.User", b =>
                {
                    b.Property<string>("SubjectId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SubjectId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IdentityServer.DataAccess.Entities.UserClaim", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("SubjectId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserSubjectId")
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("UserSubjectId");

                    b.ToTable("Claims");
                });

            modelBuilder.Entity("IdentityServer.DataAccess.Entities.UserLogin", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LoginProvider")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("ProviderKey")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("SubjectId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserSubjectId")
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("UserSubjectId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("IdentityServer.DataAccess.Entities.UserClaim", b =>
                {
                    b.HasOne("IdentityServer.DataAccess.Entities.User", null)
                        .WithMany("Claims")
                        .HasForeignKey("UserSubjectId");
                });

            modelBuilder.Entity("IdentityServer.DataAccess.Entities.UserLogin", b =>
                {
                    b.HasOne("IdentityServer.DataAccess.Entities.User", null)
                        .WithMany("Logins")
                        .HasForeignKey("UserSubjectId");
                });

            modelBuilder.Entity("IdentityServer.DataAccess.Entities.User", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Logins");
                });
#pragma warning restore 612, 618
        }
    }
}
