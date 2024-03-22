﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SistemaTurnos.Data;

#nullable disable

namespace SistemaTurnos.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("SistemaTurnos.Models.Asesor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Jwt")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Asesores");
                });

            modelBuilder.Entity("SistemaTurnos.Models.Turno", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Categoria")
                        .HasColumnType("int");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("HoraLlamada")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("HoraSolicitud")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("HoraTermino")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("NumeroTurno")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Turnos");
                });

            modelBuilder.Entity("SistemaTurnos.Models.TurnosAtendidos", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdAsesor")
                        .HasColumnType("int");

                    b.Property<int>("IdTurno")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdAsesor");

                    b.HasIndex("IdTurno");

                    b.ToTable("TurnosAtendidos");
                });

            modelBuilder.Entity("SistemaTurnos.Models.TurnosAtendidos", b =>
                {
                    b.HasOne("SistemaTurnos.Models.Asesor", "Asesor")
                        .WithMany()
                        .HasForeignKey("IdAsesor")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SistemaTurnos.Models.Turno", "Turno")
                        .WithMany()
                        .HasForeignKey("IdTurno")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asesor");

                    b.Navigation("Turno");
                });
#pragma warning restore 612, 618
        }
    }
}
