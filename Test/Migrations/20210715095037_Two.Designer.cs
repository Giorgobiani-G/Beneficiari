﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Test.Data;

namespace Test.Migrations
{
    [DbContext(typeof(BenDbContext))]
    [Migration("20210715095037_Two")]
    partial class Two
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Test.Models.Beneficiari", b =>
                {
                    b.Property<int>("Benid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Asaki")
                        .HasColumnType("int");

                    b.Property<string>("Gvari")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Misamarti")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Piradobisnomeri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Saxeli")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Benid");

                    b.ToTable("Beneficiaris");
                });

            modelBuilder.Entity("Test.Models.Visit", b =>
                {
                    b.Property<int>("Vsid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Gvari")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Mdgomareoba")
                        .HasColumnType("bit");

                    b.Property<string>("Piradoba")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Saxeli")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Vsid");

                    b.ToTable("Visits");
                });
#pragma warning restore 612, 618
        }
    }
}
