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
    [Migration("20230324224911_addInscripcionesToDatabase")]
    partial class addInscripcionesToDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

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

                    b.Property<string>("Fojas")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Manzana")
                        .HasColumnType("int");

                    b.Property<string>("NaturalezaEscritura")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("NumeroInscripcion")
                        .HasColumnType("int");

                    b.Property<string>("Predio")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Folio");

                    b.ToTable("Inscripciones");
                });
#pragma warning restore 612, 618
        }
    }
}
