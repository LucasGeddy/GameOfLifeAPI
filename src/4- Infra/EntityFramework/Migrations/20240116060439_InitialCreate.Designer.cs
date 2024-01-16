﻿// <auto-generated />
using GameOfLife.Infra.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GameOfLife.Migrations
{
    [DbContext(typeof(GameOfLifeDbContext))]
    [Migration("20240116060439_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GameOfLife.Domain.Entities.Board", b =>
                {
                    b.Property<int>("BoardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BoardId"));

                    b.Property<int>("Generation")
                        .HasColumnType("int");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.HasKey("BoardId");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("GameOfLife.Domain.Entities.Cell", b =>
                {
                    b.Property<int>("BoardId")
                        .HasColumnType("int");

                    b.Property<int>("PositionX")
                        .HasColumnType("int");

                    b.Property<int>("PositionY")
                        .HasColumnType("int");

                    b.HasKey("BoardId", "PositionX", "PositionY");

                    b.ToTable("Cells");
                });

            modelBuilder.Entity("GameOfLife.Domain.Entities.Cell", b =>
                {
                    b.HasOne("GameOfLife.Domain.Entities.Board", "Board")
                        .WithMany("LivingCells")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Board");
                });

            modelBuilder.Entity("GameOfLife.Domain.Entities.Board", b =>
                {
                    b.Navigation("LivingCells");
                });
#pragma warning restore 612, 618
        }
    }
}