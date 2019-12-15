﻿// <auto-generated />
using System;
using System.Net;
using LogBasePresenter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LogBasePresenter.Migrations
{
    [DbContext(typeof(LogBaseContext))]
    [Migration("20191215162858_LogRecords")]
    partial class LogRecords
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("LogBasePresenter.Models.LogRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<IPAddress>("Ip")
                        .HasColumnType("inet");

                    b.Property<DateTime>("RecordTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.Property<string>("UrlParameters")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("LogRecords");
                });
#pragma warning restore 612, 618
        }
    }
}