﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SII_App_Grupo_5.Data;

#nullable disable

namespace SII_App_Grupo_5.Migrations
{
    [DbContext(typeof(InscriptionsGrupo5DbContext))]
    [Migration("20230522002543_floatPorcentajeDerecho")]
    partial class floatPorcentajeDerecho
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SII_App_Grupo_5.Models.Adquiriente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Acreditado")
                        .HasColumnType("bit");

                    b.Property<int>("InscripcionId")
                        .HasColumnType("int");

                    b.Property<float>("PorcentajeDerecho")
                        .HasColumnType("real");

                    b.Property<string>("Rut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("InscripcionId");

                    b.ToTable("Adquirientes");
                });

            modelBuilder.Entity("SII_App_Grupo_5.Models.Comuna", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Comunas");
                });

            modelBuilder.Entity("SII_App_Grupo_5.Models.Enajenante", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Acreditado")
                        .HasColumnType("bit");

                    b.Property<int>("InscripcionId")
                        .HasColumnType("int");

                    b.Property<float>("PorcentajeDerecho")
                        .HasColumnType("real");

                    b.Property<string>("Rut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("InscripcionId");

                    b.ToTable("Enajenantes");
                });

            modelBuilder.Entity("SII_App_Grupo_5.Models.Inscripcion", b =>
                {
                    b.Property<int>("Folio")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Folio"));

                    b.Property<string>("Comuna")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<DateTime>("FechaInscripcion")
                        .HasColumnType("datetime2");

                    b.Property<int>("Fojas")
                        .HasColumnType("int");

                    b.Property<int>("Manzana")
                        .HasColumnType("int");

                    b.Property<string>("NaturalezaEscritura")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("NumeroInscripcion")
                        .HasColumnType("int");

                    b.Property<int>("Predio")
                        .HasColumnType("int");

                    b.HasKey("Folio");

                    b.ToTable("Inscripciones");
                });

            modelBuilder.Entity("SII_App_Grupo_5.Models.MultiPropietario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AnoInscripcion")
                        .HasColumnType("int");

                    b.Property<int?>("AnoVigenciaFinal")
                        .HasColumnType("int");

                    b.Property<int>("AnoVigenciaInicial")
                        .HasColumnType("int");

                    b.Property<string>("Comuna")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaInscripcion")
                        .HasColumnType("datetime2");

                    b.Property<int>("Fojas")
                        .HasColumnType("int");

                    b.Property<int>("Manzana")
                        .HasColumnType("int");

                    b.Property<int>("NumeroInscripcion")
                        .HasColumnType("int");

                    b.Property<float>("PorcentajeDerecho")
                        .HasColumnType("real");

                    b.Property<int>("Predio")
                        .HasColumnType("int");

                    b.Property<string>("RutPropietario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MultiPropietarios");
                });

            modelBuilder.Entity("SII_App_Grupo_5.Models.Adquiriente", b =>
                {
                    b.HasOne("SII_App_Grupo_5.Models.Inscripcion", "Inscripcion")
                        .WithMany("Adquirientes")
                        .HasForeignKey("InscripcionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inscripcion");
                });

            modelBuilder.Entity("SII_App_Grupo_5.Models.Enajenante", b =>
                {
                    b.HasOne("SII_App_Grupo_5.Models.Inscripcion", "Inscripcion")
                        .WithMany("Enajenantes")
                        .HasForeignKey("InscripcionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inscripcion");
                });

            modelBuilder.Entity("SII_App_Grupo_5.Models.Inscripcion", b =>
                {
                    b.Navigation("Adquirientes");

                    b.Navigation("Enajenantes");
                });
#pragma warning restore 612, 618
        }
    }
}
