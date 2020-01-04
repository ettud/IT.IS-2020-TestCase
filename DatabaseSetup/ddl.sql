create table "Country"
(
	"Id" integer primary key,
	"Name" text not null
);

create table "LogRecords"
(
	"Id" text primary key,
	"RecordTime" timestamp not null,
	"Ip" inet,
	"QueryDescription" json,
	"IpBit" bit(32)
);

create table "Subnet"
(
	"Id" serial primary key,
	"CountryId" integer not null references "Country" on delete cascade,
	"Broadcast" bit(32),
	"Network" bit(32)
);