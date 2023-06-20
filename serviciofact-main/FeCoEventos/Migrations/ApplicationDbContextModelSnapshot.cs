﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TFHKA.EventsDian.Infrastructure.Data.Context;

namespace FeCoEventos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TFHKA.EventsDian.Infrastructure.Data.Context.InvoiceEventTable", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("created_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("customer_identification")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("customer_type_identification")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("dian_message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("dian_response_datetime")
                        .HasColumnType("datetime2");

                    b.Property<int>("dian_status")
                        .HasColumnType("int");

                    b.Property<string>("document_id")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("event_id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("event_type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("event_uuid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("id_enterprise")
                        .HasColumnType("int");

                    b.Property<int>("invoice_id")
                        .HasColumnType("int");

                    b.Property<DateTime>("invoice_issuedate")
                        .HasColumnType("datetime2");

                    b.Property<string>("invoice_uuid")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("invoice_uuid_type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("namefile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("path_file")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("session_log")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.Property<string>("supplier_identification")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("supplier_type_identification")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("track_id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updated_at")
                        .HasColumnType("datetime2");

                    b.HasKey("id");

                    b.ToTable("invoice_events");
                });
#pragma warning restore 612, 618
        }
    }
}
